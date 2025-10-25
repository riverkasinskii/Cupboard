using UnityEngine;

public sealed class ChipHighlighter : MonoBehaviour
{
    private PulsingAnimation pulsing;

    private void Awake()
    {
        pulsing = GetComponent<PulsingAnimation>();
    }

    public void Highlight(bool state)
    {
        pulsing.Highlight(state);
    }
}

