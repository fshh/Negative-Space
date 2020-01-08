using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour {

    private UIManager ui;

    // Use this for initialization
    void Start () {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<Player>().Die();
            ui.TransitionOutRestart();
        }
    }
}
