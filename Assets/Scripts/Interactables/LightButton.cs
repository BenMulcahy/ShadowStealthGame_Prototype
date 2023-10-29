using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Button for window blinds, requires light energy to operate
/// </summary>
public class LightButton : MonoBehaviour
{
    public blinds[] blinds;

    void Update()
    {
        if (GetComponent<Light>().enabled) //if button light is on (player has turned it on with light steal)
        {
            foreach(blinds blind in blinds) //loop through each blinds object in blinds array
            {
                blind.isActive = true; //set each blinds object active to on
            }
        }
        if (!GetComponent<Light>().enabled) //if button light is off
        {
            foreach (blinds blind in blinds) //loop through each blinds object
            {
                blind.isActive = false; //set active to off
            }
        }

    }
}
