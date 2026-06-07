using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MetaProgressionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI     soulsText;
    [SerializeField] private List<UpgradeButtonUI> upgradeButtons;
    [SerializeField] private Button              newRunButton;

    private Action<int> _onBuy;

    public void Show(int souls, UpgradeConfig[] upgrades, int[] levels,
                     Action<int> onBuy, Action onNewRun)
    {
        gameObject.SetActive(true);
        _onBuy = onBuy;
        Refresh(souls, levels);

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < upgrades.Length)
            {
                upgradeButtons[i].gameObject.SetActive(true);
                int captured = i;
                upgradeButtons[i].Setup(upgrades[i], levels[i], () => _onBuy?.Invoke(captured));
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }

        newRunButton.onClick.RemoveAllListeners();
        newRunButton.onClick.AddListener(() => onNewRun?.Invoke());
    }

    public void Refresh(int souls, int[] levels)
    {
        soulsText.text = $"Souls: {souls}";
        for (int i = 0; i < upgradeButtons.Count && i < levels.Length; i++)
            upgradeButtons[i].SetLevel(levels[i]);
    }

    public void Hide() => gameObject.SetActive(false);
}
