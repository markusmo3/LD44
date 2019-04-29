using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {

    public Vector2 timeRange;
    public Vector2 rangeRange;
    public Vector2Int amountRange;
    public GameObject prefab;

    public bool shouldSpawn = true;

    private float nextSpawn;

    private void Update() {
        if (!shouldSpawn) {
            shouldSpawn = Time.time > nextSpawn;
        }
        if (shouldSpawn) {
            var amount = amountRange.randomOfRange();
            Vector3 randomLocation;
            for (var i = 0; i < amount; i++) {
                var radius = rangeRange.randomOfRange();
                if (transform.position.zeroZ().RandomPointInNavMesh(radius, out randomLocation)) {
                    var go = Instantiate(prefab);
                    go.transform.position = randomLocation;
                }
            }
            shouldSpawn = false;
            nextSpawn = Time.time + timeRange.randomOfRange();
        }
    }

    public void Reset() {
        shouldSpawn = true;
        nextSpawn = 0;
    }

    public void SetRateSmart(float rate, float fuzzyness = 1.0f) {
        var avgAmount = timeRange.avg() * rate;
        amountRange = new Vector2Int(
            Mathf.FloorToInt(avgAmount - fuzzyness),
            Mathf.CeilToInt(avgAmount + fuzzyness));
    }
}