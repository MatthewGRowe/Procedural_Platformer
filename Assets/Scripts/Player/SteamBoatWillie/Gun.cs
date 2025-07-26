using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float rotateSpeed = 60f;
    private void Update()
    {

        //float angle = Mathf.PingPong(Time.time, 140) - 70;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * rotateSpeed, 30) - 20);

        if (GameManager.instance.GetBullets() == 0)
        {
            //Hide the sprite
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;  
        }
        
    }
}
