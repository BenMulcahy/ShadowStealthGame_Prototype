using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;


/// <summary>
/// Control navmesh movement for enemyAI
/// </summary>
public class NavMeshMovement : MonoBehaviour
{

    [Header("Pathfinding")]
    public bool drawGizmos = true;
    public bool chasingPlayer = false;
    public float waitAtPoint = 5f;
    public float waitAtLight = 5f;
    public float timeToTurnAtPoint = 5f;
    public float timeToTurn = 5f;
    public float timeAtSearchPoints = 4f;
    public bool hasPath;
    public bool isDistracted;

    public NavMeshAgent enemy;
    int targetIndex;
    Transform currentTarget;
    float distToPoint;

    public Vector3 playerPositionKnown;
    public Transform playerCharacter;
    public Transform[] targets;
    public float targetLocationVariance = 1f; //how close enemy can get to target position before determines its close enough
    public float willingDistanceFromPlayer = 3f;
    public float huntingFOV = 160f;
    public float huntingFOVRad = 12f;
    float originalFOV;
    float originalFOVRad;
    Vector3 targetPos;
    Vector3 lightLocation;
    public bool moveToLight;
    public float dstToLight;
    Quaternion rot;
    Animator anim;


    void Start()
    {
        enemy = GetComponent<NavMeshAgent>(); //get reference to navMeshAgent attached to enemy
        enemy.updateRotation = false;
        targetIndex = 0;
        currentTarget = targets[targetIndex];
        StartCoroutine(UpdatePath());
        StartCoroutine(LookInDirectionOfTravel());
        originalFOV = GetComponent<FieldOfView>().viewAngle;
        originalFOVRad = GetComponent<FieldOfView>().viewRadius;
        enemy.stoppingDistance = 0.5f;
        anim = GetComponentInChildren<Animator>();
    }

    public void Distraction(Vector3 location)
    {
        lightLocation = location;
        isDistracted = true;
    }

