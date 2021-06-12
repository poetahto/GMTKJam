using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    [SerializeField] 
    private UnityEvent<bool> onActivated = new UnityEvent<bool>();

    private bool _active;
    
    public void SetActive(bool active)
    {
        if (_active != active)
        {
            _active = active;
            onActivated.Invoke(active);
        }
    }
}