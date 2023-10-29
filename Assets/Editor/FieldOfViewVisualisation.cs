using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Visualise FOV + detection spread in editor
/// </summary>
[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewVisualisation : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius); //draw an arc in editor around enemy with the view radius
        Vector3 viewAngleA = fov.dirFromAngle(-fov.viewAngle / 2, false); 
        Vector3 viewAngleB = fov.dirFromAngle(+fov.viewAngle / 2, false);

        //Draw lines from enemy to edge of radius to display FOV
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius); 
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.green;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.detectedSpreadRad); //draw arc in editor for detection spread

        //Draw line to each target found (player head and player body)
        Handles.color = Color.red;
        foreach(Transform visableTarget in fov.visableTargets)
        {
            Handles.DrawLine(fov.transform.position, visableTarget.position);
        }

        
    }

}
