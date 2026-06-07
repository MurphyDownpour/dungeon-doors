using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button          buyButton;

    public void Setup(ShopItemConfig item, Action<ShopItemConfig> onBuy)
    {
        nameText.text        = item.itemName;
        descriptionText.text = item.description;
        costText.text        = $"{item.cost} Gold";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy?.Invoke(item));
    }
}
