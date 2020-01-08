using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PlatformerController : MonoBehaviour {
    #region Fields

    public float m_rayDistance;
    public float m_slopeLimitAngle;

    public float gravity, fallMaxSpeed, minSpeedThreshold, groundFriction, airFriction, maxSpeed, acceleration, jumpHeight;

    private BoxCollider2D m_boxCollider;

    private Transform thisTransform;
    private Vector3 colliderCenter, colliderPosition, colliderSize;
    private float colliderBottom, colliderTop, colliderLeft, colliderRight;

    private int m_colLayerMask;

    private RaycastHit m_hitBottom;
    private RaycastHit m_hitBottomLeft;
    private RaycastHit m_hitBottomRight;
    private RaycastHit m_hitTop;
    private RaycastHit m_hitTopLeft;
    private RaycastHit m_hitTopRight;
    private RaycastHit m_hitLeft;
    private RaycastHit m_hitLeftTop;
    private RaycastHit m_hitLeftBottom;
    private RaycastHit m_hitRight;
    private RaycastHit m_hitRightTop;
    private RaycastHit m_hitRightBottom;

    private float m_limitMinY;
    private float m_limitMaxY;
    private float m_limitMinX;
    private float m_limitMaxX;

    private float m_posMinY;
    private float m_posMaxY;
    private float m_posMinX;
    private float m_posMaxX;

    private bool m_isGrounded;
    private bool m_isTopBlocked;
    private bool m_isLeftBlocked;
    private bool m_isRightBlocked;
    private bool m_isAboveSlope;
    private bool m_isOnSlope;

    private bool m_isRunning;
    private bool m_isJumping;

    private Vector2 m_speed;

    #endregion

    #region Properties

    public Vector2 Speed { get { return m_speed; } }
    public bool IsGrounded { get { return m_isGrounded; } }
    public bool IsJumping { get { return m_isJumping; } }

    #endregion

    #region Init

    void Awake() {
        m_boxCollider = gameObject.GetComponent<BoxCollider2D>();

        thisTransform = transform;

        m_colLayerMask = 1 << LayerMask.NameToLayer("MapCollisions");
    }

    void Start() {
        colliderCenter = Vector3.Scale(thisTransform.localScale, m_boxCollider.offset);
        colliderPosition = thisTransform.position + colliderCenter;
        colliderSize = Vector3.Scale(thisTransform.localScale, m_boxCollider.size);
        colliderBottom = colliderPosition.y - (colliderSize.y / 2);
        colliderTop = colliderPosition.y + (colliderSize.y / 2);
        colliderLeft = colliderPosition.x - (colliderSize.x / 2);
        colliderRight = colliderPosition.x + (colliderSize.x / 2);

        m_limitMinY = ComputeLimitBottom();
        m_limitMinX = ComputeLimitLeft();
        m_limitMaxX = ComputeLimitRight();
        m_limitMaxY = ComputeLimitTop();
    }

    #endregion

    private void Update() {
        Run(Input.GetAxis("Horizontal"), maxSpeed);

        if (Input.GetButton("Jump")) {
            Jump(jumpHeight);
        }
    }

    void FixedUpdate() {
        ///////////////////////////////// Compute limits and max positions ////////////////////////////////////
        #region Compute limits

        // Determine position and bounds of collider
        colliderCenter = Vector3.Scale(thisTransform.localScale, m_boxCollider.offset);
        colliderPosition = thisTransform.position + colliderCenter;
        colliderSize = Vector3.Scale(thisTransform.localScale, m_boxCollider.size);
        colliderBottom = colliderPosition.y - (colliderSize.y / 2);
        colliderTop = colliderPosition.y + (colliderSize.y / 2);
        colliderLeft = colliderPosition.x - (colliderSize.x / 2);
        colliderRight = colliderPosition.x + (colliderSize.x / 2);

        // Compute limits when needed

        m_limitMinY = ComputeLimitBottom();

        if (m_speed.x < 0)
            m_limitMinX = ComputeLimitLeft();

        if (m_speed.x > 0)
            m_limitMaxX = ComputeLimitRight();

        if (m_speed.y > 0)
            m_limitMaxY = ComputeLimitTop();

        // Calculate collider position for each limits
        m_posMinY = m_limitMinY + m_boxCollider.size.y / 2 - m_boxCollider.offset.y;
        m_posMaxY = m_limitMaxY - m_boxCollider.size.y / 2 - m_boxCollider.offset.y;
        m_posMinX = m_limitMinX + m_boxCollider.size.x / 2 - m_boxCollider.offset.x;
        m_posMaxX = m_limitMaxX - m_boxCollider.size.x / 2 - m_boxCollider.offset.x;

        #endregion

        //////////////////////////////////////// Handle collisions ////////////////////////////////////////////
        #region Handle collisions

        // Define controller state (bigger buffer to leave grounded state if on a slope)
        m_isGrounded = colliderBottom <= (m_isAboveSlope ? m_limitMinY + 5 : m_limitMinY + 1);
        m_isTopBlocked = colliderTop >= m_limitMaxY - 1;
        m_isLeftBlocked = colliderLeft <= m_limitMinX + 1;
        m_isRightBlocked = colliderRight >= m_limitMaxX - 1;


        // If on ground, turn off vertical speed and set on slope if above one
        if (m_isGrounded && !m_isJumping) {
            m_speed.y = 0;
            m_isOnSlope = m_isAboveSlope;
        }
        // If in the air, apply gravity
        else {
            //m_speed.y = Mathf.Max(m_speed.y - GameData.Physics.gravity * Time.deltaTime, -GameData.Physics.fallMaxSpeed);
            m_speed.y = Mathf.Max(m_speed.y - gravity * Time.deltaTime, -fallMaxSpeed);
            m_isOnSlope = false;
        }

        // If left/right blocked, turn off horizontal speed
        if ((m_isLeftBlocked && m_speed.x < 0) || (m_isRightBlocked && m_speed.x > 0))
            m_speed.x = 0;

        // If not running, apply friction
        if (!m_isRunning) {
            //if (Mathf.Abs(m_speed.x) < GameData.PlayerRun.minSpeedThreshold)
            if (Mathf.Abs(m_speed.x) < minSpeedThreshold)
                m_speed.x = 0;
            else
                //m_speed.x -= (m_isGrounded ? GameData.Physics.groundFriction : GameData.Physics.airFriction) * Mathf.Sign(m_speed.x) * Time.deltaTime;
                m_speed.x -= (m_isGrounded ? groundFriction : airFriction) * Mathf.Sign(m_speed.x) * Time.deltaTime;
        }

        if (m_isJumping) {
            // If top blocked while jumping, turn off vertical speed
            if (m_isTopBlocked)
                m_speed.y = 0;

            // Exit jumping state when apex reached
            if (m_speed.y <= 0)
                m_isJumping = false;
        }

        #endregion

        /////////////////////////////////////////// Apply speed  /////////////////////////////////////////
        #region Apply speed

        // Snap controller to ground if on slope
        if (m_isOnSlope)
            transform.position = new Vector3(transform.position.x, m_posMinY, 0f);
        //transform.SetPosY(m_posMinY);

        // Else, apply vertical speed
        else
            transform.position= new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + m_speed.y * Time.deltaTime, m_posMinY, m_posMaxY), 0f);
        //transform.SetPosY(Mathf.Clamp(transform.position.y + m_speed.y * Time.deltaTime, m_posMinY, m_posMaxY));

        // Apply horizontal speed
        Debug.Log(m_speed.x);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + m_speed.x * Time.deltaTime, m_posMinX, m_posMaxX), transform.position.y, 0f);
        //transform.SetPosX(Mathf.Clamp(transform.position.x + m_speed.x * Time.deltaTime, m_posMinX, m_posMaxX));
        #endregion

        m_isRunning = false;
    }

    #region Compute limits methods

    private float ComputeLimitBottom() {
        Ray rayBottomLeft = new Ray(new Vector3(colliderLeft + 2, colliderPosition.y, 0), Vector3.down);
        Ray rayBottomRight = new Ray(new Vector3(colliderRight - 2, colliderPosition.y, 0), Vector3.down);
        Ray rayBottom = new Ray(new Vector3(colliderPosition.x, colliderPosition.y, 0), Vector3.down);

        Debug.DrawRay(new Vector3(colliderLeft, colliderPosition.y, 0), Vector3.down, Color.red);
        Debug.DrawRay(new Vector3(colliderRight, colliderPosition.y, 0), Vector3.down, Color.red);
        Debug.DrawRay(new Vector3(colliderPosition.x, colliderPosition.y, 0), Vector3.down, Color.red);

        Vector3 limitBottomLeft = rayBottomLeft.origin + Vector3.down * m_rayDistance;
        Vector3 limitBottomRight = rayBottomRight.origin + Vector3.down * m_rayDistance;
        Vector3 limitBottom = rayBottom.origin + Vector3.down * m_rayDistance;

        bool slopeLeft = false;
        bool slopeRight = false;

        if (Physics.Raycast(rayBottomLeft, out m_hitBottomLeft, m_rayDistance, m_colLayerMask)) {
            limitBottomLeft = m_hitBottomLeft.point;
            Debug.Log(limitBottomLeft);

            slopeLeft = Mathf.Abs(Vector3.Angle(m_hitBottomLeft.normal, Vector3.right) - 90) >= 5;
        }

        if (Physics.Raycast(rayBottomRight, out m_hitBottomRight, m_rayDistance, m_colLayerMask)) {
            limitBottomRight = m_hitBottomRight.point;

            slopeRight = Mathf.Abs(Vector3.Angle(m_hitBottomRight.normal, Vector3.right) - 90) >= 5;
        }

        m_isAboveSlope = (slopeLeft && slopeRight) ||
                       (slopeLeft && !slopeRight && limitBottomLeft.y >= limitBottomRight.y) ||
                       (!slopeLeft && slopeRight && limitBottomRight.y >= limitBottomLeft.y);

        if (m_isAboveSlope) {
            if (Physics.Raycast(rayBottom, out m_hitBottom, m_rayDistance, m_colLayerMask)) {
                limitBottom = m_hitBottom.point;
            }

            if (slopeLeft && limitBottomLeft.y - limitBottom.y > 5)
                return limitBottomLeft.y;

            else if (slopeRight && limitBottomRight.y - limitBottom.y > 5)
                return limitBottomRight.y;

            else
                return limitBottom.y;
        } else {

            return Mathf.Max(limitBottomLeft.y, limitBottomRight.y);
        }
    }

    private float ComputeLimitTop() {
        Ray rayTopLeft = new Ray(new Vector3(colliderLeft + 2, colliderPosition.y, 0), Vector3.up);
        Ray rayTopRight = new Ray(new Vector3(colliderRight - 2, colliderPosition.y, 0), Vector3.up);

        Vector3 limitTopLeft = rayTopLeft.origin + Vector3.up * m_rayDistance;
        Vector3 limitTopRight = rayTopRight.origin + Vector3.up * m_rayDistance;

        if (Physics.Raycast(rayTopLeft, out m_hitTopLeft, m_rayDistance, m_colLayerMask)) {
            limitTopLeft = m_hitTopLeft.point;
        }

        if (Physics.Raycast(rayTopRight, out m_hitTopRight, m_rayDistance, m_colLayerMask)) {
            limitTopRight = m_hitTopRight.point;
        }

        return Mathf.Min(limitTopLeft.y, limitTopRight.y);
    }

    private float ComputeLimitLeft() {
        Ray rayLeft = new Ray(new Vector3(colliderRight - 2, colliderPosition.y, 0), Vector3.left);
        Ray rayLeftTop = new Ray(new Vector3(colliderRight - 2, colliderTop - 2, 0), Vector3.left);
        Ray rayLeftBottom = new Ray(new Vector3(colliderRight - 2, colliderBottom + 2, 0), Vector3.left);

        Vector3 limitLeft = rayLeft.origin + Vector3.left * m_rayDistance;
        Vector3 limitLeftTop = rayLeftTop.origin + Vector3.left * m_rayDistance;
        Vector3 limitLeftBottom = rayLeftTop.origin + Vector3.left * m_rayDistance;

        if (Physics.Raycast(rayLeft, out m_hitLeft, m_rayDistance, m_colLayerMask)) {
            if (Vector3.Angle(m_hitLeft.normal, Vector3.up) > m_slopeLimitAngle)
                limitLeft = m_hitLeft.point;
        }

        if (Physics.Raycast(rayLeftTop, out m_hitLeftTop, m_rayDistance, m_colLayerMask)) {
            if (Vector3.Angle(m_hitLeftTop.normal, Vector3.up) > m_slopeLimitAngle)
                limitLeftTop = m_hitLeftTop.point;
        }

        if (Physics.Raycast(rayLeftBottom, out m_hitLeftBottom, m_rayDistance, m_colLayerMask)) {
            if (Vector3.Angle(m_hitLeftBottom.normal, Vector3.up) > m_slopeLimitAngle)
                limitLeftBottom = m_hitLeftBottom.point;
        }

        return Mathf.Max(limitLeft.x, limitLeftTop.x, limitLeftBottom.x);
    }

    private float ComputeLimitRight() {
        Ray rayRight = new Ray(new Vector3(colliderLeft + 2, colliderPosition.y, 0), Vector3.right);
        Ray rayRightTop = new Ray(new Vector3(colliderLeft + 2, colliderTop - 2, 0), Vector3.right);
        Ray rayRightBottom = new Ray(new Vector3(colliderLeft + 2, colliderBottom + 2, 0), Vector3.right);

        Vector3 limitRight = rayRight.origin + Vector3.right * m_rayDistance;
        Vector3 limitRightTop = rayRightTop.origin + Vector3.right * m_rayDistance;
        Vector3 limitRightBottom = rayRightBottom.origin + Vector3.right * m_rayDistance;

        if (Physics.Raycast(rayRight, out m_hitRight, m_rayDistance, m_colLayerMask)) {
            if (Vector3.Angle(m_hitRight.normal, Vector3.up) > m_slopeLimitAngle)
                limitRight = m_hitRight.point;
        }

        if (Physics.Raycast(rayRightTop, out m_hitRightTop, m_rayDistance, m_colLayerMask)) {
            if (Vector3.Angle(m_hitRightTop.normal, Vector3.up) > m_slopeLimitAngle)
                limitRightTop = m_hitRightTop.point;
        }

        if (Physics.Raycast(rayRightBottom, out m_hitRightBottom, m_rayDistance, m_colLayerMask)) {
            if (Vector3.Angle(m_hitRightBottom.normal, Vector3.up) > m_slopeLimitAngle)
                limitRightBottom = m_hitRightBottom.point;
        }

        return Mathf.Min(limitRight.x, limitRightTop.x, limitRightBottom.x);
    }

    #endregion

    #region Move methods

    public void Run(float p_acceleration, float p_maxSpeed) {
        m_isRunning = true;
        m_speed.x = Mathf.Clamp(m_speed.x + p_acceleration * Time.deltaTime, -p_maxSpeed, p_maxSpeed);
    }

    public void Jump(float p_height) {
        //float jumpImpulsion = Trajectories.ComputeJumpImpulse(p_height);
        float jumpImpulsion = p_height;

        m_speed.y = jumpImpulsion;
        m_isJumping = true;
        m_isGrounded = false;
    }

    #endregion
}