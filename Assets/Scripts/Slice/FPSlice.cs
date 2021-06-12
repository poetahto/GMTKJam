using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Slice
{
    public class FPSlice : MonoBehaviour
    {
        [SerializeField]
        private Camera controllerCamera;

        [SerializeField]
        [Tooltip("The amount of force applied to each side of a slice")]
        private float _forceAppliedToCut = 3f;

        private GameObject _victim;
        private Vector3 _initialHead;
        private Vector3 _base;


        // Start is called before the first frame update
        void Start()
        {
            _initialHead = Vector3.zero;
            _base = Vector3.zero;
            _victim = null;
        }

        // Update is called once per frame
        void Update()
        {
            float defaultDistance = 6;
            if (!Input.GetMouseButton(1))
            {
                if (_initialHead == Vector3.zero || _victim == null) return;
                Debug.Log("cutting...");
                Vector3 finalPos = controllerCamera.transform.position * defaultDistance;

                //Create a triangle between the tip and base so that we can get the normal
                Vector3 side1 = finalPos - _base;
                Vector3 side2 = finalPos - _initialHead;

                //Get the point perpendicular to the triangle above which is the normal
                //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
                Vector3 normal = Vector3.Cross(side1, side2).normalized;

                //Transform the normal so that it is aligned with the object we are slicing's transform.
                Vector3 transformedNormal = ((Vector3)(_victim.transform.localToWorldMatrix.transpose * normal)).normalized;

                //Get the enter position relative to the object we're cutting's local transform
                Vector3 transformedStartingPoint = _victim.transform.InverseTransformPoint(_base);

                Plane plane = new Plane();

                plane.SetNormalAndPosition(
                        transformedNormal,
                        transformedStartingPoint);

                var direction = Vector3.Dot(Vector3.up, transformedNormal);

                //Flip the plane so that we always know which side the positive mesh is on
                if (direction < 0)
                {
                    plane = plane.flipped;
                }

                GameObject[] slices = Slicer.Slice(plane, _victim.gameObject);
                Destroy(_victim.gameObject);

                Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
                Vector3 newNormal = transformedNormal + Vector3.up * _forceAppliedToCut;
                rigidbody.AddForce(newNormal, ForceMode.Impulse);

                _initialHead = Vector3.zero;
                _victim = null;
                _base = Vector3.zero;
                return;
            }

            if (_initialHead == Vector3.zero)
            {
                Debug.Log("found vectors");
                _initialHead = controllerCamera.transform.position;
                _base = _initialHead * defaultDistance;
            }

            if (_victim == null)
            {
                if (Physics.Raycast(_initialHead, transform.TransformDirection(Vector3.forward), out var hit))
                {
                    if (hit.transform.gameObject.GetComponent<Sliceable>() != null)
                    {
                        Debug.Log("found victim");
                        _victim = hit.transform.gameObject;
                    }
                }
            }
        }
    }
}