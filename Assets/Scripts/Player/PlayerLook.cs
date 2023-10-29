using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//player Controller built from brackeys tutorial as base: https://www.youtube.com/watch?v=_QajrabyTJc

public class PlayerLook : MonoBehaviour
{
    //Mouse look values
    public float mouseSensitivityX = 300;
    public float mouseSensitivityY = 300;
    public bool InvertLook = false;

    //Reference to playerBody 
    public Transform playerBody;

    float xRot = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Lock cursor 
    }

    
    void Update()
    {


        //Player look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        // Player look in Y. If statment checks for if Y look is inverted or not. Rotates camera in Y
        if (InvertLook == false)
        {
            xRot -= mouseY;
            xRot = Mathf.Clamp(xRot, - 90, 90);
            transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        }
        else if (InvertLook == true)
        {
            xRot += mouseY;
            xRot = Mathf.Clamp(xRot, -90, 90);
            transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        }

       

        playerBody.Rotate(Vector3.up * mouseX); //Rotates player body by mouseX allowing for X look

    }
       
}
