using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endLevel : MonoBehaviour
{
    public Collider player;

    private void OnTriggerEnter(Collider other)
    {
        if (other == player)
        {
            SceneManager.LoadScene(0);
        }
    }
}
