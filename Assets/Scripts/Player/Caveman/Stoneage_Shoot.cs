using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoneage_Shoot : MonoBehaviour
{
    //Attached to all players, only enabled on some (as and when necessary).

    
    public Sprite myAmmoSprite;
    bool canShoot = true;
    public Animator myAnim;
    private bool isAttacking = false;

    // Public read-only property
    public bool IsAttacking => isAttacking; //Not exposed in Unity inspector an alternative to SerializeField


    [SerializeField] GameObject myClub;

    private void Awake()
    {
        myAnim = GetComponentInChildren<Animator>();
     
    }

    private void Start()
    {
        GameManager.instance.Ammo(99);  //Give 99 swipes to start
  
        GameManager.instance.AmmoSprite = myAmmoSprite;
        myClub.tag = MyTags.PLAYER_TAG;

    }
    void Update()
    {
        RaiseClub();
    }

    public void StopAttackAnimation()
    {
      
        //Called from animation event in attack animation
        myAnim.SetBool("attack", false);
    }

    void RaiseClub()
    {
        
        if (GameManager.instance.GetBullets() > 0)
        {
            if (Input.GetKeyDown(KeyCode.J) && canShoot) //If user has pressed "J"
            {
                isAttacking = true;
                myClub.tag = MyTags.BULLET_TAG; //turn the club into a bullet while swinging!
                GameManager.instance.playerInvincible = true;  //Player is invincible so any interactions will result in the other object being destroyed
                canShoot = false;
                //End game and restart
                FindObjectOfType<AudioManager>().Play(MyTags.SOUND_HEAVYTHROW);
                
                myAnim.SetBool("attack", true);

                GameManager.instance.Ammo(-1); //Reduce ammo
                                               //This sets the bullet speed to either +10 or -10
                                               //depending on the direction the player is facing
                                               //The Speed property is a public property of the FireBullet class (script)

                StartCoroutine(PauseShoot());
                StartCoroutine(ResetAttackAfterDelay(0.6f)); // Reset attack after 0.6 seconds
            }
        }
    }
    private IEnumerator ResetAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        myAnim.SetBool("attack", false);
        GameManager.instance.playerInvincible = false;
        myClub.tag = MyTags.PLAYER_TAG;
        canShoot = true;
        isAttacking = false;
    }

    IEnumerator PauseShoot()
    {
        
        yield return new WaitForSeconds(0.6f);
        GameManager.instance.playerInvincible = false;
        myClub.tag = MyTags.PLAYER_TAG;
        canShoot =true;
        isAttacking =false;
    }
}
