using System;
using UnityEngine;
using UnityEngine.Events;

public class Collider2DNotify : MonoBehaviour {
    [Serializable]
    public class Collider2DEvent : UnityEvent<Collider2D> {
    }

    public bool checkTrigger;

    public Collider2DEvent onEnter;
    public Collider2DEvent onStay;
    public Collider2DEvent onExit;

    private void OnCollisionEnter2D(Collision2D other) {
        onEnter.Invoke(other.collider);
    }

    private void OnCollisionStay2D(Collision2D other) {
        onStay.Invoke(other.collider);
    }

    private void OnCollisionExit2D(Collision2D other) {
        onExit.Invoke(other.collider);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (checkTrigger) {
            onEnter.Invoke(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (checkTrigger) {
            onStay.Invoke(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (checkTrigger) {
            onExit.Invoke(other);
        }
    }
}
