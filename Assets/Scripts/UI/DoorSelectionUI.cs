using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoorSelectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNumberText;
    [SerializeField] private TextMeshProUGUI playerStatsText;
    [SerializeField] private List<DoorButtonUI> doorButtons;

    public void Show(int room, RuntimePlayerStats player, List<DoorData> doors, Action<DoorData> onDoorSelected)
    {
        gameObject.SetActive(true);
        roomNumberText.text  = $"Room {room}";
        playerStatsText.text = FormatStats(player);

        bool isBoss = doors.Count == 1 && doors[0].roomType == RoomType.Boss;

        for (int i = 0; i < doorButtons.Count; i++)
        {
            if (i < doors.Count)
            {
                doorButtons[i].gameObject.SetActive(true);
                doorButtons[i].Setup(doors[i], onDoorSelected, isBoss);
            }
            else
            {
                doorButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void Hide() => gameObject.SetActive(false);

    private string FormatStats(RuntimePlayerStats p) =>
        $"HP: {p.currentHP:F0}/{p.maxHP:F0}  |  DMG: {p.damage:F1}  |  SPD: {p.attackSpeed:F2}/s  " +
        $"|  Crit: {p.critChance * 100:F0}%  |  Gold: {p.gold}";
}
