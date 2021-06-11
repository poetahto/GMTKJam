using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightDetector : MonoBehaviour
{
    [SerializeField]
    private WeightCollider collider;
    [SerializeField]
    private WeightCallback callback;

    private List<GameObject> objectsDetected = new List<GameObject>();
    private bool activated = false;
    private float totalWeightDetected = 0;
   
    public float maxWeightThreshold = 0;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(collider.GetWeight());
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("thresh: " + ThresholdAchieved());
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        WeightCollider collided = collidedObject.GetComponent<WeightCollider>();
        if (collided == null || !collided.CanMeasure()) return;
        totalWeightDetected += collided.GetWeight();

        objectsDetected.Add(collidedObject);

        bool activated = totalWeightDetected >= maxWeightThreshold;
        if (activated && callback != null)
        {
            callback.Callback(gameObject, activated, this);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        WeightCollider collided = collidedObject.GetComponent<WeightCollider>();
        if (collided == null || !collided.CanMeasure()) return;
        totalWeightDetected -= collided.GetWeight();

        objectsDetected.Remove(collidedObject);

        bool activated = totalWeightDetected >= maxWeightThreshold;
        if (!activated && callback != null)
        {
            callback.Callback(gameObject, activated, this);
        }
    }

    public float GetWeightDetected() 
    {
        return totalWeightDetected;
    }

    public List<GameObject> GetObjectsDetected()
    {
        return objectsDetected;
    }

    public bool ThresholdAchieved()
    {
        return totalWeightDetected >= maxWeightThreshold;
    }
}
