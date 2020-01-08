using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stompable : MonoBehaviour {

    public Collider2D bounceColl;

    private Player player;
    private Collider2D playerColl;
    private AudioSource audioSource;
    private Animator anim;

	// Use this for initialization
	void Start () {
        GameObject playerObj = GameObject.Find("Player");
        player = playerObj.GetComponent<Player>();
        playerColl = playerObj.GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		if (bounceColl && bounceColl.IsTouching(playerColl) && !player.IsGrounded()) {
            player.Bounce();
            audioSource.Play();
            anim.SetTrigger("Die");
            GameObject colls = transform.parent.Find("Colliders").gameObject;
            GetComponentInParent<Controller2D>().enabled = false;
            if (GetComponentInParent<Slime>()) {
                GetComponentInParent<Slime>().enabled = false;
            } else if (GetComponentInParent<Snail>()) {
                GetComponentInParent<Snail>().enabled = false;
            }
            Destroy(colls);
        }
	}

    public void Kill() {
        Destroy(transform.parent.gameObject);
    }
}
