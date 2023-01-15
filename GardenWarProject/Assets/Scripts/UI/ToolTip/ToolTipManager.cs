using Controllers.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolTipManager : MonoBehaviour
{
    private static ToolTipManager Instance;
    [SerializeField] private Tooltip tooltip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;
    }
    
    public static void Show(string text,string header = "")
    {
        Instance.tooltip.gameObject.SetActive(true);
        Instance.tooltip.SetText(text,header);
    }

    public static void Hide()
    {
        Instance.tooltip.gameObject.SetActive(false);
    }
}
