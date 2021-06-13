using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class VerticalTransformer : MonoBehaviour
{
    [SerializeField]
    private float duration = 1f;
    
    [SerializeField]
    private Ease ease;
    
    [SerializeField]
    private float displacement;

    private Tweener _doorAnimation;
    private bool o = false;

    private void Awake()
    {
        float speed = 1 / duration;

        _doorAnimation = transform.DOMoveY(transform.position.y + displacement, speed)
            .SetEase(ease)
            .SetSpeedBased()
            .SetAutoKill(false)
            .Pause();
    }

    public void SetOpen(bool open)
    {
        if (open)
            _doorAnimation.PlayForward();
        
        else _doorAnimation.PlayBackwards();
    }

    [Button]
    private void Open()
    {
        o = !o;
        SetOpen(o);
    }
}
