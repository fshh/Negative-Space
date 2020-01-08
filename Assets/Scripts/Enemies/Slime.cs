using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour {

    public float gravity = 2f;

    private Vector2 velocity;
    private Controller2D controller;

    // Use this for initialization
    void Start () {
        controller = GetComponent<Controller2D>();
        velocity = new Vector2();
    }
	
	// Update is called once per frame
	void Update () {
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity);

        if (controller.collisions.below || controller.collisions.above) {
            velocity.y = 0f;
        }
    }
}
