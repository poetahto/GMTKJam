using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    public bool test = false;
    public float time = 1f;
    public Ease ease;
    public float defaultVertical;

    private Vector3 initialPosition;

    private bool open;

    // Start is called before the first frame update
    void Start()
    {
        this.open = false;
        initialPosition = gameObject.transform.position;
        defaultVertical = gameObject.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (test && !isOpen())
        {
            OpenDoor();
        } else if (!test && isOpen())
        {
            CloseDoor();
        }
    }

    public void OpenDoor() 
    {
        if (isOpen()) return;
        UpdateOpen(true);
        Vector3 newPosition = initialPosition - new Vector3(0, defaultVertical, 0);
        gameObject.transform
            .DOMove(newPosition, time)
            .SetEase(ease)
            .OnComplete(fullyUpdated);
    }

    public void CloseDoor()
    {
        if (!isOpen()) return;
        UpdateOpen(false);
        gameObject.transform
            .DOMove(initialPosition, time)
            .SetEase(ease)
            .OnComplete(fullyUpdated);
    }

    private void UpdateOpen(bool open) 
    {
        this.open = open;
        //events
    }

    private void fullyUpdated()
    {
        //more events
    }

    public bool isOpen()
    {
        return open;
    }
}
