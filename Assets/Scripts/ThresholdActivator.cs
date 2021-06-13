using UnityEngine;
using UnityEngine.Events;

public class ThresholdActivator : MonoBehaviour
{
    [SerializeField] 
    private UnityEvent onActivate = new UnityEvent();

    [SerializeField]
    private float threshold = 1f;

    private float _amount;
    public float Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            
            if (value >= threshold)
                onActivate.Invoke();
        }
    }
    
    public void Add(float amount)
    {
        Amount += amount;
    }

    public void Remove(float amount)
    {
        Amount -= amount;
    }

    public void BoolChange(bool shouldAdd)
    {
        Amount += shouldAdd ? 1 : -1;
    }
}