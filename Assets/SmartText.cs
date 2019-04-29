using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SmartText : MonoBehaviour {

    private TextMeshProUGUI textMesh;
    private string startText;

    void Start() {
        textMesh = GetComponent<TextMeshProUGUI>();
        startText = textMesh.text;
    }

    public void SetText(params object[] parameters) {
        textMesh.text = string.Format(startText, parameters);
    }
}