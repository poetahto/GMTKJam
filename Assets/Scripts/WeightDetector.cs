using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeightDetector : MonoBehaviour
{
    [Serializable]
    public class ActivationChangeEvent : UnityEvent<bool> {}
    
    [SerializeField]
    private ActivationChangeEvent onActivationChange;

    [SerializeField]
    private float maxWeightThreshold;
    
    private List<GameObject> _objectsDetected = new List<GameObject>();
    public IReadOnlyList<GameObject> ObjectDetected => _objectsDetected;
    
    private bool _isActive;
    public bool ThresholdAchieved => WeightDetected >= maxWeightThreshold;
    public float WeightDetected { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && Vector3.Dot(transform.up, collision.GetContact(0).normal) < 0)
        {
            WeightDetected += collision.rigidbody.mass;
            _objectsDetected.Add(collision.gameObject);
            UpdateActiveState();
        }
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (_objectsDetected.Contains(collision.gameObject))
        {
            WeightDetected -= collision.rigidbody.mass;
            _objectsDetected.Remove(collision.gameObject);
            UpdateActiveState();
        }
    }

    private void UpdateActiveState()
    {
        if (ThresholdAchieved != _isActive)
        {
            onActivationChange.Invoke(ThresholdAchieved);
            _isActive = ThresholdAchieved;
            print("is active: " + _isActive);
        }
    }
}
