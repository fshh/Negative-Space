using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Gravity : Platform {

    public float fadeTime = 0.2f;
    public float gravity = 2f;

    private Vector2 velocity;
    private Controller2D controller;
    private Vector2 movingPlatformOffset;

    private void OnEnable() {
        velocity = Vector2.zero;
        controller = GetComponent<Controller2D>();
        movingPlatformOffset = Vector2.zero;
    }

    private void FixedUpdate() {
        velocity.y -= gravity * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, coll.bounds.size, 0f, velocity, velocity.magnitude * Time.fixedDeltaTime, 1 << LayerMask.NameToLayer("Platform"));
        if (hit) {
            if (hit.transform.GetComponent<PlatformController>()) {
                if (movingPlatformOffset == Vector2.zero) {
                    movingPlatformOffset = hit.transform.position - transform.position;
                }
                transform.position = (Vector2)hit.transform.position - movingPlatformOffset;
            }
            velocity.y = 0f;
        } else if (movingPlatformOffset != Vector2.zero) {
            movingPlatformOffset = Vector2.zero;
        }
        controller.Move(velocity);
    }

    public override void Peel(int peelCounter, int layer, int numberOfLayers) {
        Color finish = Color.Lerp(Color.white, Color.black, (float)(layer + peelCounter) / numberOfLayers);
        StartCoroutine(Fade(spriteRenderer.color, finish));
        if (finish == Color.black) {
            coll.enabled = false;
        }
    }

    IEnumerator Fade(Color start, Color finish) {
        float t = 0f;
        while (t < fadeTime) {
            spriteRenderer.color = Color.Lerp(start, finish, Mathf.Clamp01(t / fadeTime));
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        spriteRenderer.color = finish;
        if (finish == Color.black) {
            gameObject.SetActive(false);
            StopAllCoroutines();
        }
    }
}
