using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for all player abilities
/// </summary>
public class PlayerAbilities : MonoBehaviour
{
    public Transform playerCamera;
    public Camera cam;
    public Transform orbLocator;
    Animator anim;
    [Space(8)]

    [Header("Dash Variables")]
    public GameObject dashIndicator;
    public float dashRange = 20f;
    public float dashPadding = 0.9f;
    public float dashCooldown = 3f;
    public float TimeToDash = 5f;
    public float increaseFOV = 1.3f;
    [Space(8)]
    [Header("Attack Variables")]
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public float attackDamage;
    [Space(5)]
    public float stealthKillRange = 5f;
    public float stealthKillCooldown = 1f;
    public float timeToStealthKill = 3f;
    public float maxStealthKillAngle = 90f;
    [HideInInspector]
    public float stealthKillTimer = 0f;
    GameObject currentTarget;

    public LayerMask targetLayer;
    [Space(8)]
    [Header("Light Steal Variables")]
    public float lightStealRange = 5f;
    public float lightStealCooldown = 2f;
    public LayerMask lightLayer;
    public int lightCharges = 0;
    public int maxLightCharges = 1;
    public float lightZoom = 3f;
    public GameObject lightStolenMesh;
    public LayerMask obsticalMask;

    [HideInInspector]
    public bool hasDashed = false;

    [HideInInspector]
    public float attackCooldownReset;
    [HideInInspector]
    public float dashCooldownReset;
    [HideInInspector]
    public float lightCooldownReset;
    float initialFOV;
    


    GameObject spawnedIndicator;
    [HideInInspector]
    public bool hasAttacked;
    [HideInInspector]
    public bool hasStolenLight;
    [HideInInspector]
    public bool hasStealthKilled;
    GameObject stolenSphere = null;

    RaycastHit hit;
    RaycastHit kill;
    RaycastHit lightHit;
    bool chokeSFX;

    void Start()
    {
        dashCooldownReset = dashCooldown;
        attackCooldownReset = attackCooldown;
        lightCooldownReset = lightStealCooldown;
        initialFOV = cam.fieldOfView;
        hasDashed = false;
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
       //Dash Cooldown
       if(hasDashed == true)
        {
            dashCooldownReset -= Time.deltaTime;
            if(dashCooldownReset <= 0f)
            {
                dashCooldownReset = dashCooldown;
                hasDashed = false;
                Debug.Log("Dash Ready");
                
            }
         
        }

       //attzck Cooldown
       if(hasAttacked == true)
        {
            attackCooldownReset -= Time.deltaTime;
            if(attackCooldownReset <= 0f)
            {
                attackCooldownReset = attackCooldown;
                hasAttacked = false;
                Debug.Log("attack Ready");
            }
        }

       //Light Steal Cooldown
       if(hasStolenLight == true)
        {
            lightCooldownReset -= Time.deltaTime;
            if (lightCooldownReset <= 0f)
            {
                lightCooldownReset = lightStealCooldown;
                hasStolenLight = false;
                //Debug.Log("steal light ready");
            }
        }


    }

    //Dash
    public GameObject PrepDash()
    {
        //spawn dash location indicator
        if (spawnedIndicator == null)
        {
            
            spawnedIndicator = Instantiate(dashIndicator, gameObject.transform.forward, Quaternion.identity); //create dash indicator mesh 
            
        }

        Vector3 startPosition = transform.position; //get player position
        Vector3 dashDirection = transform.forward; //get player direction
        if(Physics.Raycast(startPosition, dashDirection, out hit, dashRange)) //cast ray from player out along player direction checking for hit within dashRange
        {
            spawnedIndicator.transform.position = hit.point + (hit.normal * dashPadding); //set dash idicator to be at raycast hit pos
            return hit.collider.gameObject; //on hit return the gameobject of what hit
        }
        else
        {
         
            spawnedIndicator.transform.position = transform.position + transform.forward * dashRange; //set dash idicator to be at player look within dash range
            return null; //without hit return null
            
        }

        

    }

