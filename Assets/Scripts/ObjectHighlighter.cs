using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    
    [SerializeField] 
    private MeshRenderer meshRenderer;

    [SerializeField] 
    private float duration;

    [SerializeField] 
    private float intensity = 2f;

    private Tweener _highlightAnimation;

    private void Awake()
    {
        CreateHighlightAnimation(meshRenderer.material);
    }

    private void OnDestroy()
    {
        _highlightAnimation.Kill();
    }

    private void CreateHighlightAnimation(Material material)
    {
        _highlightAnimation = DOTween.To(
            () => material.GetColor(EmissionColor),
            value => material.SetColor(EmissionColor, value), 
            material.color * intensity, 
            1 / duration);

        _highlightAnimation
            .SetSpeedBased()
            .SetAutoKill(false)
            .Pause();
    }

    [UsedImplicitly]
    public void SetHighlighted(bool highlighted)
    {
        if (highlighted)
            _highlightAnimation.PlayForward();
        
        else _highlightAnimation.PlayBackwards();
    }
}