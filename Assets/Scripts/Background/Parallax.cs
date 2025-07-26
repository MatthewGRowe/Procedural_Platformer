using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Parallax : MonoBehaviour
{

    //public GameObject cam;
    //public float parallaxEffect;
    //public bool autoScroll = false;

    //private float length, startpos;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    startpos = transform.position.x;
    //    length = GetComponent<SpriteRenderer>().bounds.size.x;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    float temp = cam.transform.position.x * (1 - parallaxEffect);
    //    float distance = (cam.transform.position.x * parallaxEffect);

    //    float desiredXPos = startpos + distance;

    //    if (autoScroll)
    //    {
    //        // this will push bg to the left
    //        desiredXPos = transform.position.x - parallaxEffect;
    //    }

    //    transform.position = new Vector3(desiredXPos, transform.position.y, 0);


    //    if (temp > startpos + length)
    //    {
    //        startpos += length;
    //    }
    //    else if (temp > startpos - length)
    //    {
    //        startpos -= length;
    //    }

    private float length, startPos;
    public GameObject cam;
    public float parallaxEffect = 1;
    public bool autoScroll = false;
    private bool doINeedToAutoScroll;


    private void Awake()
    {
        cam = GameObject.FindWithTag("MainCamera");
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        doINeedToAutoScroll = autoScroll;  //Set to true if the ORIGINAL VALUE is autoscroll

    }

    // Update is called once per frame
    void Update()
    {


        //How far have we moved in relation to the camera
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        if (doINeedToAutoScroll)
        {
            //Check if player is idle (if so stop the parralax effect)
            if (GameManager.instance.playerIsIdle)
            {
                //Player is idle make the autoscroll work so it looks like we have flies going past!
                autoScroll = true;
            }
            else
            {
                autoScroll = false;  //Don't autoscroll while running as it looks odd
            }


        }
     

        //How far have we moved from start point?
        float dist = (cam.transform.position.x * parallaxEffect);   //In world space

        //*******************Changed bit
        float distance = (cam.transform.position.x * parallaxEffect);
        float desiredXPos = startPos + distance;

        if (autoScroll)
        {
            // this will push bg to the left
            desiredXPos = transform.position.x - parallaxEffect;
        
        }
        transform.position = new Vector2(desiredXPos, transform.position.y);
        //transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        float total = startPos + length;

        //Move images to the start
        if (cam.transform.position.x - transform.position.x > length && autoScroll)  //Distance from currentcameraX to -length
        {

            startPos = cam.transform.position.x + length; //Move forward
            transform.position = new Vector2(startPos, transform.position.y);

        }
        else if (autoScroll == false)
        {
            //Move images to the start
            if (temp > startPos + length)
            {
                
                startPos += length; //Move forward
            }
            else if (temp < startPos - length)
            {
           
                startPos -= length; //Move back
            }
        }

    }

    private void SetTransform(float startPos, float desiredXPos)
    {

    }
}
