using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNumberText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private Slider          playerHPSlider;
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private Slider          enemyHPSlider;
    [SerializeField] private TextMeshProUGUI enemyHPText;
    [SerializeField] private TextMeshProUGUI combatLogText;

    private float _enemyMaxHP;

    public void Show(int room, string enemyName)
    {
        gameObject.SetActive(true);
        roomNumberText.text = $"Room {room}";
        enemyNameText.text  = enemyName;
        combatLogText.text  = string.Empty;
    }

    public void Hide() => gameObject.SetActive(false);

    public void SetPlayerHP(float current, float max)
    {
        playerHPSlider.value = max > 0 ? current / max : 0;
        playerHPText.text    = $"{current:F0} / {max:F0}";
    }

    public void SetEnemyHP(float current, float max)
    {
        _enemyMaxHP          = max;
        enemyHPSlider.value  = max > 0 ? current / max : 0;
        enemyHPText.text     = $"{current:F0} / {max:F0}";
    }

    public void AppendLog(string message)
    {
        combatLogText.text += message + "\n";
        // Keep log from growing unbounded
        const int maxChars = 800;
        if (combatLogText.text.Length > maxChars)
            combatLogText.text = combatLogText.text.Substring(combatLogText.text.Length - maxChars);
    }
}
