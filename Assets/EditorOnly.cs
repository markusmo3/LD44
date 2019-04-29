using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnly : MonoBehaviour {
    private void OnEnable() {
        var gos = GameObject.FindGameObjectsWithTag("EditorOnly");
        foreach (var go in gos) {
            go.SetActive(false);
        }
    }
}