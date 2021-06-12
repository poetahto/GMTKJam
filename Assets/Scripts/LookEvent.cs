using System;
using UnityEngine;
using UnityEngine.Events;

public class LookEvent : MonoBehaviour
{
    [SerializeField] 
    public Renderer targetRenderer;
    
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

    private void CompareDirections()
    {
        var cameraTransform = _camera.transform;
        Vector3 cameraDirection = cameraTransform.forward;
        Vector3 directionToCamera = (targetRenderer.bounds.center - cameraTransform.position).normalized;

        float similarity = Vector3.Dot(cameraDirection, directionToCamera);
        bool isLookedAt = similarity >= lookStrength;

        if (isLookedAt != _isLookedAt)
        {
            _isLookedAt = isLookedAt;
            onLookChanged.Invoke(isLookedAt);
        }
    }
}