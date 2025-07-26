using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCollectable : MonoBehaviour
{
  
   

 

    // Update is called once per frame
    void Update()
    { 
        transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * 60, 100)-45);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
            GameManager.instance.Ammo(5);
            Destroy(transform.parent.gameObject);
        }
    }


}
