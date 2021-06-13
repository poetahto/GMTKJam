using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private bool enableOnAwake;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private CanvasGroup levelStartFade;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float hoverTime = 0.25f;
    [Scene, SerializeField] private string targetScene;

    private Tweener _hoverAnimation;
    
    private void Awake()
    {
        group.interactable = false;
        group.blocksRaycasts = false;
        group.alpha = 0f;

        _hoverAnimation = transform.DOScale(hoverScale, 1 / hoverTime)
            .SetSpeedBased()
            .SetUpdate(true)
            .SetAutoKill(false)
            .Pause();
        
        if (enableOnAwake)
            Enable();
    }

    private void OnDestroy()
    {
        _hoverAnimation.Kill();
    }

    public void Enable()
    {
        group.interactable = true;
        group.blocksRaycasts = true;
        group.DOFade(1, fadeTime);
    }

    public async void OnClick()
    {
        levelStartFade.interactable = true;
        levelStartFade.blocksRaycasts = true;
        await levelStartFade.DOFade(1, 0.5f).AsyncWaitForCompletion();

        DOTween.KillAll();
        SceneManager.LoadScene(targetScene);
    }

    public void OnEnter()
    {
        _hoverAnimation.PlayForward();
    }

    public void OnExit()
    {
        _hoverAnimation.PlayBackwards();
    }
}