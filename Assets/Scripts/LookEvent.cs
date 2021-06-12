using System;
using UnityEngine;
using UnityEngine.Events;

public class LookEvent : MonoBehaviour
{
    [SerializeField] 
    private float lookStrength = 0.75f;

    [SerializeField] 
    private UnityEvent<bool> onLookChanged;

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

    private void CompareDirections()
    {
        var cameraTransform = _camera.transform;
        Vector3 cameraDirection = cameraTransform.forward;
        Vector3 directionToCamera = (transform.position - cameraTransform.position).normalized;

        float similarity = Vector3.Dot(cameraDirection, directionToCamera);
        bool isLookedAt = similarity >= lookStrength;

        if (isLookedAt != _isLookedAt)
        {
            _isLookedAt = isLookedAt;
            onLookChanged.Invoke(isLookedAt);
        }
    }
}