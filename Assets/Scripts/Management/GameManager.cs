using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //For saving and loading of data
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Cinemachine;

public class GameManager : MonoBehaviour
{

    //Make sure there is only one instance of the game manager
    public static GameManager instance;

    public GameObject player;
    public bool playerIsIdle;

    //Stats for all our characters
    public int lives = 3;
    public int score = 0;
    public float jumpPower;       //Set by player on Awake
    public float originalJumpPower;   //Set by player on Awake

    //Control player movement, ie if there is a menu open stop movement, if we are switching scenes stop movement - etc...
    public bool gameMenuOpen;
    public bool dialogueActive;
    public bool fadingBetweenAreas;

    public bool gameIsOver;  //True if GameOver
    public Canvas theCanvas;
    public TextMeshProUGUI txtLives; //Set in the inspector
    public TextMeshProUGUI txtScore; //Set in the inspector
    public TextMeshProUGUI txtInformation; //Set in the inspector
    public TextMeshProUGUI txtAmmo;

    public Image healthBar;
    private float health = 1;

    private int bullets;

    public float gameSpeed = 1f;
    public float gameSpeedIncrement = 0.1f;
    public float gameSpeedTimeBetweenIncrements = 15f;

    public Vector3 latestCheckPoint;  //Set initally by the player
    public int totalCheckPoints = 0;  //All checkpoints will reference this so that they know their relative place

    private float lightIntensity = 1f;

    private bool levelReset = true;  //False if we are losing a life!

    public Image LifeLostSplash;
    public Sprite AmmoSprite;  //Set this to the player's preferred sprite (assigned in player shoot script)


    private TextMeshProUGUI splashText;

    public bool playerHasDied = false;  //True momentarily when player is dead, read by coins to reset them.

    //SET THIS TO TRUE BEFORE PUBLISHING IT MAKES NEW LEVELS GET CREATED!!!

    public bool playerInvincible = false;
    //Player always starts here, first tile is at 0,0,0 so this moves us up and along a bit.
    public static readonly Vector3 playerStartPosition = new Vector3(5, 8, 0);
    public bool playerJumping = false; //Will be updated by the player script, used by CameraNoJumped.cs

    public GameObject playerScript;
    public int selectedPlayerIndex;  //Set by PlayerSelector.cs, needed if player opts to replay the same level in Highscore.cs

    public bool onStairs = false; //Set to true/false by collider and when True is used to turn off the CameraNoJump.cs script so stair climbing looks smooth

    public bool bossAttack = false;

    public int apples = 0;
    public TextMeshProUGUI txtApples;
    public GameObject applePanel;

    public Vector3 bossDropLocation;  //Used for the Enemy placer so it knows where to put the boss, set by platformgenerator.cs

