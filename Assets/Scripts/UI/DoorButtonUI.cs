using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoorButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomTypeText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button          button;

    public void Setup(DoorData door, Action<DoorData> onSelected, bool isBossRoom)
    {
        roomTypeText.text = isBossRoom ? "⚠ BOSS" : RoomTypeLabel(door.roomType);
        rewardText.text   = RewardLabel(door.reward);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelected?.Invoke(door));
    }

    private static string RoomTypeLabel(RoomType t) => t switch
    {
        RoomType.Normal => "Normal Room",
        RoomType.Elite  => "Elite Room",
        RoomType.Shop   => "Shop",
        RoomType.Boss   => "BOSS",
        _               => t.ToString(),
    };

    private static string RewardLabel(RewardType r) => r switch
    {
        RewardType.Damage      => "+15% Damage",
        RewardType.AttackSpeed => "+15% Attack Speed",
        RewardType.MaxHP       => "+20% Max HP",
        RewardType.RestoreHP   => "Restore 20% HP",
        RewardType.GoldGain    => "+25% Gold Gain",
        RewardType.CritChance  => "+5% Crit Chance",
        _                      => r.ToString(),
    };
}
