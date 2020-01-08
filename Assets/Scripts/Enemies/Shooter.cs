using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {

    public GameObject bullet;
    public float shotSpeed = 10f;
    public float shotDespawnDelay = 10f;
    public float fireDelay = 2f;

    private float timeSinceShot = 0f;
    private Transform shotOrigin;

	// Use this for initialization
	void Start () {
        shotOrigin = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
        timeSinceShot += Time.deltaTime;
        if (timeSinceShot > fireDelay) {
            GameObject shot = Instantiate(bullet, shotOrigin.position, transform.rotation);
            shot.GetComponent<Bullet>().speed = shotSpeed;
            shot.GetComponent<Bullet>().despawnDelay = shotDespawnDelay;
            timeSinceShot = 0f;
        }
	}
}