    private string GameOverTheme, LifeLostTheme;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            //Keep this copy and only this copy
            instance = this;
            //Don't destroy between scenes
            DontDestroyOnLoad(gameObject);
            // Hook into sceneLoaded callback
            SceneManager.sceneLoaded += OnSceneLoaded;
            StartCoroutine(RunAfterFirstUpdate());

        }
        else
        {
            Destroy(gameObject);
        }

    }

    IEnumerator RunAfterFirstUpdate()
    {
        yield return null; // Waits until AFTER the first Update()
        theCanvas.enabled = true; //Make sure the user can see his score
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only act when the main game scene is loaded
        if (scene.name == MyTags.SCENE_MAIN_GAME)
        {
            StartNewGame(); // NOW it can safely reference objects in the new scene
        }
    }

    private void Start()
    {

        player = GameObject.FindWithTag(MyTags.PLAYER_TAG);

        ResetPlayerStats();

        splashText = LifeLostSplash.GetComponentInChildren<TextMeshProUGUI>(); //Find splashscreen
        splashText.enabled = false;
        LifeLostSplash.enabled = false;
        

        SetInitialLighting();

        int gameOver = Random.Range(1, 2);
        if (gameOver == 1)
            GameOverTheme = "GameOver1";
        else GameOverTheme = "GameOver2";

        int lifeLost = Random.Range(1, 3);
        if (lifeLost == 1)
            LifeLostTheme = "Die1";
        else if (lifeLost == 2)
            LifeLostTheme = "Die2";
        else LifeLostTheme = "Die3";


    }


    private void Update() //Delete this
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Other respawn logic
            Vector3 doctor = GameObject.FindWithTag("Doctor").transform.position;
            Transform player = GameObject.FindWithTag("Player").transform;
            doctor.x -= 10;
            player.transform.position = doctor;

        }

    }

    public void ResetAmbience()
    {
        AudioManager.instance.StopAllSounds();
        GameObject.Find("Global Light 2D").GetComponent<Light2D>().intensity = 1f; //Colour white
        //Neutral
        GameObject.Find("Global Light 2D").GetComponent<Light2D>().color = new Color32(255, 255, 255, 255);
    }

    private void SetInitialLighting()
    {
        //Lighting

        lightIntensity = UnityEngine.Random.Range(1.8f, 3f); //Much brighter than boss fight lighting

        //Generate an array of 4 byte values
        System.Random rnd = new System.Random();
        System.Byte[] b = new System.Byte[4];
        rnd.NextBytes(b);


        GameObject.Find("Global Light 2D").GetComponent<Light2D>().intensity = lightIntensity;
        GameObject.Find("Global Light 2D").GetComponent<Light2D>().color = new Color32(b[0], b[1], b[2], b[3]);

    }

    public void SetAmbiance()
    {
        //Lighting

        lightIntensity = UnityEngine.Random.Range(0.5f, 1);

        //Generate an array of 4 byte values
        System.Random rnd = new System.Random();
        System.Byte[] b = new System.Byte[4];
        rnd.NextBytes(b);


        GameObject.Find("Global Light 2D").GetComponent<Light2D>().intensity = lightIntensity;
        GameObject.Find("Global Light 2D").GetComponent<Light2D>().color = new Color32(b[0], b[1], b[2], b[3]);

    }

    private void GameSpeedBoost()
    {
        InvokeRepeating("BoostGameSpeed", gameSpeedTimeBetweenIncrements, gameSpeedTimeBetweenIncrements);



    }

    public void InfoTextDisplay(string text, float timeToDisplay)
    {  //Currently only used by dog
        txtInformation.text = text;
        StartCoroutine(ClearText(timeToDisplay));

    }

    IEnumerator ClearText(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        txtInformation.text = "";
    }


    public void LoseALife()
    {
        playerHasDied = true;
        if (lives > 0)
        {
            AudioManager.instance.Play(MyTags.SOUND_HEAVYTHROW, 1.4f);  //Play with higher pitch
            StartCoroutine(ALifeLost());
        }
        else //We are dead (sad times)
        {
            //Display end scene and highscore
            AudioManager.instance.StopAllSounds();
            lives = 3;
            //No need to store the current score as it is already stored in this script
            theCanvas.gameObject.SetActive(false);
            SceneManager.LoadScene(MyTags.SCENE_HIGHSCORE);

        }
    }

    public void DropHealth(float amountToDropAsValueZeroToOne)
    {
        //Calculate the health
        health = health - amountToDropAsValueZeroToOne;
        //Reduce the healthbar
        healthBar.fillAmount = health;
        //Lose life if required
        if (health <= 0.1)
        {
            LoseALife();
        }

    }

    public void StartNewGame()
    {

        //All this handled on the Awake in PlatformGenerator (set the CreateNewLevel is done from the highscore menu)

        GameObject LevelCreator = GameObject.FindGameObjectWithTag(MyTags.LEVEL_CREATOR_TAG);
        if (LevelCreator != null)
        {
            LevelCreator.GetComponent<PlatformGenerator>().GenerateLevel();
            Debug.Log("Level generated.");
        }
        else
        {
            Debug.LogError("LevelCreator not found!");
        }

        //Reset player stats
        ResetPlayerStats();
    }

    public void ResetPlayerStats()
    {
        player = GameObject.FindGameObjectWithTag(MyTags.PLAYER_TAG);
        if (player != null)
        {
            score = 0;
            lives = 3;
            health = 1;
            txtScore.text = score.ToString();
            healthBar.fillAmount = health;
            txtLives.text = lives.ToString();
            player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

            latestCheckPoint = GameManager.playerStartPosition;  //Record initial start point
            //Find the camera
            var vcam = GameObject.FindWithTag(MyTags.VCAM_TAG).GetComponent<CinemachineVirtualCamera>();
            vcam.Follow = player.transform;
        }
    }

    IEnumerator ALifeLost()
    {
        if (levelReset == true)
        {
            //Re-arm if zero
            if (bullets == 0)
            {
                Ammo(UnityEngine.Random.Range(3, 6));
            }
            player.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            levelReset = false; //Stop this being called over and over
            player.GetComponentInChildren<Animator>().enabled = false; //Stop player moving
            //player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GameObject.Find("Global Light 2D").GetComponent<Light2D>().intensity = 0.2f;


            //Splash Screen
            StartCoroutine(LooseALifeSplashScreenFadeIn());  //Also calls fade out


            yield return new WaitForSeconds(1f);

            health = 1;
            healthBar.fillAmount = health;
            lives--;
            txtLives.text = lives.ToString();
            //Restart scene (needs to change to start at the last checkpoint!)
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
            gameSpeed = 1;
            player.transform.position = latestCheckPoint;
            player.GetComponentInChildren<Animator>().enabled = true; //Stop player moving
            GameObject.Find("Global Light 2D").GetComponent<Light2D>().intensity = lightIntensity;

            levelReset = true;
            if (player.name.Contains("Cave"))
            {
                player.GetComponentInChildren<Stoneage_Movement>().levelReset = true; //Allow player to be killed again
            }
            else if (player.name.Contains("Boat")) //Steam Boat Willy
            {
                player.GetComponentInChildren<SteamBoatWillyMovement>().levelReset = true;
            }
            else
            {
                player.GetComponent<PlayerMovement>().levelReset = true; //Allow player to be killed again
            }

        }
    }

    IEnumerator LooseALifeSplashScreenFadeIn()
    {
        splashText.text = "Lose a Life!";

        //All this seems over the top, but to develop the game the images must be enabled and have the transparency set to zero
        //If they are not active when the game starts it causes problems so they are active and invisible
        //Recativation
        LifeLostSplash.enabled = true;
        splashText.enabled = true;

        //Make visible
        LifeLostSplash.color = new Color(1, 1, 1, 1);
        splashText.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(2f);
        StartCoroutine(LooseALifeSplashScreenFadeOut());

        //Fading code
        //for (float i = 0; i <= 1; i += Time.deltaTime)
        //{   // set color with i as alpha
        //    LifeLostSplash.color = new Color(1, 1, 1, i);
        //    splashText.color = new Color(1, 1, 1, i);
        //    yield return null;
        //}
    }
    //LifeLostSplash.enabled = false;

    IEnumerator LooseALifeSplashScreenFadeOut()
    {
        if (lives > 1)
        {
            AudioManager.instance.Play(LifeLostTheme);
            splashText.text = lives + " lives left";
        }
        else if (lives == 1)
        {
            AudioManager.instance.Play(LifeLostTheme);
            splashText.text = lives + " life left";
        }
        else if (lives == 0)
        {
            AudioManager.instance.Play(LifeLostTheme);
            splashText.text = "No lives left! \n be careful.";
        }
        else
        {

            splashText.text = "You're dead!";

            //Reset the Apples panel, we don't want to see that for a while
            applePanel.SetActive(false);
        }
        LifeLostSplash.enabled = true;
        yield return new WaitForSeconds(1f);
        LifeLostSplash.enabled = false;
        splashText.enabled = false;
        player.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;  //Enable player to move again
        playerHasDied = false; //Reset as player is now born again, fresh and new!
    }

    public void Ammo(int ammo)
    {
        bullets += ammo;
        txtAmmo.text = bullets.ToString();
    }

    public int GetBullets()
    {
        return bullets;
    }
    public void AddALife(int livesToAdd)
    {

        lives = lives + livesToAdd;
        txtLives.text = lives.ToString();

    }

    public void Points(int pointsToAward)  //Can take a negative value
    {

        score = score + pointsToAward;
        txtScore.text = score.ToString();


    }


    public void alterJumpPower(int increment)  //Always use this to set the jump power
    {

        jumpPower = increment;

        if (player.name.Contains("Cave") || (player.name.Contains("Police"))) //Caveman or policeman
        {
            player.GetComponentInChildren<Stoneage_Movement>().setJumpPower(jumpPower, "GameManager");
        }
        else if (player.name.Contains("Boat")) //Steam Boat Willy
        {
            player.GetComponentInChildren<SteamBoatWillyMovement>().setJumpPower(jumpPower, "GameManager");
        }
        else if (player.name.Contains("Gordo"))  //Gordo
        {
            player.GetComponentInChildren<GordoMovement>().setJumpPower(jumpPower, "GameManager");
        }
        else
        {
            player.GetComponent<PlayerMovement>().setJumpPower(jumpPower, "GameManager");
        }
    }

    public void resetJumpPower()
    {
        jumpPower = originalJumpPower;
        if (jumpPower < 5)
        {
            print("Jump power issue, jump power is " + jumpPower);
        }

        // Check if the player object is valid
        if (player == null)
        {
            Debug.LogError("Player object is null in GameManager!");
            return;
        }

        // Handle different player types based on their names
        if (player.name.Contains("Cave") || player.name.Contains("Police"))
        {
            var movement = player.GetComponentInChildren<Stoneage_Movement>();
            if (movement != null)
            {
                movement.setJumpPower(jumpPower, "GameManager");
            }
            else
            {
                Debug.LogError("Stoneage_Movement component not found on player!");
            }
        }
        else if (player.name.Contains("Boat")) //Steam Boat Willy
        {
            var movement = player.GetComponentInChildren<SteamBoatWillyMovement>();
            if (movement != null)
            {
                movement.setJumpPower(jumpPower, "GameManager");
            }
            else
            {
                Debug.LogError("SteamBoatWillyMovement component not found on player!");
            }
        }
        else if (player.name.Contains("Gordo")) //Gordo
        {
            var movement = player.GetComponentInChildren<GordoMovement>();
            if (movement != null)
            {
                movement.setJumpPower(jumpPower, "GameManager");
            }
            else
            {
                Debug.LogError("GordoMovement component not found on player!");
            }
        }
        else
        {
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.setJumpPower(jumpPower, "GameManager");
            }
            else
            {
                Debug.LogError("PlayerMovement component not found on player!");
            }
        }
    }

    void BoostGameSpeed()
    {
        //Turned off because it is a pain in the backside!
        //print("Boosting game Speed " + Time.deltaTime);
        //gameSpeed = gameSpeed + gameSpeedIncrement;
        //Time.timeScale = gameSpeed;

    }





}