    private void Update()
    {
        if (Vector3.Distance(enemy.velocity, Vector3.zero) <= 0)
        {
            anim.SetFloat("speed", 0);
        }

        if (Vector3.Distance(enemy.velocity, Vector3.zero) > 0)
        {
            anim.SetFloat("speed", 1);
        }
       
       
        
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
                //Calculate Distance between target and current position 
                currentTarget = targets[targetIndex];
                distToPoint = Vector3.Distance(currentTarget.position, transform.position); //get distance between character and current path target
            if (!chasingPlayer)
            {
                if (!moveToLight)
                {
                    if (isDistracted)
                    {
                        enemy.isStopped = true; //stop moving
                        var waitForSeconds = new WaitForSecondsInterruptable(waitAtLight); //WaitForSecondsInterruptable is a customYieldInstrcution that can be interupted with bool check
                        waitForSeconds.OnKeepWaiting += i => i.Stop(chasingPlayer); //waitForSeconds unless chasingPlayer is true then interupt wait.
                        yield return waitForSeconds; //return after wait or if chase player set true
                        enemy.isStopped = false; //restart movement
                        isDistracted = false; //no longer distracted
                    }

                    enemy.stoppingDistance = 0.5f;
                    if (distToPoint <= targetLocationVariance) //if character is within area of target point
                    {
                        //Debug.Log("Reached current target");

                        if (targetIndex <= targets.Length)
                        {

                            targetIndex++; //increment target index

                            //Wait at point unless chasing enemy
                            var waitForSeconds = new WaitForSecondsInterruptable(waitAtPoint); //WaitForSecondsInterruptable is a customYieldInstrcution that can be interupted with bool check
                            waitForSeconds.OnKeepWaiting += i => i.Stop(chasingPlayer); //waitForSeconds unless chasingPlayer is true then interupt wait.
                            yield return waitForSeconds; //return after wait or if chase player set true
                        }

                        if (targetIndex >= targets.Length)
                        {
                            //Debug.Log("End of path");
                            Array.Reverse(targets);
                            targetIndex = 1;
                        }

                    }
                    enemy.destination = currentTarget.position;
                }
                else if (moveToLight)
                {
                    float tmpDst = Vector3.Distance(transform.position, lightLocation);
                    enemy.stoppingDistance = 0.8f;
                    enemy.destination = lightLocation;
                    if(enemy.remainingDistance <= enemy.stoppingDistance)
                    {
                        moveToLight = false;
                        Debug.Log("Reached light");
                        //Wait at light unless chasing enemy
                        var waitForSeconds = new WaitForSecondsInterruptable(waitAtLight); //WaitForSecondsInterruptable is a customYieldInstrcution that can be interupted with bool check
                        waitForSeconds.OnKeepWaiting += i => i.Stop(chasingPlayer); //waitForSeconds unless chasingPlayer is true then interupt wait.
                        yield return waitForSeconds; //return after wait or if chase player set true
                        isDistracted = false;
                        Debug.Log("returning to point");

                    }
                }
                
            }

            //If chasing player set current target to be players position, chasing player set in FOV script
            else if (chasingPlayer)
                {
                    enemy.stoppingDistance = willingDistanceFromPlayer;
                    if (GetComponent<FieldOfView>().detectedPlayer)
                    {
                        //chase player 
                        enemy.destination = playerCharacter.position;
                        playerPositionKnown = playerCharacter.position;
                        float dst = Vector3.Distance(transform.position, playerCharacter.position);

                        //adjust enemy fov
                        GetComponent<FieldOfView>().viewAngle = huntingFOV;
                        GetComponent<FieldOfView>().viewRadius = huntingFOVRad;

                        //attack player
                        if (dst <= GetComponentInChildren<WeaponBehaviour>().weaponRange)
                        {
                            StartCoroutine(GetComponent<AttackBehaviour>().prepAttack());
                        }
                        else StopCoroutine(GetComponent<AttackBehaviour>().prepAttack());
                    }
                    if (!GetComponent<FieldOfView>().detectedPlayer)
                    {
                        //readjust enemy fov
                        GetComponent<FieldOfView>().viewAngle = originalFOV;
                        GetComponent<FieldOfView>().viewRadius = originalFOVRad;

                        //go to last known location for player and wait before returning to path
                        enemy.destination = playerPositionKnown;
                        var waitForSeconds = new WaitForSecondsInterruptable(timeAtSearchPoints); //WaitForSecondsInterruptable is a customYieldInstrcution that can be interupted with bool check
                        waitForSeconds.OnKeepWaiting += i => i.Stop(GetComponent<FieldOfView>().detectedPlayer); //waitForSeconds unless chasingPlayer is true then interupt wait.
                        yield return waitForSeconds; //return after wait or if chase player set true
                    }
                }
                
                yield return new WaitForSeconds(0.25f); //wait for .25 of a second before looping - reduce calls as not calling each frame
            

        }
    }


    /// <summary>
    /// Look in direction of travel
    /// </summary>
    /// <returns></returns>
    IEnumerator LookInDirectionOfTravel()
    {
        while (true)
        {
            
            if (!chasingPlayer)
            {
                targetPos = currentTarget.position;

                //look in direction given by target point
                if (distToPoint <= targetLocationVariance) //if enemy has reached target
                {
                    if (transform.forward != currentTarget.forward) //if enemy is not facing direction of current target
                    {
                        transform.forward = Vector3.Lerp(transform.forward, currentTarget.forward, timeToTurnAtPoint * Time.deltaTime); //lerp towards direction of current target
                        yield return new WaitForEndOfFrame();
                    }
                }

                //look in direction of travel
                else if(!enemy.velocity.Equals(Vector3.zero)) //if enemy is not standing still
                {
                    Vector3 dir = (enemy.pathEndPosition - transform.position).normalized; //calculate direction of travel 
                    Quaternion heading = Quaternion.FromToRotation(transform.forward, dir); //calculate heading along dir of travel
                    transform.rotation *= heading; //rotate enemy to face direction of travel
                }

            }

            //look at player
            if (chasingPlayer)
            {
                
                targetPos = playerPositionKnown; //set targetposition as known player pos
                Vector3 dir = (targetPos - transform.position).normalized; //calculate vector direction to known player pos
                Quaternion heading = Quaternion.FromToRotation(transform.forward, dir); //calculate rotation between transform dir and direction to player pos known
                transform.rotation *= heading; //set rotation to heading

            }

            //look at distraction
            if (isDistracted)
            {
                targetPos = lightLocation;
                Vector3 dir = (lightLocation - transform.position).normalized; //calculate direction to light
                transform.forward = dir; //look in direction of light
            }
            

            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDrawGizmos()
    {
        Transform prev = null;
        if (drawGizmos)
        {
            Gizmos.color = Color.green;
            foreach (Transform target in targets)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(target.position, targetLocationVariance/2);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(target.position, target.forward);
                if(prev == null)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(transform.position, target.position);
                }                
                if (prev != null)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(target.position, prev.position);
                }
                prev = target;
                
            }


        }
        
    }

}
