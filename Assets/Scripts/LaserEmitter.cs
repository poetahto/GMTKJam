using UnityEngine;

[ExecuteAlways]
public class LaserEmitter : MonoBehaviour
{
    [SerializeField] 
    private LineRenderer laserLine;

    [SerializeField] 
    private ParticleSystem hitParticles;
    
    private int _resultsLength;
    private RaycastHit[] _results = new RaycastHit[100];
    private Vector3[] _laserPositions = new Vector3[2];
    private LaserReceiver _currentlyActivating;
    private Ray LaserRay => new Ray { direction = transform.forward, origin = transform.position };

    private void Start()
    {
        hitParticles.Play();
    }

    private void Update()
    {
        RaycastLaser();
    }

    private void RaycastLaser()
    {
        _resultsLength = Physics.RaycastNonAlloc(LaserRay, _results);
        
        if (_resultsLength > 0)
        {
            var closestHit = GetClosestHit();
            PlayHitParticles(closestHit);
            UpdateLineRenderer(closestHit.point);
            CheckForReceiverActivation(closestHit.transform.gameObject);
        }
    }

    private RaycastHit GetClosestHit()
    {
        var closestHit = _results[0];
            
        for (int i = 0; i < _resultsLength; i++)
        {
            if (_results[i].distance < closestHit.distance)
                closestHit = _results[i];
        }

        return closestHit;
    }

    private void PlayHitParticles(RaycastHit closestHit)
    {
        var particleTransform = hitParticles.transform;
        
        particleTransform.position = closestHit.point;
        particleTransform.forward = closestHit.normal;
    }
    
    private void UpdateLineRenderer(params Vector3[] points)
    {
        _laserPositions = new Vector3[points.Length + 1];
        _laserPositions[0] = transform.position;

        for (int i = 1; i < _laserPositions.Length; i++)
            _laserPositions[i] = points[i - 1];

        laserLine.SetPositions(_laserPositions);
    }

    private void CheckForReceiverActivation(GameObject target)
    {
        if (target.TryGetComponent<LaserReceiver>(out var receiver))
        {
            receiver.SetActive(true);
            _currentlyActivating = receiver;
        }
        else if (_currentlyActivating)
        {
            _currentlyActivating.SetActive(false);
            _currentlyActivating = null;
        }
    }
}