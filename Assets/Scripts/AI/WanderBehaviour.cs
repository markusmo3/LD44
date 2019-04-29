using UnityEngine;
using UnityEngine.AI;

public class WanderBehaviour : AIMachineBehaviour {

    public float distance = 1f;
    public float timeout = 3f;
    public float minDistanceToNavMeshEdge = 0.1f;

    public override void OnAIStateEnter() {
        Vector3 randomLocation;
        if (controller.transform.position.zeroZ().RandomPointInNavMesh(distance, out randomLocation)) {
            controller.navAgent.destination = randomLocation;
        } else {
            doContinue();
        }
    }

    public override void OnAIStateUpdate() {
        if (controller.navAgent.remainingDistance <= 0.1f || timeout < 0f) {
            doContinue();
        }
        timeout -= Time.deltaTime;

        if (controller.navAgent.pathStatus != NavMeshPathStatus.PathComplete) {
            Debug.Log("INVALID PATH");
        }
    }

    public Vector3 RandomNavmeshLocation(Vector3 start, float radius) {
//        Vector3 randomDirection = Random.insideUnitSphere * radius;
//        randomDirection += start;
        Vector3 randomDirection = start;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.negativeInfinity;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;
            finalPosition += hit.normal * minDistanceToNavMeshEdge;
            return finalPosition;
        }
        return finalPosition;
    }

}