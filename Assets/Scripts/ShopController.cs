using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopController : MonoBehaviour {

    public UnityEvent onShopOpen;
    public UnityEvent onShopClose;
    public GameObject shopPanel;
    public DeathController deathController;

    private ShopButton[] shopButtons;

    public void Start() {
        shopButtons = shopPanel.GetComponentsInChildren<ShopButton>();
    }

    private void Update() {
        if (Input.GetButton("Menu") && isOpen()) {
            SetOpen(false);
        }
    }

    public void SetOpen(bool b) {
        if (b == isOpen() || shopButtons == null) {
            return;
        }
        shopPanel.SetActive(b);
        Time.timeScale = b ? 0.1f : 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        if (b) {
            deathController.inputEnabled = false;
            onShopOpen.Invoke();
        } else {
            deathController.inputEnabled = true;
            onShopClose.Invoke();
        }
    }

    public bool isOpen() {
        return shopPanel.activeSelf;
    }

    public void OnClick(ShopButton shopButton) {
        if (deathController.minorSouls <= shopButton.curCost) {
            // not enough souls, but continue and kill the player >:D
            deathController.minorSouls = 0;
            SetOpen(false);
        } else {
            deathController.minorSouls -= shopButton.curCost;
        }

        if (shopButton.name.Contains("Ammo")) {
            deathController.reloadAmount.AddModifier(shopButton.statModifier);
        } else if (shopButton.name.Contains("Clip")) {
            deathController.maxAmmoAmount.AddModifier(shopButton.statModifier);
        }

        deathController.UpdateStatText();
    }
}