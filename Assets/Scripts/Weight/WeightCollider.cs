using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightCollider : MonoBehaviour
{
    private bool measurement = false;
    private float weight;

    void Awake()
    {
        Rigidbody body = gameObject.GetComponent<Rigidbody>();
        if (body != null)
        {
            this.measurement = true;
            this.weight = body.mass;
            //Debug.Log("Body: " + measurement + " " + weight);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanMeasure()
    {
        return measurement;
    }

    public float GetWeight()
    {
        return weight;
    }
}
