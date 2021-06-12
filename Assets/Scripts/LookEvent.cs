using System;
using UnityEngine;
using UnityEngine.Events;

public class LookEvent : MonoBehaviour
{
    [SerializeField] 
    public float lookStrength = 0.75f;

    [SerializeField] 
    public UnityEvent<bool> onLookChanged = new UnityEvent<bool>();

    private bool _isLookedAt;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        CompareDirections();
    }

    public float readonlySim = 0f;
    
    private void CompareDirections()
    {
        var cameraTransform = _camera.transform;
        Vector3 cameraDirection = cameraTransform.forward;
        Vector3 directionToCamera = (transform.position - cameraTransform.position).normalized;

        float similarity = Vector3.Dot(cameraDirection, directionToCamera);
        readonlySim = similarity;
        bool isLookedAt = similarity >= lookStrength;

        if (isLookedAt != _isLookedAt)
        {
            _isLookedAt = isLookedAt;
            print(isLookedAt + " " + gameObject.name);
            onLookChanged.Invoke(isLookedAt);
        }
    }
}