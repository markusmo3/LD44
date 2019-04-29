using UnityEngine;
using UnityEngine.AI;

public static class VectorExtensions {

    public static Vector2 to2D(this Vector3 t) {
        return t;
    }

    public static Vector3 zeroZ(this Vector3 t) {
        t.z = 0;
        return t;
    }

    public static Vector3 inverse(this Vector3 t) {
        return -t;
    }

    public static float randomOfRange(this Vector2 t) {
        return Random.Range(t.x, t.y);
    }

    public static int randomOfRange(this Vector2Int t) {
        return Random.Range(t.x, t.y);
    }

    public static float avg(this Vector2 t) {
        return (t.x + t.y) / 2.0f;
    }

    public static bool RandomPointInNavMesh(this Vector3 center, float range, out Vector3 result) {
        for (int i = 0; i < 30; i++) {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, 1)) {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

}