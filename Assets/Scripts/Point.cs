using UnityEngine;

public sealed class Point : MonoBehaviour
{
    [field: SerializeField]
    public PointName Name { get; private set; } = PointName.FirstPoint;

    [field: SerializeField]
    public RectTransform RectTransform { get; private set; }

    [SerializeField]
    private PulsingAnimation _animation;
    
    private void Start()
    {
        _animation.Highlight(true);
    }
}
