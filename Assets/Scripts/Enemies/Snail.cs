using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : MonoBehaviour {

    public float gravity = 2f;
    public float speed = 5f;
    public PolygonCollider2D shell;
    public BoxCollider2D head, tail;

    private Vector2 velocity;
    private Controller2D controller;
    private int direction = 1;
    private SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        velocity = new Vector2();
	}
	
	// Update is called once per frame
	void Update () {
        velocity.x = direction * speed * Time.deltaTime;
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity);

        if (controller.collisions.below || controller.collisions.above) {
            velocity.y = 0f;
        }
        
        if ((controller.collisions.right && direction == 1) || (controller.collisions.left && direction == -1)) {
            sprite.flipX = !sprite.flipX;
            head.offset *= new Vector2(-1, 1);
            tail.offset *= new Vector2(-1, 1);
            direction *= -1;
            if (direction == 1) {
                shell.offset = Vector2.zero;
            } else {
                shell.offset = new Vector2(0.5f, 0);
            }
        }
	}
}
