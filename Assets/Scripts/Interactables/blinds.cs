using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Open and close roller door blinds on windows 
/// </summary>
public class blinds : MonoBehaviour
{
    public bool isActive = false;
    [SerializeField] Vector3 closePos; //serialize to be visable in inspector
    Vector3 openPos;

    public float moveSpeed = 5f;

    void Start()
    {
        openPos = transform.position; //set openPos to starting position
    }

    void Update()
    {
        //move to close position - isActive set in LightButton script
        if (isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, closePos, moveSpeed * Time.deltaTime);
        }

        //move to open position 
        if (!isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPos, moveSpeed * Time.deltaTime); 
        }
    }
}
