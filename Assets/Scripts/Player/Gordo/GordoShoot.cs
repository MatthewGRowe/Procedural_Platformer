using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GordoShoot : MonoBehaviour
{
    //Attached to all players, only enabled on some (as and when necessary).
    /*
    public GameObject fireBullet;  //The bullet prefab 
    public Transform bulletInstantiationPoint;
    bool canShoot = true;
    public Sprite myAmmoSprite;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>(); //Link to the animator component
        GameManager.instance.AmmoSprite = myAmmoSprite;  //Set sprite
    }

    void Update()
    {
        ShootBullet();
    }

    void ShootBullet()
    {

        if (GameManager.instance.GetBullets() > 0)
        {

            if (Input.GetKeyDown(KeyCode.J) && canShoot) //If user has pressed "J"
            {
                anim.SetBool("Throw", true);
                print("Shoot!");
                canShoot = false;
                //End game and restart
                FindObjectOfType<AudioManager>().Play(MyTags.SOUND_GUNSHOT);
                GameObject bullet = Instantiate(fireBullet, bulletInstantiationPoint.position, Quaternion.identity); //Create an instance / copy of the bullet
                                                                                                                     //transform.position = the co-ords of the player as this script is attached to the player
                                                                                                                     //Quaternion identity is a shortcut for 0,0,0
                bullet.GetComponent<BulletScript>().Speed *= transform.localScale.x;
                bullet.GetComponent<BulletScript>().direction = transform.localScale.x;
                GameManager.instance.Ammo(-1); //Reduce ammo
                                               //This sets the bullet speed to either +10 or -10
                                               //depending on the direction the player is facing
                                               //The Speed property is a public property of the FireBullet class (script)

                StartCoroutine(PauseShoot());
            }
        }
    }
    IEnumerator PauseShoot()
    {
        yield return new WaitForSeconds(1f);
        canShoot = true;
    }

    public void StopThrow()
    {
        anim.SetBool("Throw", false);
        print("Stop shooting!");
    }
    */
}
