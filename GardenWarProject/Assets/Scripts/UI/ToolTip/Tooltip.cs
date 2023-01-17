using Controllers.Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;
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
        headerText.text = header;
        descriptionText.text = text;
        
        layoutElement.enabled = (headerText.text?.Length > characterLimit || descriptionText.text?.Length > characterLimit);

        var position = PlayerInputController.mousePos;

        position.x /= Screen.width;
        position.y /= Screen.height;

        tr.pivot = position;
        
        transform.position = PlayerInputController.mousePos;
    }
}
