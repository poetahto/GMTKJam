using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup instructions;
    [SerializeField] private CanvasGroup main;
    [SerializeField] private CanvasGroup titleText;
    [SerializeField] private Image background;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private float colorShiftTime;
    [SerializeField] private LevelButton[] levels;
    [SerializeField] private float introDuration = 1f;
    [SerializeField] private float introHangTime = 1f;
    [SerializeField] private Vector3 postIntroTitleLocation;
    [SerializeField] private float postIntroTitleScale;
    [SerializeField] private float postIntroTransitionDuration;
    [SerializeField] private float levelAppearTime = 0.1f;

    private bool _instructionsOpen;
    
    private IEnumerator Start()
    {
        background.color = startColor;
        background.DOColor(endColor, colorShiftTime).SetLoops(-1, LoopType.Yoyo);
        
        titleText.alpha = 0;
        titleText.DOFade(1, introDuration);
        yield return new WaitForSeconds(introDuration + introHangTime);

        titleText.transform.DOScale(postIntroTitleScale, postIntroTransitionDuration);
        titleText.transform.DOLocalMove(postIntroTitleLocation, postIntroTransitionDuration);
        yield return new WaitForSeconds(postIntroTransitionDuration);
        
        foreach (var levelButton in levels)
        {
            levelButton.Enable();
            yield return new WaitForSeconds(levelAppearTime);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && _instructionsOpen)
        {
            _instructionsOpen = false;
            DOTween.Kill(instructions);
            DOTween.Kill(main);
            instructions.DOFade(0, 0.25f);
            main.DOFade(1, 0.25f);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenInstructions()
    {
        _instructionsOpen = true;
        DOTween.Kill(instructions);
        DOTween.Kill(main);
        instructions.DOFade(1, 0.25f);
        main.DOFade(0, 0.25f);
    }
}