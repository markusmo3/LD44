using System;
using System.Collections;
using Gamekit2D;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class WaveController : MonoBehaviour {

    public int currentWave = 0;
    public bool waveRunning = true;

    public Vector2 greenSlimeModInfo = new Vector2(0.8f, 1.1f);
    public float curGreenSlimeRate;
    public Vector2 timeModInfo = new Vector2(30, 1.1f);
    public float curTimeLeft;
    public float timeLeft;

    private Damager waveDamager;
    public GameObject waveEndPrefab;
    public ShopController shopController;
    public SmartText waveText;
    public Slider waveSlider;

    private Spawner[] spawners;

    private void Start() {
        waveDamager = gameObject.AddComponent<Damager>();
        waveDamager.damage = 10000;
        waveDamager.knockback = 0;
        waveDamager.hittableLayers = LayerMask.GetMask("Enemy");
        waveDamager.disableLoot = true;
        spawners = FindObjectsOfType<Spawner>();
        shopController.onShopClose.AddListener(BeginWave);

        UpdateValues();
    }

    private IEnumerator EndWave() {
        foreach (var spawner in spawners) {
            spawner.Reset();
            spawner.enabled = false;
        }
        var enemiesLeft = FindObjectsOfType<Enemy>();
        foreach (var e in enemiesLeft) {
            var damageable = e.GetComponent<Damageable>();
            if (damageable) {
                damageable.TakeDamage(waveDamager);
            }
        }
        if (waveEndPrefab) {
            Instantiate(waveEndPrefab);
        }
        yield return new WaitForSeconds(0.5f);
        shopController.SetOpen(true);
    }

    private void BeginWave() {
        var enemiesLeft = FindObjectsOfType<Enemy>();
        foreach (var e in enemiesLeft) {
            Destroy(e);
        }
        waveRunning = true;
        currentWave++;
        UpdateValues();
        foreach (var spawner in spawners) {
            spawner.enabled = true;
        }
    }

    private void Update() {
        if (waveRunning) {
            curTimeLeft -= Time.deltaTime;
            if (curTimeLeft <= 0) {
                StartCoroutine(EndWave());
                waveRunning = false;
            }
            waveText.SetText(currentWave);
            waveSlider.value = 1 - curTimeLeft / timeLeft;
        }
    }

    private void UpdateValues() {
        timeLeft = CalcNewValue(timeModInfo);
        curTimeLeft = timeLeft;
        curGreenSlimeRate = CalcNewValue(greenSlimeModInfo);

        foreach (var spawner in spawners) {
            spawner.SetRateSmart(curGreenSlimeRate);
        }
    }

    private float CalcNewValue(Vector2 modInfo) {
        return modInfo.x * Mathf.Pow(modInfo.y, currentWave);
    }

}