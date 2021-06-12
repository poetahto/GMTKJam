using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.MeshOperations;

public class WeightDetector : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<bool> onActivationChange;

    [SerializeField]
    private float maxWeightThreshold;

    [SerializeField] 
    private float debounceSeconds = 0.25f;
    
    private List<GameObject> _objectsDetected = new List<GameObject>();
    private Dictionary<GameObject, Coroutine> _debouncingObjects = new Dictionary<GameObject, Coroutine>();

    private bool _isActive;
    private bool ThresholdAchieved => WeightDetected >= maxWeightThreshold;
    private float WeightDetected { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        if (ObjectIsDebouncing(collision.gameObject))
        {
            FinishDebounce(collision.gameObject);
        }
        else if (ShouldAddWeigher(collision))
        {
            AddWeigher(collision.rigidbody);
        }
    }
    
    private bool ObjectIsDebouncing(GameObject obj)
    {
        return _debouncingObjects.ContainsKey(obj);
    }

    private void FinishDebounce(GameObject obj)
    {
        StopCoroutine(_debouncingObjects[obj]);
        _debouncingObjects.Remove(obj);
    }

    private static bool ShouldAddWeigher(Collision collision)
    {
        return collision.rigidbody != null;
    }

    private void AddWeigher(Rigidbody rb)
    {
        WeightDetected += rb.mass;
        _objectsDetected.Add(rb.gameObject);
        UpdateActiveState();
    }
    
    private void OnCollisionExit(Collision collision)
    {
        if (ShouldRemoveWeigher(collision.gameObject))
            StartDebounce(collision);
    }

    private bool ShouldRemoveWeigher(GameObject obj)
    {
        return _objectsDetected.Contains(obj);
    }

    private void StartDebounce(Collision collision)
    {
        var removingCoroutine = StartCoroutine(WaitRemoveWeigher(collision.rigidbody));
        _debouncingObjects.Add(collision.gameObject, removingCoroutine);
    }

    private IEnumerator WaitRemoveWeigher(Rigidbody rb)
    {
        yield return new WaitForSeconds(debounceSeconds);
        _debouncingObjects.Remove(rb.gameObject);
        RemoveWeigher(rb);
    }

    private void RemoveWeigher(Rigidbody rb)
    {
        WeightDetected -= rb.mass;
        _objectsDetected.Remove(rb.gameObject);
        UpdateActiveState();
    }

    private void UpdateActiveState()
    {
        if (ThresholdAchieved != _isActive)
        {
            onActivationChange.Invoke(ThresholdAchieved);
            _isActive = ThresholdAchieved;
        }
    }
}
