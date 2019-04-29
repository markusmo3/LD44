using System;
using UnityEditor;
using UnityEngine;

public class Orbital : MonoBehaviour {

    public float radius = 1f;
    public bool resetXYAngles = true;
    public bool resetZAngle = false;
    public float smoothMovementFactor = 0.8f;

    [Tooltip("in degree per second")]
    public float speed;

    private Transform[] children;
    private float curOffset;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Handles.color = Color.magenta;
        Handles.DrawLine(transform.position, transform.position + transform.forward);
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, transform.forward, radius);
    }
#endif

    private void Start() {
        OnTransformChildrenChanged();
    }

    private void OnTransformChildrenChanged() {
        children = GetComponentsInChildren<Transform>();
    }

    private void Update() {
        curOffset += speed * Time.deltaTime;
        if (curOffset > 360f) {
            curOffset %= 360f;
        }


        var childCount = children.Length;
        var angleStep = 360f / (childCount - 1);
        var curAngle = curOffset;
        foreach (var child in children) {
            if (child == transform) {
                continue;
            }

            var pos = new Vector3(0f, radius, 0f);
            pos = Quaternion.Euler(0, 0, curAngle) * pos;
            child.localPosition = Vector3.Lerp(pos, child.localPosition, smoothMovementFactor);
            if (resetXYAngles) {
                if (resetZAngle) {
                    child.eulerAngles = Vector3.zero;
                } else {
                    child.eulerAngles = new Vector3(0f, 0f, curAngle);
                }
            }
            curAngle += angleStep;
        }
    }
}
