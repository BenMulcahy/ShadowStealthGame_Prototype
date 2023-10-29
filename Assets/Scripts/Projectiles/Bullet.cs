using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bullet class for projectile bullet
/// </summary>
public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 1.0f;
    public Vector3 spawnLoc;
    public float dstFromSpawn;
    float weaponRange;
    float weaponDamage;

    private void Awake()
    {
        spawnLoc = transform.position; //store spawn location of bullet
        
    }

    //Set range and damage for bullet - passed from attack behaviour
    public void SetDamageAndRange(float range, float dmg)
    {
        weaponRange = range;
        weaponDamage = dmg;
    }

    void Update()
    {
        Vector3 prevlocation = transform.position; //store current bullet location before updating location
        transform.position += transform.forward * bulletSpeed * Time.deltaTime; //move bullet at travel speed
        dstFromSpawn = Vector3.Distance(transform.position, spawnLoc); //check distance from bullet to spawn location
        if(dstFromSpawn >= weaponRange) //if bullet has travelled further than weapon range - destroy bullet 
        {
            Destroy(gameObject);
        }

        //hit detection using ray cast to ensure detection is not missed on frame update
        RaycastHit hit;
        Ray bulletDetection = new Ray(prevlocation, transform.forward);
        if (Physics.Raycast(bulletDetection, out hit, Vector3.Distance(prevlocation, transform.position))) //cast ray from bullets location prior to move to the current location of bullet.
        {
            if(hit.transform.gameObject != null) //if hit something
            {
              if(hit.transform.gameObject.GetComponent<HP>() != null) //if object hit has HP component
                {
                    hit.transform.gameObject.GetComponent<HP>().Damage(weaponDamage); //damage object hit
                    Destroy(gameObject);
                }
                else Destroy(gameObject); //if object doesnt have HP component destory bullet
            }
        }
    }
}