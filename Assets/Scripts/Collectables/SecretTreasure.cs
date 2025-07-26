using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SecretTreasure : MonoBehaviour
{
    [SerializeField] private TextMeshPro rewardText; // Assign in Inspector
    private int _points;
    private bool isCollected; // Prevent double-collection


    [SerializeField] 
    private TreasureSpriteDatabase spriteDatabase; //Uses the database, see the folder where this script lives, the TreasureSpriteDatabase.cs creates the menu option, the menu creates the asset which we are now referencing.
    private void Start()
    {
        _points = Random.Range(30, 50);

        if (rewardText != null)
            rewardText.enabled = false;

        if (spriteDatabase == null)
            Debug.LogError("treasureSprites list is null");
        else
           // Debug.Log("treasureSprites count: " + spriteDatabase.treasureSprites.Count);



        StartCoroutine(RunAfterFirstUpdate());
    }

    IEnumerator RunAfterFirstUpdate()
    {
        yield return null; // Waits until the first Update() completes
                           // Check if the list has sprites
        if (spriteDatabase == null || spriteDatabase.treasureSprites == null || spriteDatabase.treasureSprites.Count == 0)
        {
            Debug.LogError("Treasure sprite database is missing or empty!");
        }
        else
        {
            int rnd = Random.Range(0, spriteDatabase.treasureSprites.Count);
            GetComponent<SpriteRenderer>().sprite = spriteDatabase.treasureSprites[rnd];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected == false && collision.CompareTag(MyTags.PLAYER_TAG))
        {

            isCollected = true;
            AwardTreasure();
        }
    }

    private void AwardTreasure()
    {
        // Award points
        GameManager.instance?.Points(_points); // Null check for safety

        // Play sound
        AudioManager.instance.Play("Treasure");

        // Disable visuals/physics
        GetComponent<SpriteRenderer>().enabled = false;

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        GetComponent<BoxCollider2D>().enabled = false;
        // Disable the Particle System
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop();           // Stops new particles from spawning
            ps.Clear();          // Removes existing particles immediately
            
        }


        rewardText.text = $"+{_points} Points!";
        rewardText.enabled = true;
        

  

        // Destroy after delay
        StartCoroutine(DestroyAfterDelay(2f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}