using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomsClearedText;
    [SerializeField] private TextMeshProUGUI goldEarnedText;
    [SerializeField] private TextMeshProUGUI soulsEarnedText;
    [SerializeField] private Button          metaProgressionButton;
    [SerializeField] private Button          newRunButton;

    public void Show(int roomsCleared, int goldEarned, int soulsEarned,
                     Action onMetaProgression, Action onNewRun)
    {
        gameObject.SetActive(true);
        roomsClearedText.text = $"Rooms Cleared: {roomsCleared}";
        goldEarnedText.text   = $"Gold Earned: {goldEarned}";
        soulsEarnedText.text  = $"Souls Earned: {soulsEarned}";

        metaProgressionButton.onClick.RemoveAllListeners();
        metaProgressionButton.onClick.AddListener(() => onMetaProgression?.Invoke());

        newRunButton.onClick.RemoveAllListeners();
        newRunButton.onClick.AddListener(() => onNewRun?.Invoke());
    }

    public void Hide() => gameObject.SetActive(false);
}
