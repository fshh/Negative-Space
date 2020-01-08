using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed = 10f;
    public float despawnDelay = 20f;

    private float despawnTimer = 0f;

    // Update is called once per frame
    void Update() {
        if (despawnTimer >= despawnDelay) {
            Destroy(gameObject);
        }
        despawnTimer += Time.deltaTime;

        transform.position += -transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Platform")) {
            Destroy(gameObject);
        }
    }
}
