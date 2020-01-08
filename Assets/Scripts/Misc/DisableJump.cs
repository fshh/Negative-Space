using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableJump : MonoBehaviour {

    private PlayerInput input;

	// Use this for initialization
	void Start () {
        input = GameObject.Find("Player").GetComponent<PlayerInput>();
        input.DisableJump();
	}
}
