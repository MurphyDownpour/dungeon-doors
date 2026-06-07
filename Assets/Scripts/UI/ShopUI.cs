using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private List<ShopItemUI> itemSlots;
    [SerializeField] private Button leaveButton;

    public void Show(RuntimePlayerStats player, List<ShopItemConfig> items,
                     Action<ShopItemConfig> onBuy, Action onLeave)
    {
        gameObject.SetActive(true);
        RefreshGold(player.gold);

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count)
            {
                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].Setup(items[i], onBuy);
            }
            else
            {
                itemSlots[i].gameObject.SetActive(false);
            }
        }

        leaveButton.onClick.RemoveAllListeners();
        leaveButton.onClick.AddListener(() => onLeave?.Invoke());
    }

    public void RefreshGold(int gold) => goldText.text = $"Gold: {gold}";

    public void Hide() => gameObject.SetActive(false);
}
