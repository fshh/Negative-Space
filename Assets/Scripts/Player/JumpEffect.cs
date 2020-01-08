using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEffect : MonoBehaviour
{
    public float duration = 1f;

    private GameObject sprite;

    public void PlayEffect()
    {
        sprite = transform.Find("Sprite").gameObject;
        StartCoroutine(EffectRoutine());
    }

    IEnumerator EffectRoutine()
    {
        float t = 0f;
        while (t < duration)
        {
            GameObject trail = Instantiate(sprite, sprite.transform.position, sprite.transform.rotation);
            trail.GetComponent<Animator>().enabled = false;
            trail.AddComponent<Fade>();
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