    public IEnumerator Dash()
    {
        //Destroy dash location idicator

          

        if (PrepDash() != null) //if object hit is not null 
        {
            Vector3 posStart = transform.position; //Stores current player position
            Vector3 posTarget = hit.point + (hit.normal * dashPadding); //stores raycast hit position plus padding value to ensure player doesnt clip into object
            Vector3 posCam = playerCamera.transform.localPosition;
            float t = 0f; //set t for lerp

            while (t <= 1) //whilst lerp t is less than 0. t will equal 1 when player reaches target pos
            {
                t += Time.deltaTime / TimeToDash; //increase t by time/how long it takes to dash
                transform.position = Vector3.Lerp(posStart, posTarget, t); //lerp player position to target position
                playerCamera.transform.localPosition = Vector3.Lerp(posCam,new Vector3(posCam.x, (posCam.y * t), posCam.z), t); //lerps camera down and back to start position to create swoop effect
                cam.fieldOfView = Mathf.Lerp(initialFOV, (initialFOV * increaseFOV), t); //Increase camera FOV for gamefeel 
                yield return new WaitForEndOfFrame(); //wait for end of frame before looping
            }

            playerCamera.transform.localPosition = posCam; //Resets camera position to account for lerp inaccuracy 
            cam.fieldOfView = initialFOV; //reset FOV

        }
        else
        {
            Vector3 posStart = transform.position; //Stores current player position
            Vector3 posTarget = transform.position + transform.forward*dashRange;
            Vector3 posCam = playerCamera.transform.localPosition;
            
            float t = 0f; //set t for lerp

            while (t <= 1) //whilst lerp t is less than 0. t will equal 1 when player reaches target pos
            {
                t += Time.deltaTime / TimeToDash; //increase t by time/how long it takes to dash
                transform.position = Vector3.Lerp(posStart, posTarget, t); //lerp player position to target position
                playerCamera.transform.localPosition = Vector3.Lerp(posCam, new Vector3(posCam.x, (posCam.y * t), posCam.z), t); //lerps camera down and back to start position to create swoop effect
                cam.fieldOfView = Mathf.Lerp(initialFOV, (initialFOV * increaseFOV), t);//Increase camera FOV for gamefeel 
                yield return new WaitForEndOfFrame(); //wait for end of frame before looping
            }

            
            playerCamera.transform.localPosition = posCam; //Resets camera position to account for lerp inaccuracy 
            cam.fieldOfView = initialFOV; //reset FOV
               


        }
        Destroy(spawnedIndicator);
        hasDashed = true;

    }
    
    //Attack
    public void Attack()
    {
        if(Physics.Raycast(transform.position, transform.forward, out kill, attackRange, targetLayer)) //Cast ray from player for kill using targetLayer mask to ensure only enemys/destructable objects can be hit
        {
            if (kill.transform.gameObject.GetComponent<HP>() != null)
            {
                kill.transform.gameObject.GetComponent<HP>().Damage(attackDamage);
            }
            else Debug.LogError("No HP componentent found");
            hasAttacked = true;
        }

    }

    //Stealth Kill
    public void StealthKillCheck()
    {
        if (Physics.Raycast(transform.position, transform.forward, out kill, stealthKillRange, targetLayer))
        {
            if(kill.transform.gameObject.GetComponent<HP>() != null && kill.transform.gameObject.GetComponent<HP>().isStealthKillable)
            {
                float angleDif;
                angleDif = Quaternion.Angle(kill.transform.rotation, transform.rotation); //get the difference between players angle and the enemy angle
                if (angleDif < maxStealthKillAngle) //if player is within the stealth kill angle i.e. behind the enemy then stealth kill
                {
                    currentTarget = kill.transform.gameObject;
                    StealthKill();
                }
                
            }
            else Debug.LogError("No HP componentent found OR not stealth killable");
        }
    }

