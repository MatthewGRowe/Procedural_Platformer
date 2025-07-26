using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinningTrophy : MonoBehaviour
{
    bool collectable = false;
    public GameObject theText;
    public GameObject theParticles;

    void Start()
    {
        StartCoroutine("RaiseBeer");
        theText.SetActive(false);
        theParticles.SetActive(false);  
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == MyTags.PLAYER_TAG  && collectable)
        {
            //Deal with points
            int completionBonus = Random.Range(0, 100);
            GameManager.instance.Points(completionBonus);
            theText.GetComponent<TextMeshPro>().text = "Completion Bonus: " + completionBonus + "\r\nTotal Score: " + GameManager.instance.score;
            theText.SetActive(true);

            collectable = false;
            GetComponent<SpriteRenderer>().enabled = false; // Hide immediately
            
            
            AudioManager.instance.StopAllSounds();
            AudioManager.instance.Play("Burp");

            //Display highscore
            StartCoroutine(LoadHighScore());
        }
    }

    IEnumerator LoadHighScore()
    {
        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene(MyTags.SCENE_HIGHSCORE);
    }

    private bool isTouchingGround = true;

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isTouchingGround = false;
        }
    }


    IEnumerator RaiseBeer()
    {
        yield return new WaitForSeconds(2f);
        AudioManager.instance.Play("TrophyArising");

        float riseSpeed = 2f;
        isTouchingGround = true;
        float minRiseTime = 1f; // Force at least 1 seconds of movement
        float elapsed = 0f;

        while (isTouchingGround || elapsed < minRiseTime)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 4f) * 5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = Quaternion.identity;
        AudioManager.instance.StopPlay("TrophyArising");
        StartCoroutine(WobbleForever());
        StartCoroutine(AllowCollection());
    }
    IEnumerator AllowCollection()
    {
        theParticles.SetActive(true);
        AudioManager.instance.Play("BeerOpen");
        yield return new WaitForSeconds(3);
       
        AudioManager.instance.Play("SmallCheer");
        yield return new WaitForSeconds(3);
        collectable = true;

    }
    IEnumerator WobbleForever()
    {
   
        
        AudioManager.instance.StopPlay("TrophyArising");
        float wobbleSpeed = 3f;     // How fast it wobbles
        float wobbleAmount = 5f;    // How far it rotates (in degrees)
        
        
        while (true)
        {
            float zRotation = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
            yield return null; // Wait for next frame
        }
    }
}
