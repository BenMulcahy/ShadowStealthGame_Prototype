using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Controls UI for showing enemy detection level
/// </summary>
public class DetectionUI : MonoBehaviour
{
    public Slider detectionSlider;
    FieldOfView enemyParentFOV;

    private void Awake()
    {
        enemyParentFOV = transform.gameObject.GetComponentInParent<FieldOfView>(); //get refernce to FOV script on parent enemy
        detectionSlider.maxValue = enemyParentFOV.chaseThreshold; //set max value of slider to the parent enemy chase threshold
    }

    void Update()
    {
        detectionSlider.value = enemyParentFOV.detection; //set slider value to detection value of parent enemy
    }
}
