using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentOpen : MonoBehaviour
{

    public PlayerController player;

    // Update is called once per frame
    void Update()
    {
        if (player.hasKey) //if button light is on (player has turned it on with light steal)
        {
            GetComponent<DoorMover>().isActive = true;
        }
        if (!player.hasKey) //if button light is off
        {
            GetComponent<DoorMover>().isActive = false;
        }
    }
}
