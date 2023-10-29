using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Health script for gameObjects, can be applied to player, enemies and any destructable items
/// </summary>
public class HP : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public float regenHealthRate;
    public bool canRegenHealth;
    public float healthRegenAmount;

    public bool isStealthKillable;
    public bool canRespawn;
    public Vector3 spawnLocation;

    void Start()
    {
        spawnLocation = transform.position; //get spawn location if object can be respawned
        currentHealth = maxHealth; //set current health to maximum health
        StartCoroutine(RegainHealth());
    }

    /// <summary>
    /// Deal damage to object by given damage value
    /// </summary>
    /// <param name="damageDealt"></param>
    public void Damage(float damageDealt)
    {
        currentHealth -= damageDealt; //decrease current health by damage dealt
        //Debug.Log("Dealt " + damageDealt + " to object " + gameObject.name);

        //if object health is less or equal to 0 then check if respawnable, if so respawn else destroy gameobject
        if (currentHealth <= 0f)
        {
            if (canRespawn)
            {
                Respawn();
            }
            else if (!canRespawn)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Regain health if possible - regain by given amount per regainHealthRate up to max health
    /// </summary>
    /// <returns></returns>
    public IEnumerator RegainHealth()
    {
        while (true)
        {
            if(currentHealth < maxHealth && canRegenHealth)
            {
                yield return new WaitForSeconds(regenHealthRate);
                currentHealth = Mathf.Clamp(currentHealth += healthRegenAmount,0, maxHealth);
            }
            else yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Respawn function - if gameobject to respawn is player then reload scene, otherwise reset gameobject position and health
    /// </summary>
    public void Respawn()
    {
        if (gameObject.name == "Player") SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reloud scene
        else transform.SetPositionAndRotation(spawnLocation, Quaternion.identity); //move gameobject to spawn location 
        currentHealth = maxHealth; //reset object health
    }
}
