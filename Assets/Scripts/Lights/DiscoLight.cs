using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 //For light effects in 2D

//Used to calc the light positions
//[RequireComponent(typeof(BoxCollider2D))]

public class DiscoLight : MonoBehaviour
{
    //Creates a moving disco light effect with different colours inside a box collider

    //The colours of the light
    [Header("Do NOT leave the Alpha at zero!!")]
    public Color[] LightColours;

    //Rotates the light between 2 points
    public float rotateSpeed = 120f;


    //Movement either horizontal or vertical
    //[Header("If this is false the light will move horizontally within the box collider 2D")]
    //public bool vertical;

    public float timeBetweenColour = 1f;

    UnityEngine.Rendering.Universal.Light2D myLight;
    //BoxCollider2D myBox;
    //List<float> lightPositions = new List<float>();

    private void Start()
    {
        //Figure out what the light is (the script is attached to it)
        myLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        //Pause slightly before the flickering starts (may be useful sometimes)
        StartCoroutine(StartDisco());

        //Calculate the positons for the lights to be displayed
        //int numOfLights = LightColours.Length;
        //myBox = GetComponent<BoxCollider2D>();



        //if (vertical)
        //{
        //    float boxHeight = myBox.size.y;  //Read the height of the box
        //    float interval = boxHeight / numOfLights;

        //    for (int y = 0; y < LightColours.Length; y++)
        //    {
        //        //Fill up array of lightPositons
        //        lightPositions.Add((gameObject.transform.position.y - boxHeight + (interval * y))*0.8f);   //ie first pos is at the bottom of the box pos 0
        //                                                                    //second pos is at interval *1 etc...
        //    }
        //}
        //else   //Horizontal
        //{
        //    float boxWidth = myBox.size.x;
        //    float interval = boxWidth / numOfLights;

        //    for (int x = 0; x < LightColours.Length; x++)
        //    {
        //        //Fill up array of lightPositons
        //        lightPositions.Add(gameObject.transform.position.x + (interval * x));     //ie first pos is at the bottom of the box pos 0
        //                                                                    //second pos is at interval *1 etc...
        //    }
        //}

        


    }

    private void Update()
    {
        transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * rotateSpeed, 45) - 30);
    }

    IEnumerator StartDisco()
    {
        //Adjust the outer radius at given points.
        yield return new WaitForSeconds(timeBetweenColour);
        //Pick next colour to be displayed
        int nextColor = Random.Range(0, LightColours.Length);
        myLight.color = LightColours[nextColor];

        //For moving lights, we don't need to move!
        //if (vertical)
        //{
        //    myLight.transform.position = new Vector3(myLight.transform.position.x, lightPositions[nextColor], myLight.transform.position.z);
        //}
        //else
        //{
        //    myLight.transform.position = new Vector3(lightPositions[nextColor], myLight.transform.position.y, myLight.transform.position.z);
        //}
        StartCoroutine(StartDisco());
    }


}
