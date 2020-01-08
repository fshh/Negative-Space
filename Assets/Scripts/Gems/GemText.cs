using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemText : MonoBehaviour {

    public int totalGems = 4;

	// Use this for initialization
	void Start () {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        TextMesh mesh = GetComponent<TextMesh>();
        mesh.text = "You collected " + gm.GetGemCount() + "/" + totalGems + " gems.";
	}
}
