using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {
    private Camera cam;

    void Start() {
        cam = Camera.main;
    }

    void Update() {
        var mouseInWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        var aimVec = mouseInWorld.to2D() - transform.position.to2D();
        transform.right = aimVec;
    }
}