    /// <summary>
    /// Hold enemy in place while player stealth kills - stealth kill takes a set time before "killing".
    /// </summary>
    void StealthKill()
    {
        if (stealthKillTimer <= timeToStealthKill)
        {
            
            if(chokeSFX != true)
            {
                FindObjectOfType<AudioManager>().Play("Choke");
                chokeSFX = true;
            }
            
            currentTarget.transform.rotation = transform.rotation; //force enemy to look in same direction as player
            currentTarget.GetComponent<NavMeshMovement>().enemy.isStopped = true; //stop enemy navmesh
            stealthKillTimer += Time.deltaTime; //increase timer for stealth kill
        }
        else if (!currentTarget.GetComponent<HP>().canRespawn)
        {
            Destroy(currentTarget); //destroy target when timer  = time to stealth kill
            FindObjectOfType<AudioManager>().Stop("Choke");
        }
        else if (currentTarget.GetComponent<HP>().canRespawn) currentTarget.GetComponent<HP>().Respawn();
    }

    /// <summary>
    /// Reset enemy if player cancels stealth kill
    /// </summary>
    public void StealthKillReset()
    {
        stealthKillTimer = 0f; //set stealth kill timer to 0
        if(currentTarget != null) //if there is a game object in current target
        {
            currentTarget.GetComponent<NavMeshMovement>().enemy.isStopped = false; //Restart enemy navmesh
            currentTarget.GetComponent<FieldOfView>().detection = currentTarget.GetComponent<FieldOfView>().chaseThreshold; //set enemy detection to chase threshold
            currentTarget.GetComponent<NavMeshMovement>().playerPositionKnown = transform.position; //tell enemy player position
            currentTarget.GetComponent<NavMeshMovement>().chasingPlayer = true; //begin chasing player
            currentTarget = null; //set current target back to null
        }
    }

