using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class PulsingAnimation : MonoBehaviour
{    
    [SerializeField]
    private float minAlpha = 0.9f;

    [SerializeField]
    private float maxAlpha = 1.0f;

    [SerializeField] 
    private float pulseSpeed = 5f;

    private bool isHighlighted = false;
    private Coroutine pulseRoutine;
    private Color originalColor;
    private Image image;

    private void Awake()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }            

        originalColor = image.color;
    }

    public void Highlight(bool enable)
    {
        isHighlighted = enable;

        if (enable)
        {
            pulseRoutine ??= StartCoroutine(Pulse());
        }
        else
        {
            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }
            image.color = originalColor;
        }
    }

    private IEnumerator Pulse()
    {        
        Color currentState = originalColor;

        while (isHighlighted)
        {            
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            currentState.a = alpha;
            image.color = currentState;
            yield return null;
        }
    }
}

