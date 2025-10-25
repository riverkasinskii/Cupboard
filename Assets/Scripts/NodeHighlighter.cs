using UnityEngine;
using UnityEngine.UI;

public class NodeHighlighter : MonoBehaviour
{
    [SerializeField] 
    private Image highlightImage;

    [SerializeField] 
    private Color allowedColor = new(0f, 1f, 0f, 0.5f);

    [SerializeField] 
    private Color blockedColor = new(1f, 0f, 0f, 0.5f);

    private void Awake()
    {
        if (highlightImage != null)
            highlightImage.enabled = false;
    }

    public void ShowAllowed(float depthScale = 1f)
    {
        if (!highlightImage) 
            return;

        var color = allowedColor; 
        color.a *= Mathf.Clamp01(depthScale);
        highlightImage.color = color;
        highlightImage.enabled = true;
    }

    public void ShowBlocked()
    {
        if (!highlightImage) 
            return;

        highlightImage.color = blockedColor;
        highlightImage.enabled = true;
    }

    public void Hide()
    {
        if (!highlightImage) 
            return;

        highlightImage.enabled = false;
    }
}


