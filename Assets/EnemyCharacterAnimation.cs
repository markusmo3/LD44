using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterAnimation : MonoBehaviour {

    public AudioSource movementAudioSource;

    public void PlayJump() {
        movementAudioSource.Play();
    }
}