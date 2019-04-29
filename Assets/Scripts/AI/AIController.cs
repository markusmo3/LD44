using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AIController : MonoBehaviour {
    private int playerLayer;
    private int otherThanEnemyLayerMask;

    public GameObject root;
    [ShowOnly]
    public NavMeshAgent navAgent;
    [ShowOnly]
    public Rigidbody2D rb2d;

    public UnityEvent onAttack;

    public Animator animator;
    private Dictionary<string, GameObject> goParameters = new Dictionary<string, GameObject>();

    private void Start() {
        playerLayer = LayerMask.NameToLayer("Player");
        otherThanEnemyLayerMask = LayerMask.GetMask("Default", "Player", "Enemy");
        animator = GetComponent<Animator>();
        var aiBehaviours = animator.GetBehaviours<StateMachineBehaviour>();
        foreach (var aiBehaviour in aiBehaviours) {
            var ai = aiBehaviour as AIMachineBehaviour;
            if (ai) {
                ai.init(animator, this);
            }
        }

        navAgent = root.GetComponentInChildren<NavMeshAgent>();
        rb2d = root.GetComponentInChildren<Rigidbody2D>();
        SetTarget(null);
    }

    private void Update() {
        var target = GetTarget();
        if (target) {
            var aimVec = (target.transform.position - transform.position).to2D();
            var sqrMagnitude = aimVec.sqrMagnitude;
            animator.SetFloat("distanceToTarget", sqrMagnitude);

            Debug.DrawRay(transform.position, aimVec, Color.magenta);
            var raycastHits = Physics2D.RaycastAll(transform.position, aimVec.normalized,
                5f, otherThanEnemyLayerMask);
            var hitPlayer = false;
            if (raycastHits != null) {
                foreach (var hit in raycastHits) {
                    if (hit.collider.gameObject == root) {
                        continue;
                    }
                    hitPlayer = hit.collider.gameObject.layer == playerLayer;
                    break;
                }
            }
            animator.SetBool("lineOfSight", hitPlayer);
        }
    }

    public void SetGameObject(string key, GameObject value) {
        goParameters[key] = value;
    }

    public GameObject GetGameObject(string key) {
        return goParameters[key];
    }

    public void SetTarget(GameObject value) {
        goParameters["__Target"] = value;
        animator.SetBool("hasTarget", value != null);
        if (value == null) {
            animator.SetFloat("distanceToTarget", float.PositiveInfinity);
            animator.SetBool("lineOfSight", false);
        }
    }

    public GameObject GetTarget() {
        return goParameters["__Target"];
    }

}
