using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            if (other.GetComponent<PlayerController>().hasKey == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else Debug.Log("Key Required First");
        }
    }
}
