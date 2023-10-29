using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startOn : MonoBehaviour
{
    public bool isOn;

    private void Awake()
    {
        if (gameObject.GetComponent<Light>().enabled)
        {
            isOn = true;
        }
        else isOn = false;
    }
}
