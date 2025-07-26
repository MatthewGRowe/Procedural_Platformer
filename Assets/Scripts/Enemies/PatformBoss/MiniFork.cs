using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniFork : MonoBehaviour
{

    private bool stopSpammingDamage = false;
    void Start()
    {
        Destroy(gameObject, 5f); // Auto-destroy after 5 seconds
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(MyTags.PLAYER_TAG) && stopSpammingDamage == false)
        {
            stopSpammingDamage=true;
            // Damage the player, or knock them back, etc.
            Debug.Log("Player hit by mini Trishul!");
            AudioManager.instance.Play("Sizzle");
            GameManager.instance.DropHealth(0.1f);
            StartCoroutine(AllowDamage());
        }
        else if (collision.CompareTag(MyTags.GROUND_LAYER_TAG))
        {
            // Shatter on impact
            Destroy(gameObject);
        }
    }

    IEnumerator AllowDamage()
    {
        yield return new WaitForSeconds(0.8f);
        stopSpammingDamage = false;

    }
}
