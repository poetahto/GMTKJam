using System;
using Assets.Scripts.Slice;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectHighlighter : MonoBehaviour, ISliceCleanup
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    
    [SerializeField] 
    public MeshRenderer meshRenderer;

    [SerializeField] 
    public float duration;

    [SerializeField] 
    public float intensity = 2f;

    private Tweener _highlightAnimation;
    private Color _initialColor;

    private void Start()
    {
        var material = meshRenderer.material;

        _initialColor = material.GetColor(EmissionColor);
        CreateHighlightAnimation(material);
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    private void CreateHighlightAnimation(Material material)
    {
        _highlightAnimation = DOTween.To(
            () => material.GetColor(EmissionColor),
            value => material.SetColor(EmissionColor, value), 
            material.GetColor(EmissionColor) * intensity, 
            1 / duration);

        _highlightAnimation
            .SetSpeedBased()
            .SetAutoKill(false)
            .SetUpdate(true)
            .Pause();
    }

    [UsedImplicitly]
    public void SetHighlighted(bool highlighted)
    {
        if (highlighted)
            _highlightAnimation.PlayForward();
        
        else _highlightAnimation.PlayBackwards();
    }

    public void Cleanup()
    {
        _highlightAnimation.Kill();
        meshRenderer.material.SetColor(EmissionColor, _initialColor);
    }
}