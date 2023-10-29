using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    GameObject door;
    GameObject doorParent;

    private void Start()
    {
        doorParent = transform.parent.gameObject;
        door = doorParent.GetComponentInChildren<DoorMover>().transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Light>().enabled) //if button light is on (player has turned it on with light steal)
        {
            door.GetComponent<DoorMover>().isActive = true;
        }
        if (!GetComponent<Light>().enabled) //if button light is off
        {
            door.GetComponent<DoorMover>().isActive = false;
        }
    }
}
