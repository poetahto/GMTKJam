using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    [SerializeField] 
    public Renderer[] targetRenderers;

    [SerializeField] 
    public float duration;

    [SerializeField] 
    public float targetAlpha;

    private Sequence _fadeSequence;

    private void Awake()
    {
        CreateFadeAnimation();    
    }

    private void OnDestroy()
    {
        _fadeSequence.Kill();
    }

    private void CreateFadeAnimation()
    {
        _fadeSequence = DOTween.Sequence();

        foreach (var targetRenderer in targetRenderers)
        {
            _fadeSequence.Join(targetRenderer.material
                .DOFade(targetAlpha, 1 / duration)
                .SetSpeedBased()
                .SetAutoKill(false)
                .SetUpdate(true)
                .Pause());
        }
    }

    [UsedImplicitly]
    public void SetFading(bool fading)
    {
        if (fading)
            _fadeSequence.PlayForward();
        
        else _fadeSequence.PlayBackwards();
    }
}