using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour
{
    public Texture2D fadeOutTexture;  //This is a texture that will overlay the screen.  You add a semi-transparent sprite for this
   
    public float fadeSpeed = 0.8f;  //Speed that the screen fades

    private int drawDepth = -10;    //The textures order in the draw heirachy, this makes it render on top (low number)
    private float alpha = 0.5f;     //The texture's alpha value between 0 and 1
    private int fadeDir = -1;       //Fade in = -1, fade out = 1

   
    void OnGUI()  //OnGui is called every time something is required to render on the screen
    {               //OnGUI gives you access to some graphical options

      
        //Fade out/in the alpha direction a speed and TimeDelta to convert the operation into seconds
        alpha += fadeDir * fadeSpeed * Time.deltaTime;

        //Clamp the number between 0 and 1 as G?UI.color uses a value between 0 and 1
        alpha = Mathf.Clamp01(alpha);

        //Set colour of GUI, all colour vals remain the same and Alpha is set to the alpha variable
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;  //make the black tesxture render on top
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);  //Draw the texture to fit screen
    }

    public float BeginFade(int direction)
    {
        //If direction = 1 then we fade from normal colour to the dark colour
        //If direction = -1 then we fade from dark colour to a normal colour

        fadeDir = direction;
        return (fadeSpeed);  //return fadespeed var so it is easy to time the Application.LoadLevel();
    }

}
