using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightCallback : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Callback(GameObject obj, bool thresholdAchieved, WeightDetector detector)
    {
        Debug.Log("thresh: " + thresholdAchieved);
    }
}
