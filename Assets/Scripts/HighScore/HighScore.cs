using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class HighScore : MonoBehaviour
{
    public GameObject highScoreEntryPrefab;
    public Transform highScorePanel;
    private int playerIndex;
    private bool quitting = false;  //True if user is quitting

    private const int maxEntries = 10;  //No more than 10 in our table
    private List<HighScoreEntry> highScores = new List<HighScoreEntry>(); //See HighScoreEntry.cs for data structure

    [SerializeField]
    List<GameObject> ShutDownOrder = new List<GameObject>();
    public void StartUp()  //Called from GoButton.cs
    {
        //When the scene launches we display the scores
        LoadHighScores();  //From PlayerPrefs
        DisplayHighScores();  //In the scene

    }
    private void Update()
    {
        
        if (FindObjectsOfType<HighScore>().Length > 1)
        {
            Debug.LogWarning("Multiple HighScore scripts found in the scene!");
        }
    }

    public void AddHighScore(string playerName)
    {
        int score = 0;

        // Get score from GameManager (or use 35 if testing)
        if (GameManager.instance == null)
        {
            score = 45;
        }
        else
        {
            score = GameManager.instance.score;
        }

        // Load existing scores from PlayerPrefs
        LoadHighScores();

        // Create a new high score entry
        HighScoreEntry newEntry = new HighScoreEntry(playerName, score);
        highScores.Add(newEntry);

        // Sort all entries in descending order by score
        highScores.Sort((a, b) => b.score.CompareTo(a.score));

        // Remove lowest scores if more than max allowed
        if (highScores.Count > maxEntries)
        {
            highScores.RemoveAt(highScores.Count - 1);
        }

        // Store the player's index (if they're still in the list)
        playerIndex = highScores.IndexOf(newEntry);

        // Save updated list to PlayerPrefs
        SaveHighScores();
    }

    void DisplayHighScores()
    {
        foreach (Transform child in highScorePanel)
        {
            Destroy(child.gameObject);  //Clear the old entries
        }

        for (int i = 0; i < highScores.Count; i++)
        {
            GameObject entry = Instantiate(highScoreEntryPrefab, highScorePanel); //Instantiate entries as children of highscorepanel
            entry.transform.localScale = Vector3.one; //Set scale to 1,1,1

            //Enable us to manipulate the text for each entry
            TextMeshProUGUI rankText = entry.transform.Find("txtRank").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = entry.transform.Find("txtName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = entry.transform.Find("txtScore").GetComponent<TextMeshProUGUI>();
            //Make most recent score red
            if (i == playerIndex)
            {
                rankText.color = Color.red;
                nameText.color = Color.red;
                scoreText.color = Color.red;
            }



            rankText.text = (i + 1).ToString();  //Entries 1..10
            nameText.text = highScores[i].name;  //Add the name from the list
            scoreText.text = highScores[i].score.ToString();  //Add the score from the list

            if (i == 0)  //Top score is always gold
            {
                //Add ** to first place
                nameText.text = "*** " + highScores[i].name + " ***";
                rankText.color = Color.yellow;
                nameText.color = Color.yellow;
                scoreText.color = Color.yellow;
            }
        }
    }

    void SaveHighScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            //Add all the entries to PlayerPrefs
            PlayerPrefs.SetString("HS_name_" + i, highScores[i].name);
            PlayerPrefs.SetInt("HS_score_" + i, highScores[i].score);
        }
        PlayerPrefs.SetInt("HS_Count", highScores.Count); //Should always be 10
        PlayerPrefs.Save();
    }

    void LoadHighScores()
    {
        highScores.Clear();  //Clear the list
        int count = PlayerPrefs.GetInt("HS_Count", 0);  //Read total number (should be 10)
        for (int i = 0; i < count; i++)
        {
            //for each value in playerprefs
            string name = PlayerPrefs.GetString("HS_name_" + i, "AAA"); //Format with capitals
            int score = PlayerPrefs.GetInt("HS_score_" + i, 0);
            highScores.Add(new HighScoreEntry(name, score)); //Add to highscore table
        }

        // Add dummy scores if none exist
        if (highScores.Count == 0)
        {
            highScores.Add(new HighScoreEntry("SUSAN", 100));
            highScores.Add(new HighScoreEntry("PETER", 90));
            highScores.Add(new HighScoreEntry("OLIVE", 80));
            highScores.Add(new HighScoreEntry("OSCAR", 70));
            highScores.Add(new HighScoreEntry("NINA", 60));
            highScores.Add(new HighScoreEntry("FRED", 50));
            highScores.Add(new HighScoreEntry("EMMA", 40));
            highScores.Add(new HighScoreEntry("DAVE", 30));
            highScores.Add(new HighScoreEntry("", 0));
            highScores.Add(new HighScoreEntry("", 0));

            SaveHighScores();
        }

    }

    public void StartNewGame()
    {

        SceneManager.LoadScene(MyTags.SCENE_MAIN_GAME);
    }

    public void QuitGame()
    {
        print("!quit");
        if (quitting == false)
        {
            if (AudioManager.instance != null) //We have an audio manager
            {
                AudioManager.instance.Play(MyTags.SOUND_LEVELCOMPLETE);

            }

            quitting = true;

            StartCoroutine(QuitAfterPause());
        }
    }

    private IEnumerator QuitAfterPause()
    {
        foreach (GameObject objectToClose in ShutDownOrder)
        {
            // Hide the object first (optional)
            objectToClose.SetActive(false);
            yield return new WaitForSeconds(0.2f);

            // Show it again and enable physics
            objectToClose.SetActive(true);
            Rigidbody2D rb = objectToClose.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1f;

                if (objectToClose.name != "HighScorePanel")
                {// Slight initial tilt
                    objectToClose.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-25f, 25f));

                    // Gentle torque
                    //rb.AddTorque(Random.Range(-2f, 2f));

                    // Soft push
                    //rb.AddForce(new Vector2(Random.Range(-10f, 10f), -30f));
                }
                else
                {
                    rb.bodyType= RigidbodyType2D.Dynamic;
                }
            }

            // Wait a bit before next object falls
            yield return new WaitForSeconds(0.6f);
        }

    }
}
