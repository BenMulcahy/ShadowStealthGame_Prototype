using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parenting : MonoBehaviour
{

    public GameObject hand;
    public Vector3 offset;
    public Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.parent = hand.transform;
        transform.localPosition = offset;
        transform.localRotation = rot;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
