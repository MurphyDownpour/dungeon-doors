using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button          buyButton;

    public void Setup(UpgradeConfig upgrade, int currentLevel, Action onBuy)
    {
        nameText.text        = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
        SetLevel(currentLevel);
        costText.text        = $"{upgrade.costPerLevel} Soul";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy?.Invoke());
    }

    public void SetLevel(int level) => levelText.text = $"Level {level}";
}
