using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStar : MonoBehaviour
{
    public float duration = 5f;

    private float t;
    private Vector3 startPos;
    private Vector3 origScale;
    private Transform sprite;

    // Start is called before the first frame update
    void Start()
    {
        t = 0f;
        startPos = transform.position;
        sprite = transform.Find("Sprite");
        origScale = sprite.localScale;
        JumpEffect effect = GetComponent<JumpEffect>();
        effect.duration = duration;
        effect.PlayEffect();
    }

    // Update is called once per frame
    void Update()
    {
        if (t >= 1f) {
            Destroy(gameObject);
        }
        transform.position = Vector3.Lerp(startPos, Vector3.zero, t);
        sprite.localScale = Vector3.Lerp(origScale, Vector3.zero, t);
        t += Time.deltaTime / duration;
    }
}
