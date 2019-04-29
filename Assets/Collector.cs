using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour {
    private DeathController deathController;

    private void Start() {
        deathController = GetComponentInParent<DeathController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var collectible = other.gameObject.GetComponent<Collectible>();
        if (collectible) {
            deathController.PickupSoul();
            Destroy(other.gameObject);
        }
    }

}
