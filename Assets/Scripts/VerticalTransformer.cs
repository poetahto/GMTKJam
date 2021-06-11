using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class VerticalTransformer : MonoBehaviour
{
    public float time = 1f;
    public Ease ease;
    public float defaultVertical;

    private Vector3 initialPosition;

    private bool open;

    // Start is called before the first frame update
    void Start()
    {
        open = false;
        initialPosition = gameObject.transform.position;
        defaultVertical = gameObject.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Button]
    private void TestDoor()
    {
        if (IsOpen())
        {
            CloseDoor();
        } else
        {
            OpenDoor();
        }
    }

    public void OpenDoor() 
    {
        if (IsOpen()) return;
        UpdateOpen(true);
        Vector3 newPosition = initialPosition - new Vector3(0, defaultVertical, 0);
        gameObject.transform
            .DOMove(newPosition, time)
            .SetEase(ease)
            .OnComplete(FullyUpdated);
    }

    public void CloseDoor()
    {
        if (!IsOpen()) return;
        UpdateOpen(false);
        gameObject.transform
            .DOMove(initialPosition, time)
            .SetEase(ease)
            .OnComplete(FullyUpdated);
    }

    private void UpdateOpen(bool update) 
    {
        this.open = update;
        //events
    }

    private void FullyUpdated()
    {
        //more events
    }

    public bool IsOpen()
    {
        return open;
    }
}
