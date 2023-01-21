using Controllers.Inputs;
using LogicUI.FancyTextRendering;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public MarkdownRenderer headerText;
    public MarkdownRenderer descriptionText;
    public LayoutElement layoutElement;
    public uint characterLimit;

    [SerializeField] private RectTransform tr;

    private void Start()
    {
        gameObject.SetActive(false);
    }
    
    public void SetText(string text,string header = "")
    {
        if (string.IsNullOrEmpty(text)) text = "";
        headerText.gameObject.SetActive(header != "");
        headerText.Source = header;
        descriptionText.Source = text;
        
        layoutElement.enabled = (headerText.Source?.Length > characterLimit || descriptionText.Source?.Length > characterLimit);

        var position = PlayerInputController.mousePos;

        position.x /= Screen.width;
        position.y /= Screen.height;

        tr.pivot = position;
        
        transform.position = PlayerInputController.mousePos;
    }
}
