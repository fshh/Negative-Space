using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour {

    private int numberOfLayers, peelCounter;
    private Platform[][] platforms;

	// Use this for initialization
	void Start () {
        numberOfLayers = transform.childCount;
        peelCounter = 0;
        platforms = new Platform[numberOfLayers][];

        for (int i = 0; i < numberOfLayers; i++) {
            Transform layer = transform.GetChild(i);
            int platformsInLayer = layer.childCount;
            platforms[i] = new Platform[platformsInLayer];

            for (int j = 0; j < platformsInLayer; j++) {
                Platform platform = layer.GetChild(j).gameObject.GetComponent<Platform>();
                platform.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.black, (float)i / numberOfLayers);
                platforms[i][j] = platform;
            }
        }
	}

    public void Peel() {
        peelCounter++;

        for (int i = 0; i < numberOfLayers; i++) {
            for (int j = 0; j < transform.GetChild(i).childCount; j++) {
                if (platforms[i][j].gameObject.activeSelf) {
                    platforms[i][j].GetComponent<Platform>().Peel(peelCounter, i, numberOfLayers);
                }
            }
        }
    }
}
