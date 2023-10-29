using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightManager : MonoBehaviour
{
    public Collider playerCollider;
    public LayerMask lightsLayer;
    Vector3 castDistance;
    [SerializeField] List<Light> lights;

    private void Awake()
    {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().GetComponent<Collider>();
    }

    private void Start()
    {
        castDistance = GetComponent<BoxCollider>().size;
        populateLightList();
    }

    void populateLightList()
    {
        lights = new List<Light>(); //create a list for all lights
        Collider[] maybeLights = Physics.OverlapBox(transform.position, castDistance/2, Quaternion.identity, lightsLayer);

        for (int i = 0; i< maybeLights.Length; i++)
        {
            lights.Add(maybeLights[i].GetComponent<Light>());
        }

        //loop through all lights and disable them
        foreach (Light light in lights)
        {
            if (light.enabled)
            {
                light.enabled = false;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == playerCollider)
        {
            //loop through all lights and enable them
            foreach(Light light in lights)
            {
                if (!light.enabled && light.GetComponent<startOn>().isOn)
                {
                    light.enabled = true;
                }
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other == playerCollider)
        {
            //loop through all lights and disable them
            foreach (Light light in lights)
            {
                if (light.enabled)
                {
                    light.enabled = false;
                }
            }
        }
    }


}
