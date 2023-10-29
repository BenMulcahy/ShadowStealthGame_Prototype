using System.Collections;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public TMP_Text text;

    private void Start()
    {
        text.enabled = false;
    }
    
    //show text when player enters hitbox
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            text.enabled = true;
        }
        else return;
    }

    //hide text when player leaves hitbox
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            text.enabled = false;
        }
        else return;
    }
}
