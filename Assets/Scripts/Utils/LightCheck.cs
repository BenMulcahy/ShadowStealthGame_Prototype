using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Created using https://www.youtube.com/watch?v=NYysvuyivc4&t=144s

public class LightCheck : MonoBehaviour
{
    public RenderTexture lightCheckTexture;
    public float lightLevel;
    public bool isDark;
    public float darknessLimit;
    public float litnessLimit;
    int tmpTextureDepth = 0;

    public float damageTickTime = 2f;
    public float lightExposureDamage = 5f;
    float damageTimer = 0f;



    void Update()
    {

        //Create a temporary copy from the lightchecktexture this ensures we dont interfere with the lightchecktexutre itself
        RenderTexture tmpTexture = RenderTexture.GetTemporary(lightCheckTexture.width, lightCheckTexture.height, tmpTextureDepth, RenderTextureFormat.Default, RenderTextureReadWrite.Linear); //Create tmp texture with same size as lightchecktexture, set colour space to linear
        Graphics.Blit(lightCheckTexture, tmpTexture); //Copy lightcheck texture into tmptexture
        RenderTexture previous = RenderTexture.active; //save the currently active render texture into previous - this render texture should be the lightchecktexture
        RenderTexture.active = tmpTexture; //set tmp texture to active


        //Create texture2D from temp texture
        Texture2D temp2DTexture = new Texture2D(lightCheckTexture.width, lightCheckTexture.height); //create a new temp2DTexture the same size as the lightchecktexture
        temp2DTexture.ReadPixels(new Rect(0, 0, tmpTexture.width, tmpTexture.height),0, 0); //Copys the pixels from the current active render texture (tmpTexture) 
        temp2DTexture.Apply(); //applies changes made in previous line

        RenderTexture.active = previous; //sets active render texture back to lightchecktexture
        RenderTexture.ReleaseTemporary(tmpTexture); //releases tmptexture from memory


        //Get colour information from lightchecktexture
        Color32[] colours = temp2DTexture.GetPixels32(); //store pixels from 2dtexture into colours array
        Destroy(temp2DTexture); //deletes the 2dtexture since we no longer need it

        lightLevel = 0; //resets light level

        //loop through colours array
        for (int i = 0; i< colours.Length; i++)
        {
            lightLevel += (0.2126f * colours[i].r) + (0.7152f * colours[i].g) + (0.722f * colours[i].b); //get white value from each pixel
        }

        lightLevel = lightLevel / 1000;
        
        //Compare light levels to limits to determine if character is in dark or not
        if(lightLevel<= darknessLimit)
        {
            isDark = true;
            if(damageTimer != 0) damageTimer = 0f;
        }
        if(lightLevel>= litnessLimit)
        {
            isDark = false;
            DamagePlayer();
        }


        //Debug.Log(damageTimer);
        //Debug.Log("In Darkness " +isDark);
        //Debug.Log("Light level = " + lightLevel);
    }

    void DamagePlayer()
    {
        if (damageTimer <= damageTickTime)
        {
            damageTimer += Time.deltaTime;
        }
        else
        {
            GetComponent<HP>().Damage(lightExposureDamage);
            damageTimer = 0;
        }
        
    }

}
