using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI : MonoBehaviour
{
    public GameObject player;

    public PlayerAbilities abilities;
    public PlayerController playerController;
    public Transform playerCamera;
    public LightCheck lightCheck;
    public HP playerHealth;

    public TMP_Text HP;
    public Slider health;
    public TMP_Text lightCharge;

    public Slider dash;
    public Slider lightSteal;
    public TMP_Text sneakText;
    public Slider sneakSlider;
    public Slider lightLevel;
    public Slider lightCutoff;
    public GameObject stealthKill;
    public Image Cursor;

    private void Start()
    {
        //setup slider max values
        lightSteal.maxValue = abilities.lightStealCooldown;
        dash.maxValue = abilities.dashCooldown;
        health.maxValue = playerHealth.maxHealth;
        sneakSlider.maxValue = abilities.timeToStealthKill;
       
    }

    // Update is called once per frame
    void Update()
    {
        //set slider values
        lightSteal.value = abilities.lightCooldownReset;
        dash.value = abilities.dashCooldownReset;
        health.value = playerHealth.currentHealth;
        sneakSlider.value = abilities.stealthKillTimer;
        lightLevel.value = lightCheck.lightLevel / 10000;

        lightCharge.text = abilities.lightCharges.ToString();
        HP.text = (playerHealth.currentHealth + "/" + playerHealth.maxHealth);

        
        lightCutoff.value = lightCheck.litnessLimit / 10000;

        //If player is in the light set slider color to red
        if (!lightCheck.isDark)
        {
            lightLevel.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.red;
        }
        else lightLevel.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.yellow; //default slider to yellow

        //check for player behind enemy within sneak attack range
        RaycastHit hitEnemy;
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hitEnemy, abilities.stealthKillRange, abilities.targetLayer))
        {
            if (hitEnemy.transform.gameObject != null)
            {
                float angleDif;
                angleDif = Quaternion.Angle(hitEnemy.transform.rotation, player.transform.rotation); //get the difference between players angle and the enemy angle
                if (angleDif < abilities.maxStealthKillAngle) //if player is within the stealth kill angle i.e. behind the enemy then stealth kill
                {
                    stealthKill.SetActive(true); //show stealth kill prompt and slider
                }
            }
        }
        else
        {
            stealthKill.SetActive(false); //hide stealth kill UI
        }

        //check for player looking at light interactable
        if(Physics.Raycast(playerCamera.position, playerCamera.forward, abilities.lightStealRange, abilities.lightLayer))
        {
            Cursor.color = Color.magenta; //set cursor colour to show looking at interactable
        }
        else Cursor.color = Color.white; //ser cursor colour to white
    }
}
