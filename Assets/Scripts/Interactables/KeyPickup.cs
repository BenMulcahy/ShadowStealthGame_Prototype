using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public PlayerController player;

    private void OnDestroy()
    {
        player.hasKey = true;
        Debug.Log("GivePlayerKey"); 
    }

}
