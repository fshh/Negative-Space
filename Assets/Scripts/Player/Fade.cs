using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public float duration = 0.5f;
    public Color end = Color.black;

    private float t = 0f;
    private SpriteRenderer sprite;
    private Color start;
    private Vector3 origScale;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        start = sprite.color;
        origScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / duration;
        if (t >= 1f) {
            Destroy(gameObject);
        }
        transform.localScale = Vector3.Lerp(origScale, Vector3.zero, t);
        sprite.color = Color.Lerp(start, end, t);
        sprite.sortingOrder--;
    }
}
