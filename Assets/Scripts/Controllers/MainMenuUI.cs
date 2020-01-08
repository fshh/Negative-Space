using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private GameManager gm;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        anim.SetTrigger("TransitionIn");
    }

    public void PlayGame() {
        anim.SetTrigger("TransitionOutAdvance");
    }

    public void Advance() {
        gm.AdvanceToNextLevel();
    }

    public void QuitGame() {
        gm.Quit();
    }
}
