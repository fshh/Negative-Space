using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAfterDelay : MonoBehaviour {

    public float fadeTime = 4f;

    private bool activated = false;
    private Collider2D coll;
    private Bounds bounds;
    private GameObject player;

    void Start () {
        coll = GetComponent<Collider2D>();
        bounds = new Bounds(transform.position, new Vector3(coll.bounds.extents.x * 2, coll.bounds.extents.y * 2, 20f));
        player = GameObject.Find("Player");
    }

    void Update () {
        if (!activated && bounds.Contains(player.transform.position)) {
            StartCoroutine(FadeIn());
            activated = true;
        }
    }

    IEnumerator FadeIn() {
        float timer = 0f;
        while (timer <= fadeTime) {
            timer += Time.deltaTime;
            foreach (Transform t in transform) {
                if (t.GetComponent<SpriteRenderer>()) {
                    t.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.clear, Color.white, Mathf.Clamp01(timer / fadeTime));
                }
                if (t.GetComponent<TextMesh>()) {
                    t.GetComponent<TextMesh>().color = Color.Lerp(Color.clear, Color.white, Mathf.Clamp01(timer / fadeTime));
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
