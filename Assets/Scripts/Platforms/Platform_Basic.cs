using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Basic : Platform {

    public float fadeTime = 0.2f;

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
