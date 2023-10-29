using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Enemy FOV class, controls detection of player - Heavily based off code by Sebastian Lague: https://www.youtube.com/watch?v=rQG9aUWarwE&t=191s
/// </summary>
public class FieldOfView : MonoBehaviour
{

    [Header ("FOV setting")]
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    [Space(5)]
    
    [Header("FOV Masking")]
    public LayerMask obsticalMask;
    public LayerMask player;
    public Transform headDetector;
    public Transform playerBody;
    [HideInInspector]
    public List<Transform> visableTargets = new List<Transform>(); 
    [Space(5)]
    [Header("Detection Level")]
    public float detection;
    public float detectionRatePerSec = 1f;
    public float headDetectionCoeff = 0.5f;
    public float bodyDetectionCoeff = 1f;
    public float detectionResetTime = 3f;
    public float detectionReductionRate = 1f;
    public int chaseThreshold = 8;
    float detectionResetTimer;
    public bool detectedPlayer;
    [Range(0, 50)]
    public float detectedSpreadRad = 5f;
    public LayerMask allies;

    [Space(5)]
    [Header("FOV visual")]
    public float meshResolution;
    public MeshFilter viewMeshFilter;
    public int edgeResIterations = 5;
    public float edgeDistThreshhold;
    Mesh viewMesh;
    public GameObject UI;
    GameObject tmpUI = null;
    public Vector3 uiOffset;


    
    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "fov";
        viewMeshFilter.mesh = viewMesh;
        detectionResetTimer = detectionResetTime;
        StartCoroutine("FindTargets", .2f);
        StartCoroutine("AdjustDetection", detectionRatePerSec);
    }

    private void Update()
    {
        //when player detection hits chase threshold player is detected and nearby enemies are alearted
        if(detection >= chaseThreshold)
        {
            List<Transform> nearbyVisbleAllies = new List<Transform>(); //create list for nearby visable enemies
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, detectedSpreadRad, allies); //get array of any enemies within detection spread radius


            //loop through each nearby enemy and cast ray to see if they are visable to enemy
            for (int i = 0; i < nearbyEnemies.Length; i++) 
            {
                Transform allie = nearbyEnemies[i].transform;
                Vector3 dirToAllie = (allie.position - transform.position);
                float dstToAllie = Vector3.Distance(allie.position, transform.position);
                if (!Physics.Raycast(transform.position, dirToAllie, dstToAllie, obsticalMask)) //if ray doesn't hit an obstical
                {
                    nearbyVisbleAllies.Add(allie.parent); //add nearby enemy to list of nearby allies
                }
            }

            //loop through each visable allie and set them to chase player
            foreach(Transform allie in nearbyVisbleAllies)
            {
                allie.GetComponent<FieldOfView>().detection = allie.GetComponent<FieldOfView>().chaseThreshold;
                allie.GetComponent<NavMeshMovement>().playerPositionKnown = playerBody.position;
                allie.GetComponent<NavMeshMovement>().chasingPlayer = true;
            }

            //chase player in Navmesh script
            GetComponent<NavMeshMovement>().chasingPlayer = true;
        }

        //check for any any visable targets - if none then set detected player to false
        if (visableTargets.Count == 0)
        {
            detectedPlayer = false;
        }

        
        //check for any visable targets and the headdetector is visable if yes set detected player to true
        if (visableTargets.Count > 0 && visableTargets.Contains(headDetector))
        {
            detectedPlayer = true;
    
            //if tmpUI is null (detection meter has not been created) instantatate the dectection UI
            if(tmpUI == null)
            {
                tmpUI = Instantiate(UI, transform.position + uiOffset, transform.rotation, transform);
            }
            GetComponent<NavMeshMovement>().playerPositionKnown = playerBody.position;
        }

        //after set delay set detection back to 0 
        if (detectedPlayer == false && detection > 0f)
        {
            Animator anim;
            anim = GetComponentInChildren<Animator>();
            anim.SetBool("Shoot", false);
            detectionResetTimer -= Time.deltaTime;
            float tmpDetection = detection;
            detection = Mathf.Lerp(tmpDetection, 0, Time.deltaTime * 0.5f);
            if (detectionResetTimer <= 0.1f)
            {
                detectionResetTimer = detectionResetTime;
                detection = 0; //reset detection
                Destroy(tmpUI); //destroy detection ui
                GetComponent<NavMeshMovement>().chasingPlayer = false; //enemy no longer chasing after player

            }
        }
    }

    //similar to update only happens last
    private void LateUpdate()
    {
        DrawFOV();
    }


    /// <summary>
    /// increase detection per detection rate
    /// </summary>
    /// <param name="detectionRate"></param>
    /// <returns></returns>
    IEnumerator AdjustDetection(float detectionRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionRate);
            IncreaseDectectionValue();
        }
    }

    /// <summary>
    /// find any visable targets per delay
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator FindTargets(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    /// <summary>
    /// Increase detection value if player visable
    /// </summary>
    void IncreaseDectectionValue()
    {
        if (visableTargets.Contains(headDetector) == true && (visableTargets.Contains(playerBody) == false)) //if visable targets contains the player head
        {
            detection += 1 * headDetectionCoeff;
            
        }
        
        if (visableTargets.Contains(headDetector) == true && (visableTargets.Contains(playerBody) == true)) //if body and head visable
        {
            detection += 1 * bodyDetectionCoeff;
            
        }
        
        if(visableTargets.Contains(headDetector) == false && (visableTargets.Contains(playerBody) == true)) //ensures player cant be spotted if only body visable
        {
  
            return;
        }
      
    }


    /// <summary>
    /// populate visableTargets array with collisions within sphere around enemy
    /// </summary>
    void FindVisibleTargets()
    {
        visableTargets.Clear(); //ensure visble targets is empty
        Collider[] targetsInViewRad = Physics.OverlapSphere(transform.position, viewRadius, player); //Create collider array for any targets within sphere cast

        //cast ray to each collider in TargetsInViewRad to check no obstrutions from obsticles
        for (int i = 0; i<targetsInViewRad.Length; i++) //loop through collider array
        {
            Transform target = targetsInViewRad[i].transform; //get target transform
            Vector3 dirToTarget = (target.position - transform.position); //get direction to target
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) //if direction to target is within view angle
            {
                float distToTarget = Vector3.Distance(transform.position, target.position); //float for distance to target
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obsticalMask)) //cast ray to target checking for collisions in case of walls
                {
                    visableTargets.Add(target); //add any applicable targets to visableTargets
                }
            }
        }
    }

    /// <summary>
    /// Create mesh to draw FOV - Taken from Sebastian Lague https://www.youtube.com/watch?v=73Dc5JTCmKI
    /// </summary>
    void DrawFOV()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution); //stepcount = how many rays to draw from player (mesh resolution per degree)
        float stepAngleSize = viewAngle / stepCount; //calculate how many degrees within each step
        List<Vector3> viewPoints = new List<Vector3>(); //list of all points hit for building FOV mesh
        viewCastInfo oldViewCast = new viewCastInfo(); //previous viewCast

            //loop through each step and cast ray for each
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i; //set current angle from left most angle through to right most angle
                viewCastInfo newViewCast = viewCast(angle); //cast ray for each angle

                //if the loop has looped once 
                if (i > 0)
                {
                
                bool thresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistThreshhold; //check if edge distance threshold is exceeded
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && thresholdExceeded)) //if the old view cast and new view cast are not the same or if both old view cast and new view cast hit and is outside the distance threshold for edge detection 
                {
                    //Detect edge of obstical
                    edgeInfo edge = findEdge(oldViewCast, newViewCast);
                        if(edge.pointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointA); 
                        }
                        if (edge.pointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointB);
                        }
                }
                }
            viewPoints.Add(newViewCast.point); //add points to viewPoints
            oldViewCast = newViewCast; //update old view cast
            //Debug.DrawLine(transform.position, transform.position + dirFromAngle(angle, true) * viewRadius, Color.red);
        }
        
            
            //
        int vertexCount = viewPoints.Count + 1; //Work out total vertecies for FOV mesh (number of viewPoints + origin point (enemy position)
        Vector3[] vertices = new Vector3[vertexCount]; //create array of vertices from vertexCount
        int[] triangles = new int[(vertexCount - 2) * 3]; //calculate number of triangles in mesh using "Triangles = Verticies - 2" = Length of triangles array is equal to (Verticies - 2) * 3 as 3 vertices in each triangle (V-2)

        vertices[0] = Vector3.zero; //start vertex for FOV mesh is enemy position (vector zero)
            
            //loop through vertexcount ignoring the first vertex as this is point 0
            for(int i = 0; i< vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]); //set vertex point in local space
                
                //if i is less than vertexCount - 2 (ensure triangles is only caluated for each triangle in mesh)
                if(i < vertexCount - 2)
                {
                    triangles[i * 3] = 0; //first vertex of triangle
                    triangles[i * 3 + 1] = i + 1; //second vertex of triangle
                    triangles[i * 3 + 2] = i + 2; //third vertex of triangle
                }
            }

        viewMesh.Clear(); //clear mesh
        viewMesh.vertices = vertices; //define each vertex of mesh
        viewMesh.triangles = triangles; //define each triangle of mesh
        viewMesh.RecalculateNormals(); //calculate mesh
    }

    /// <summary>
    /// Function for casting ray using viewCast struct info
    /// </summary>
    /// <param name="globalAngle"></param>
    /// <returns></returns>
    viewCastInfo viewCast (float globalAngle)
    {
        Vector3 dir = dirFromAngle(globalAngle, true);
        RaycastHit hit;

        //check if raycast hits obstical 
        if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obsticalMask))
        {
            return new viewCastInfo(true, hit.point, hit.distance, globalAngle); //if hit return true with the point of hit, distance and angle
        }
        else
        {
            return new viewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle); //if not hit return false, the point at the edge of the radius, the radius of the FOV and the angle
        }
    }

    /// <summary>
    /// Binary search algorithm to find edge of obsticals - gets ray that hits obsitcal and ray that misses obstical and casts rays between them until ray hits
    /// </summary>
    /// <param name="minViewCast"></param>
    /// <param name="maxViewCast"></param>
    /// <returns></returns>
    edgeInfo findEdge(viewCastInfo minViewCast, viewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i = 0; i < edgeResIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            viewCastInfo newViewCast = viewCast(angle);
            bool thresholdExceeded = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDistThreshhold; 

            if (newViewCast.hit == minViewCast.hit && !thresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new edgeInfo(minPoint, maxPoint);

    }

    public Vector3 dirFromAngle(float angleInDegrees, bool angleIsGlobal) 
    {
        if(angleIsGlobal != true)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        
    }

    /// <summary>
    /// Struct for all vision ray cast information
    /// </summary>
    public struct viewCastInfo 
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public viewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }
    }

    /// <summary>
    /// Struct for edge detection info
    /// </summary>
    public struct edgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public edgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}
