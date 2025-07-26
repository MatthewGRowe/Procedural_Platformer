using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    List<Collider2D> disabledColliders = new List<Collider2D>();
    [SerializeField] private float colliderRestoreDelay = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG)
        {
        
         

            // Disable colliders
            Collider2D[] colliders = collision.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in colliders)
            {
                if (col.enabled)
                {
                    col.enabled = false;
                    disabledColliders.Add(col);
                }
            }

            // Start coroutine to re-enable colliders after a short delay
            StartCoroutine(RestoreColliders());
        }
    }

    private IEnumerator RestoreColliders()
    {
        yield return new WaitForSeconds(colliderRestoreDelay);

        foreach (Collider2D col in disabledColliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }
        disabledColliders.Clear(); // Clear list to prevent issues

        GameManager.instance.LoseALife();  //Lose a life
    }
}
