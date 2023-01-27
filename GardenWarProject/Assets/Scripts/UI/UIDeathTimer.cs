using TMPro;
using UnityEngine;

public class UIDeathTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deathTimerText;

    public void UpdateTextTimer(int value)
    {
        gameObject.SetActive(value >= 0);
        deathTimerText.text = $"{value}";
    }
}
