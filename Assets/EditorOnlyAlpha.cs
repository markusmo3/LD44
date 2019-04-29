using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class EditorOnlyAlpha : MonoBehaviour {
    private SpriteRenderer sr;
    private Tilemap tr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Tilemap>();
        if (!sr && !tr) {
            throw new Exception("Lawl dats wrong tho");
        }
        OnEnable();
    }

    private void OnEnable() {
        if (!sr && !tr) {
            return;
        }
        if (Application.isPlaying) {
            SetAlpha(0.0f);
        } else {
            SetAlpha(1.0f);
        }
    }

    public void SetAlpha(float alpha) {
        if (sr) {
            var srColor = sr.color;
            srColor.a = alpha;
            sr.color = srColor;
        }

        if (tr) {
            var trColor = tr.color;
            trColor.a = alpha;
            tr.color = trColor;
        }
    }
}