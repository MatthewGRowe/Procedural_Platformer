using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spring : MonoBehaviour
{
    [SerializeField] 
    private float springForce = 20f; //Force applied to player
    private SpriteRenderer spriteRenderer; //Sprite 
    private Sprite originalSprite; // Store the default sprite
    private Animator springAnim;

    private void Start()
    {
        //Setup
        springAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite; // Save the original sprite

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
            // Launch player straight up (could add directional logic if you want chaos)
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(rb.velocity.x, 0); // Cancel existing Y velocity
            rb.AddForce(Vector2.up * springForce, ForceMode2D.Impulse);
            AudioManager.instance.Play(MyTags.SOUND_BOING);
            // Trigger spring animation if it has one
            springAnim = GetComponent<Animator>();
            springAnim.enabled = true;
            // Get the length of the current animation state
            float animationLength = springAnim.GetCurrentAnimatorStateInfo(0).length;
            // Disable the Animator after the clip finishes
            Invoke("DisableAnimator", animationLength);

            if (springAnim != null)
            {
                springAnim.SetTrigger("Bounce"); // You’ll need a Bounce trigger in the Animator
            }

            // Optional: play a sound effect, camera shake, particle burst, jazz hands
        }
    }

    void DisableAnimator()
    {
        springAnim.enabled = false;
        spriteRenderer.sprite = originalSprite; // Reset to the original sprite
    }

    public void ResetSprite()
    {
        //Called by animation event
        spriteRenderer.sprite = originalSprite;
    }
}
