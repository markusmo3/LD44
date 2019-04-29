using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    public AIController ai;
    public Animator characterAnimator;

    private Rigidbody2D rb2d;
    private NavMeshAgent agent;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        var agentNextPosition = agent.nextPosition.to2D();
        var moving = rb2d.position != agentNextPosition;
        if (moving) {
            rb2d.position = agentNextPosition;
        }
        characterAnimator.SetBool("running", moving);
    }

    public void OnTEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            // player in feel radius, ATTACK!!!!111elf
            ai.SetTarget(other.gameObject);
        }
    }

    public void OnTExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            // player in feel radius, ATTACK!!!!111elf
            ai.SetTarget(null);
        }
    }

    public void OnDie() {
        characterAnimator.SetTrigger("death");
        ai.gameObject.SetActive(false);
        rb2d.velocity = Vector2.zero;
        rb2d.simulated = false;
        List<Collider2D> colliders = new List<Collider2D>();
        rb2d.GetAttachedColliders(colliders);
        foreach (var c in colliders) {
            c.enabled = false;
        }
    }
}