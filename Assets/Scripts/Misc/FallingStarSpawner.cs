using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStarSpawner : MonoBehaviour
{
    public GameObject star;
    public float padding = 30f;
    public float spawnRate = 1f;
    public float fallDuration = 5f;

    private float dx = Screen.width;
    private float dy = Screen.height;
    private enum ScreenSide { TOP = 0, RIGHT = 1, BOTTOM = 2, LEFT = 3 }
    private ScreenSide side;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(dx);
        Debug.Log(dy);
        InvokeRepeating("SpawnStar", 0f, spawnRate);
        side = ScreenSide.TOP;
    }

    void SpawnStar() {
        //ScreenSide side = (ScreenSide)Random.Range(0, 4);
        Vector3 spawnPos;

        switch (side) {
            case ScreenSide.TOP:
                spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, dx), dy + padding, 0f));
                break;
            case ScreenSide.RIGHT:
                spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(dx + padding, Random.Range(0, dy), 0f));
                break;
            case ScreenSide.BOTTOM:
                spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, dx), 0 - padding, 0f));
                break;
            default:
                spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(0 - padding, Random.Range(0, dy), 0f));
                break;
        }
        side = (ScreenSide)(((int)side + 1) % 4);
        spawnPos.z = 0f;
        Instantiate(star, spawnPos, Quaternion.identity);
        star.GetComponent<FallingStar>().duration = fallDuration;
    }
}
