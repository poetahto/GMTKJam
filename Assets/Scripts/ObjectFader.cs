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

    private Tweener[] _fadeSequences = new Tweener[0];

    private void Awake()
    {
        CreateFadeAnimation();    
    }

    private void OnDestroy()
    {
        foreach (var fadeSequence in _fadeSequences)
            fadeSequence.Kill();    
    }

    private void CreateFadeAnimation()
    {
        _fadeSequences = new Tweener[targetRenderers.Length];
        
        for (int i = 0; i < _fadeSequences.Length; i++)
        {
            _fadeSequences[i] = targetRenderers[i].material
                    .DOFade(targetAlpha, 1 / duration)
                    .SetSpeedBased()
                    .SetUpdate(true)
                    .SetAutoKill(false)
                    .Pause();
        }
    }

    [UsedImplicitly]
    public void SetFading(bool fading)
    {
        if (fading)
        {
            foreach (var fadeSequence in _fadeSequences)
                fadeSequence.PlayForward();
        }
        
        else foreach (var fadeSequence in _fadeSequences)
            fadeSequence.PlayBackwards();
    }
}