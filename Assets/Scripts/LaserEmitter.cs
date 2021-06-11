using System.Linq;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    [SerializeField] 
    private LineRenderer laserLine;

    private RaycastHit[] _results = new RaycastHit[100];
    private Vector3[] _laserPositions = new Vector3[2];
    private LaserReceiver _currentlyActivating;
    
    private void Update()
    {
        RaycastLaser();
    }

    private void RaycastLaser()
    {
        if (Physics.RaycastNonAlloc(transform.position, transform.forward, _results) > 1)
        {
            var closestHit = _results.OrderBy(hit => hit.distance).First(hit => hit.transform != transform);

            _laserPositions[0] = transform.position;
            _laserPositions[1] = closestHit.point;
            laserLine.SetPositions(_laserPositions);

            if (closestHit.transform.TryGetComponent<LaserReceiver>(out var receiver))
            {
                receiver.SetActive(true);
                _currentlyActivating = receiver;
            }
            else
            {
                if (_currentlyActivating)
                {
                    _currentlyActivating.SetActive(false);
                    _currentlyActivating = null;
                }
            }
        }
    }
}