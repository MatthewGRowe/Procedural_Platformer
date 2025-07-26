using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
 //For light effects in 2D

public class FlickeringLight : MonoBehaviour
{
    //Copied from "How to make a cool flickering light system in Unity2D
    //On YouTube, author OneTrueKingGames 17/01/2020

    public float betweenLightFlickers, lightFlickerMin, lightFlickerMax, beginningTime=0f;
    Light2D myLight;

    private void Start()
    {
        //Make sure we are in the same position as the parent
        transform.position = transform.parent.position;
        //Figure out what the light is (the script is attached to it)
        myLight = GetComponent<Light2D>();
        //Pause slightly before the flickering starts (may be useful sometimes)
        StartCoroutine(StartScene());
        //Set the angle (for a candle)
        myLight.pointLightInnerAngle = Random.Range(23, 80);
        myLight.pointLightOuterAngle = Random.Range(230, 280);
    }

    IEnumerator StartScene()
    {
        //Pause before you start flickering (may not be useful)
        yield return new WaitForSeconds(beginningTime);
        StartCoroutine(LightFlicker());
    }
    
    IEnumerator LightFlicker()
    {
        //Adjust the outer radius at given points.
        yield return new WaitForSeconds(betweenLightFlickers);
        myLight.pointLightOuterRadius = Random.Range(lightFlickerMin, lightFlickerMax);
        StartCoroutine(LightFlicker());
    }
}
