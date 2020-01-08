using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    public float exitAnimationDuration;

    private UIManager ui;
    private AudioSource sound;

    // Use this for initialization
    void Start () {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerInput>().DisableMovement();
            sound.Play();
            StartCoroutine(ExitRoutine(collision.gameObject));
        }
    }

    IEnumerator ExitRoutine(GameObject player) {
        Vector3 origPos = player.transform.position;
        float t = 0f;
        while (t < 1f) {
            player.transform.position = Vector3.Lerp(origPos, transform.position, t);
            player.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            player.transform.Rotate(new Vector3(0f, 0f, 5f));
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime / exitAnimationDuration;
        }
        ui.TransitionOutAdvance();
    }
}
