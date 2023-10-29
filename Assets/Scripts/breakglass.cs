using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakglass : MonoBehaviour
{
    public GameObject glassCollider;
    bool broken = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && broken != true)
        {
            broken = true;
            breakGlass();
            FindObjectOfType<AudioManager>().Play("GlassShatter");
        }
    }

    void breakGlass()
    {
        GetComponentInChildren<BreakableWindow>().breakWindow();
        Destroy(glassCollider);
    }

}
