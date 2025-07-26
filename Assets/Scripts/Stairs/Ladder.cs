using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public LayerMask platformLayer; // Layer used by ceilings/platforms
    private Collider2D playerCollider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(MyTags.PLAYER_TAG))
        {
            
            playerCollider = other;
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
            }

            // Disable collisions with platform layer
            Physics2D.IgnoreLayerCollision(other.gameObject.layer, LayerMaskToLayer(platformLayer), true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(MyTags.PLAYER_TAG))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float vertical = Input.GetAxis("Vertical");
                rb.velocity = new Vector2(0, vertical * 5f); // adjust climb speed
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(MyTags.PLAYER_TAG))
        {
            
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 1;
            }

            // Re-enable platform collisions
            Physics2D.IgnoreLayerCollision(other.gameObject.layer, LayerMaskToLayer(platformLayer), false);
        }
    }

    // Utility method to convert LayerMask to Layer index
    int LayerMaskToLayer(LayerMask layerMask)
    {
        int layer = 0;
        int layerValue = layerMask.value;
        while (layerValue > 1)
        {
            layerValue = layerValue >> 1;
            layer++;
        }
        return layer;
    }
}