    //Light Steal
    public GameObject LightSteal()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out lightHit, lightStealRange, lightLayer)) //Cast ray from player for lightsteal using light layer mask to ensure only objects on layer light can be hit
        {
            
            return lightHit.collider.gameObject; //return the gameobject of the cast hit
        }
        else return null;
    }

    public IEnumerator StolenLight()
    {
        
        if (LightSteal() != null)
        {
            anim.SetTrigger("Steal");
            FindObjectOfType<AudioManager>().Play("Energy");
            Light light = LightSteal().GetComponent<Light>(); //get light component from gameobject hit with light raycast
            hasStolenLight = true;
            //Disable Light
            if (light.enabled)
            {

                //Distract Enemy

                if (light.GetComponent<LightDistractionRadius>() != null)
                {
                    //create new list transforms and populate with enemies nearby to trigger distraction
                    List<Transform> visableEnemies = new List<Transform>();
                    Collider[] nearbyEnemies = Physics.OverlapSphere(light.transform.position, light.GetComponent<LightDistractionRadius>().distractionRad, targetLayer); //Create array of all enemies around light
                    if(nearbyEnemies.Length > 0)
                    {
                        for (int i = 0; i < nearbyEnemies.Length; i++) //loop through all nearby enemies
                        {
                            Transform enemy = nearbyEnemies[i].transform; //get enemy transform
                            Vector3 dirToEnemy = (enemy.position - light.transform.position); //dir from light to enemy
                            float dstToEnemy = Vector3.Distance(enemy.position, light.transform.position); //dst from light to enemy 
                            if (!Physics.Raycast(light.transform.position, dirToEnemy, dstToEnemy, obsticalMask)) //cast ray from light to enemy, if ray does not hit anything then add enemy to list
                            {
                                visableEnemies.Add(enemy.parent); //store list of enemies distracted by light
                            }
                        }

                        visableEnemies[0].GetComponent<NavMeshMovement>().dstToLight = Vector3.Distance(light.transform.position, visableEnemies[0].position);
                        float closestDst = visableEnemies[0].GetComponent<NavMeshMovement>().dstToLight;
                        int closestEnemy = 0;

                        //loop through list of visible enemies and work out which is closest to light source
                        for (int i = 0; i < visableEnemies.Count; i++)
                        {
                            visableEnemies[i].GetComponent<NavMeshMovement>().dstToLight = Vector3.Distance(light.transform.position, visableEnemies[i].position);
                            if (visableEnemies[i].GetComponent<NavMeshMovement>().dstToLight < closestDst)
                            {
                                closestDst = visableEnemies[i].GetComponent<NavMeshMovement>().dstToLight;
                                closestEnemy = i;
                            }
                        }

                        //loop through all enemies in visable enemies and distract, moving the closest enemy towards the light
                        foreach (Transform enemy in visableEnemies)
                        {
                            Debug.Log(visableEnemies.IndexOf(enemy) + " " + enemy.parent.name);
                            Debug.Log(closestEnemy);
                            if (visableEnemies.IndexOf(enemy) == closestEnemy)
                            {
                                enemy.GetComponent<NavMeshMovement>().Distraction(light.transform.position);
                                enemy.GetComponent<NavMeshMovement>().moveToLight = true;
                            }
                            else
                            {
                                enemy.GetComponent<NavMeshMovement>().Distraction(light.transform.position);
                            }
                        }

                    }
                    

                }
                //if the player is holding less light charges than the maximum 
                if (lightCharges < maxLightCharges)
                {
                    light.enabled = false; //turn off light
                    light.GetComponent<startOn>().isOn = false;

                    lightCharges++; //increase light charges
                    if (stolenSphere == null) //if there is not a stolenSphere mesh
                    {
                        stolenSphere = Instantiate(lightStolenMesh, light.transform.position, orbLocator.rotation, playerCamera); //create stolen sphere at light with player camera as parent
                        float distToHand = Vector3.Distance(stolenSphere.transform.position, orbLocator.position); //get distance between sphere and the players hand
                        
                        //While distance is less than 0.05 (usinging dist to account for inaccuracys with lerp) - move sphere to player hand
                        while (distToHand >= 0.05f)
                        {
                            stolenSphere.transform.position = Vector3.Lerp(stolenSphere.transform.position, orbLocator.position, lightZoom * Time.deltaTime); //lerp sphere towards player hand  
                            distToHand = Vector3.Distance(stolenSphere.transform.position, orbLocator.position); //update distance between hand and sphere
                            yield return new WaitForEndOfFrame(); //wait until end of frame before looping
                        }
                        stolenSphere.transform.position = orbLocator.position; //set sphere location to hand orb location - again accounting for lerp innaccuracies
                        yield break; //break from if statement, skipping next if check
                       
                    }

                    //if stolenSphere does exsist then spawn temp sphere to move towards hand then destory the temp sphere
                    if(stolenSphere != null)
                    {
                        GameObject stolenSphereTemp; //create temp gameobject variable to store temp sphere
                        stolenSphereTemp = Instantiate(lightStolenMesh, light.transform.position, light.transform.rotation, playerCamera); //create sphere as in previous but assign to temp sphere game object
                        //Same as previous if statement using temp sphere instead
                        float distToHandTemp = Vector3.Distance(stolenSphereTemp.transform.position, orbLocator.position);
                        while (distToHandTemp >= 0.05f)
                        {
                            stolenSphereTemp.transform.position = Vector3.Lerp(stolenSphereTemp.transform.position, orbLocator.position, lightZoom * Time.deltaTime);
                            distToHandTemp = Vector3.Distance(stolenSphereTemp.transform.position, orbLocator.position);
                            yield return new WaitForEndOfFrame();
                        }
                        Destroy(stolenSphereTemp); //destroy temp sphere

                        yield break;
                    }

                }

            }


            //Enable light
            if (!light.enabled)
            {
                //Distract Enemy

                if (light.GetComponent<LightDistractionRadius>() != null)
                {
                    //create new list transforms and populate with enemies nearby to trigger distraction
                    List<Transform> visableEnemies = new List<Transform>();
                    Collider[] nearbyEnemies = Physics.OverlapSphere(light.transform.position, light.GetComponent<LightDistractionRadius>().distractionRad, targetLayer); //Create array of all enemies around light
                    if (nearbyEnemies.Length > 0)
                    {
                        for (int i = 0; i < nearbyEnemies.Length; i++) //loop through all nearby enemies
                        {
                            Transform enemy = nearbyEnemies[i].transform; //get enemy transform
                            Vector3 dirToEnemy = (enemy.position - light.transform.position); //dir from light to enemy
                            float dstToEnemy = Vector3.Distance(enemy.position, light.transform.position); //dst from light to enemy 
                            if (!Physics.Raycast(light.transform.position, dirToEnemy, dstToEnemy, obsticalMask)) //cast ray from light to enemy, if ray does not hit anything then add enemy to list
                            {
                                visableEnemies.Add(enemy.parent); //store list of enemies distracted by light
                            }
                        }

                        visableEnemies[0].GetComponent<NavMeshMovement>().dstToLight = Vector3.Distance(light.transform.position, visableEnemies[0].position);
                        float closestDst = visableEnemies[0].GetComponent<NavMeshMovement>().dstToLight;
                        int closestEnemy = 0;

                        //loop through list of visible enemies and work out which is closest to light source
                        for (int i = 0; i < visableEnemies.Count; i++)
                        {
                            visableEnemies[i].GetComponent<NavMeshMovement>().dstToLight = Vector3.Distance(light.transform.position, visableEnemies[i].position);
                            if (visableEnemies[i].GetComponent<NavMeshMovement>().dstToLight < closestDst)
                            {
                                closestDst = visableEnemies[i].GetComponent<NavMeshMovement>().dstToLight;
                                closestEnemy = i;
                            }
                        }

                        //loop through all enemies in visable enemies and distract, moving the closest enemy towards the light
                        foreach (Transform enemy in visableEnemies)
                        {
                            Debug.Log(visableEnemies.IndexOf(enemy) + " " + enemy.parent.name);
                            Debug.Log(closestEnemy);
                            if (visableEnemies.IndexOf(enemy) == closestEnemy)
                            {
                                enemy.GetComponent<NavMeshMovement>().Distraction(light.transform.position);
                                enemy.GetComponent<NavMeshMovement>().moveToLight = true;
                            }
                            else
                            {
                                enemy.GetComponent<NavMeshMovement>().Distraction(light.transform.position);
                            }
                        }

                    }

                }
                //If player has any light charges in hand
                if (lightCharges > 0)
                {
                    //ensure stolenSphere (gameObject displaying stolen light) is not null
                    if (stolenSphere != null)
                    {
                        //If the player has only one light charge then this needs to destroy the original stolenSphere
                        if (lightCharges == 1)
                        {
                            stolenSphere.transform.parent = light.transform; //set stolenSphere parent to light.transfrom to prevent issues if player moves while orb lerps.
                            float distToLight = Vector3.Distance(stolenSphere.transform.position, light.transform.position); //get dist between sphere and light
                            
                            //while distToLight is less than 0.05 lerp towards light
                            while (distToLight > 0.05f)
                            {
                                stolenSphere.transform.position = Vector3.Lerp(stolenSphere.transform.position, light.transform.position, lightZoom * Time.deltaTime); //lerp towards light
                                distToLight = Vector3.Distance(stolenSphere.transform.position, light.transform.position); //update dist between sphere and light
                                yield return new WaitForEndOfFrame(); //wait until end of the frame before looping
                            }
                            Destroy(stolenSphere); //destroy StolenSphere
                            light.enabled = true; //switch light on 
                            light.GetComponent<startOn>().isOn = true;
                            lightCharges--; //decrement light charges
                            yield break; //break from if, skipping the following if statement.

                        }

                        //if the player has more than one light charge then create a duplicate sphere and lerp that towards light before destroying
                        if (lightCharges >= 2)
                        {
                            GameObject tmpStolenSphere = Instantiate(lightStolenMesh, orbLocator.transform.position, orbLocator.transform.rotation, light.gameObject.transform); //create duplicate sphere with light as parent
                            float distToLight = Vector3.Distance(tmpStolenSphere.transform.position, light.transform.position); //get distance between temp sphere and light
                            
                            //same as while loop in prev if statement
                            while (distToLight > 0.05f)
                            {
                                tmpStolenSphere.transform.position = Vector3.Lerp(tmpStolenSphere.transform.position, light.transform.position, lightZoom * Time.deltaTime);
                                distToLight = Vector3.Distance(tmpStolenSphere.transform.position, light.transform.position);
                                yield return new WaitForEndOfFrame();
                            }
                            Destroy(tmpStolenSphere); //destroy temp sphere
                            light.enabled = true; //switch on light
                            light.GetComponent<startOn>().isOn = true;
                            lightCharges--; //decrement light charges
                            yield break; //break from if
                        }
                     }
                }
            }
            
            yield return new WaitForEndOfFrame(); //wait until end of frame before returning

        }
        

    }

}
