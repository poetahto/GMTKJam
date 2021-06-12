using System;
using Assets.Scripts.Slice;
using DG.Tweening;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    [SerializeField] 
    public Renderer targetRenderer;

    [SerializeField] 
    public float duration;

    [SerializeField] 
    public float targetAlpha;

    private Tweener _fadeAnimation;

    private void Awake()
    {
        CreateFadeAnimation(targetRenderer.material);       
    }

    private void OnDestroy()
    {
        _fadeAnimation.Kill();
    }

    private void CreateFadeAnimation(Material material)
    {
        _fadeAnimation = material
            .DOFade(targetAlpha, 1 / duration)
            .SetSpeedBased()
            .SetAutoKill(false)
            .SetUpdate(true)
            .Pause();
    }

    [UsedImplicitly]
    public void SetFading(bool fading)
    {
        if (fading)
            _fadeAnimation.PlayForward();
        
        else _fadeAnimation.PlayBackwards();
    }
}