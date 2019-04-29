using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public Animator death;
    public Animator slime;

    private void Start() {
        // just in case reset timescale here...
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void Update() {
        if (!death.GetBool("attack")) {
            death.SetTrigger("attack");
        }
        if (!slime.GetBool("attack")) {
            slime.SetTrigger("attack");
        }

        if (Input.GetButtonDown("Menu")) {
            SceneManager.LoadScene(Scenes.GAME);
        }
    }
}