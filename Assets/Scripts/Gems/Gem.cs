using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

    public float gravity = 2f;
    public Vector3 carryOffset;
    public Vector3 carryScale;
    public float carryMaxSpeed = 3f;
    public float carrySlowDistance = 6f;

    private Vector3 carryVelocity;
    private Vector2 sittingVelocity;
    private GameManager gameManager;
    private BoxCollider2D coll;
    private GameObject player;
    private Controller2D controller;
    private bool carrying;
    private bool transitioning;
    private AudioSource audioSource;

    // Use this for initialization
    void Start () {
        gameManager = FindObjectOfType<GameManager>();
        coll = GetComponent<BoxCollider2D>();
        player = GameObject.Find("Player");
        controller = GetComponent<Controller2D>();
        carrying = false;
        transitioning = false;
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (carrying) {
            // Calculate the desired velocity
            Vector3 offset = new Vector3(Mathf.Clamp(transform.position.x - player.transform.position.x, -carryOffset.x, carryOffset.x), carryOffset.y, 0f);
            Vector3 desired_velocity = player.transform.position + offset - transform.position;
            float distance = desired_velocity.magnitude;

            // Check the distance to detect whether the character
            // is inside the slowing area
            if (distance < carrySlowDistance) {
                // Inside the slowing area
                desired_velocity = Vector3.Normalize(desired_velocity) * carryMaxSpeed * (distance / carrySlowDistance);
            } else {
                // Outside the slowing area
                desired_velocity = Vector3.Normalize(desired_velocity) * carryMaxSpeed;
            }

            // Set the steering based on this
            Vector3 steering = desired_velocity - carryVelocity;
            carryVelocity = Truncate(carryVelocity + steering, carryMaxSpeed);
            transform.position += carryVelocity;
        } else if (!transitioning) {
            sittingVelocity.y -= gravity * Time.deltaTime;
            controller.Move(sittingVelocity);

            if (controller.collisions.above || controller.collisions.below) {
                sittingVelocity.y = 0f;
            }
            
            if (coll.bounds.Intersects(player.GetComponent<Collider2D>().bounds)) {
                audioSource.Play();
                gameManager.AcquireGem();
                StartCoroutine(TransitionToCarry());
            }
        }
    }

    IEnumerator TransitionToCarry() {
        transitioning = true;
        for (float t = 0.01f;  t <= 1f; t += 0.1f / (1.01f + t * 2)) {
            Vector3 offset = new Vector3(Mathf.Clamp(transform.position.x - player.transform.position.x, -carryOffset.x, carryOffset.x), carryOffset.y, 0f);
            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, t);
            transform.localScale = Vector3.Lerp(Vector3.one, carryScale, t);
            yield return new WaitForEndOfFrame();
        }
        transitioning = false;
        carrying = true;
    }

    Vector3 Truncate(Vector3 vector, float max) {
        float i;
        i = max / vector.magnitude;
        i = i < 1.0f ? i : 1.0f;
        return vector *= i;
    }
}
