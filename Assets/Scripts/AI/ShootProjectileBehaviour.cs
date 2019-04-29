using System.Collections;
using UnityEngine;

public class ShootProjectileBehaviour : AIMachineBehaviour {
    public GameObject projectilePrefab;
    public bool aimAhead;

    public float projectileSpeed = 10;
//    public float projectileSpawnDistance;

    public override void OnAIStateEnter() {
        controller.StartCoroutine(attack());
    }

    IEnumerator attack() {
        controller.onAttack.Invoke();
        yield return new WaitForSeconds(0.3f);
        var target = controller.GetTarget();
        if (target) {
            var p = Instantiate(projectilePrefab);
            p.transform.position = controller.transform.position;
            var prb2d = p.GetComponent<Rigidbody2D>();
            var aimVec = (target.transform.position.to2D() -
                         controller.transform.position.to2D()).normalized;
            p.transform.up = aimVec;
            prb2d.AddForce(aimVec * projectileSpeed, ForceMode2D.Impulse);
        }
        doContinue();
    }
}