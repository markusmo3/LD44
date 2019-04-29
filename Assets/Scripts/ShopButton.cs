using System;
using Kryz.CharacterStats;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour {

    public int cur;
    public int max = 10;
    public int cost = 1;
    public float costMod = 2f;
    public int curCost;
    public StatModifier statModifier;
    public TextMeshProUGUI buttonText;

    private ShopController shopController;
    private Button button;
    private string startText;

    private void Start() {
        shopController = GetComponentInParent<ShopController>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        calcCost();
        UpdateText();
    }

    private void calcCost() {
        curCost = Mathf.FloorToInt(cost + cost * Mathf.Pow(costMod, cur));
    }

    public void UpdateText() {
        buttonText.text = string.Format(getStartText(), cur, max,
            cur >= max ? "???" : curCost.ToString());
    }

    public void OnClick() {
        cur++;
        shopController.OnClick(this);
        if (cur >= max) {
            button.interactable = false;
            curCost = int.MaxValue;
        } else {
            calcCost();
        }
        UpdateText();
    }

    public string getStartText() {
        if (startText == null) {
            startText = buttonText.text;
        }
        return startText;
    }

}