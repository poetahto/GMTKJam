using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryBubble : MonoBehaviour
{
    [Scene, SerializeField] private string mainMenu;
    [SerializeField] private CanvasGroup background;
    [SerializeField] private float animationDuration;
    
    private void Awake()
    {
        transform.DOScale(1.5f, animationDuration).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<Controller>().enabled = false;
            background.alpha = 0.1f;
            background.DOFade(1, 0.5f).OnComplete(() =>
            {
                DOTween.KillAll();
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(mainMenu);
            });
        }
    }
}
