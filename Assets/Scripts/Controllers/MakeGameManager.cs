using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeGameManager : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Awake () {
        GameObject gm = GameObject.Find("GameManager");
        if (gm == null) {
            GameObject instance = Instantiate(prefab);
            instance.name = "GameManager";
        }
        Destroy(gameObject);
	}
}
