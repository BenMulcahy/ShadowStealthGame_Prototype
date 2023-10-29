using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMover : MonoBehaviour
{
    public bool isActive = false;
    [SerializeField] Vector3 EndPos; //serialize to be visable in inspector
    Vector3 startPos;

    public float moveSpeed = 5f;

    void Start()
    {
        startPos = transform.localPosition; //set openPos to starting position
    }

    void Update()
    {
        //move to close position - isActive set in LightButton script
        if (isActive)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, EndPos, moveSpeed * Time.deltaTime);
        }

        //move to open position 
        if (!isActive)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPos, moveSpeed * Time.deltaTime); ;
        }
    }
}
