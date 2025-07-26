using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    public LayerMask playerLayer;  // Allows you to choose which layer the player is on
    public TMP_Text myText;  // For displaying motivational messages
    public GameObject textBackground; // The background for the text
    private Vector3 offSet = new Vector3(10, -1.35f, 0);
    public int gapLength = 0;
    public LayerMask groundLayer;
    public int myGapLength = 0;
    private bool pointsAwarded = false;  // Make sure we don't award twice!
    private Rigidbody2D myBody;
    
    private List<string> motivationalMessages = new List<string>();
    public SpriteRenderer coinImage; // Make sure we turn this off so it disappears on collision
    private Vector3 originalPosition;
    private void Awake()
    {
        originalPosition = transform.position;
        myBody = GetComponent<Rigidbody2D>();
        myBody.bodyType = RigidbodyType2D.Static;
        
        InitializeMotivationalMessages();
    }

    private void InitializeMotivationalMessages()
    {
        motivationalMessages.Add("Keep going");
        motivationalMessages.Add("Do a big ole jump!");
        motivationalMessages.Add("A cat's ear has 32 muscles");
        motivationalMessages.Add("Sudan has many pyramids");
        motivationalMessages.Add("Bumblebee bats exist!");
        motivationalMessages.Add("Lemons float, limes sink");
        motivationalMessages.Add("The first oranges weren’t orange");
        motivationalMessages.Add("A cow-bison hybrid is a beefalo!");
        motivationalMessages.Add("Scotland has 421 words for snow");
        motivationalMessages.Add("The windy city isn't windy!");
        motivationalMessages.Add("Armadillo shells are bulletproof");
        motivationalMessages.Add("Firefighters can make water wetter");
        motivationalMessages.Add("Numbers <1000 don't contain 'a'");
        motivationalMessages.Add("Greeks invented Christmas cards");
        motivationalMessages.Add("Roosevelt had a pet hyena");
        motivationalMessages.Add("Rats have dreams");
        motivationalMessages.Add("The Terminator script was sold for $1");
        motivationalMessages.Add("Abraham Lincoln was a bartender");
        motivationalMessages.Add("Beethoven was bad at maths");
        motivationalMessages.Add("What did Yoda say when he saw himself in 4k?\nHDMI");
        motivationalMessages.Add("What is a room with no walls? A mushroom");
        motivationalMessages.Add("Bananas are berries, but strawberries aren't!");
        motivationalMessages.Add("Octopuses have three hearts!");
        motivationalMessages.Add("Wombat poop is cube-shaped!");
        motivationalMessages.Add("The Eiffel Tower can grow taller in summer!");
        motivationalMessages.Add("Honey never spoils!");
        motivationalMessages.Add("Polar bears have black skin!");
        motivationalMessages.Add("The heart of a shrimp is in its head!");
        motivationalMessages.Add("A day on Venus is longer than a year on Venus!");
        motivationalMessages.Add("Cows have best friends!");
        motivationalMessages.Add("The dot over the letter 'i' is called a tittle!");
        motivationalMessages.Add("Sloths can hold their breath longer than dolphins!");
        motivationalMessages.Add("The inventor of the Pringles can is buried in one!");
        motivationalMessages.Add("Peanuts aren’t nuts—they’re legumes!");
        motivationalMessages.Add("The longest hiccuping spree lasted 68 years!");
        motivationalMessages.Add("A group of flamingos is called a flamboyance!");
        motivationalMessages.Add("The first computer mouse was made of wood!");
        motivationalMessages.Add("Kangaroos can’t walk backward!");
        motivationalMessages.Add("The world’s smallest reptile fits on your fingertip!");
        motivationalMessages.Add("A cloud can weigh over a million pounds!");
        motivationalMessages.Add("The first video game was created in 1958!");
        motivationalMessages.Add("The longest recorded flight of a chicken is 13 seconds!");
        motivationalMessages.Add("The Hawaiian alphabet has only 12 letters!");
        motivationalMessages.Add("A shrimp’s heart is in its head!");
        motivationalMessages.Add("The first oranges weren’t orange—they were green!");
        motivationalMessages.Add("The longest wedding veil was longer than 63 football fields!");
        motivationalMessages.Add("A group of crows is called a murder!");
        motivationalMessages.Add("The first product with a barcode was chewing gum!");
        motivationalMessages.Add("The longest time between twins being born is 87 days!");
        motivationalMessages.Add("The first alarm clock could only ring at 4 a.m.!");
        motivationalMessages.Add("The world’s oldest piece of chewing gum is 9,000 years old!");
        motivationalMessages.Add("The longest recorded flight of a paper airplane is 27.6 seconds!");
        motivationalMessages.Add("The first-ever YouTube video was uploaded in 2005!");
        motivationalMessages.Add("The world’s largest snowflake was 15 inches wide!");
        motivationalMessages.Add("The first-ever email was sent in 1971!");
        motivationalMessages.Add("The world’s smallest mammal is the bumblebee bat!");
        motivationalMessages.Add("The first-ever photograph was taken in 1826!");
        motivationalMessages.Add("The world’s largest pizza was 122 feet in diameter!");
        motivationalMessages.Add("The first-ever text message was sent in 1992!");
        motivationalMessages.Add("The world’s longest mustache was over 14 feet long!");
        motivationalMessages.Add("The first-ever tweet was sent in 2006!");
        motivationalMessages.Add("The world’s largest rubber band ball weighs over 9,000 pounds!");
        motivationalMessages.Add("The first-ever emoji was created in 1999!");
        motivationalMessages.Add("The world’s largest chocolate bar weighed over 12,000 pounds!");
        motivationalMessages.Add("The first-ever website is still online!");
        motivationalMessages.Add("The world’s largest jigsaw puzzle had over 551,232 pieces!");
        motivationalMessages.Add("The first-ever Instagram photo was uploaded in 2010!");
        motivationalMessages.Add("The world’s largest Lego tower was over 114 feet tall!");
        motivationalMessages.Add("The first-ever Facebook profile was created in 2004!");
        motivationalMessages.Add("The world’s largest yo-yo was over 11 feet tall!");
        motivationalMessages.Add("The first-ever text message said 'Merry Christmas'!");
        motivationalMessages.Add("The world’s largest rubber duck is over 54 feet tall!");
        motivationalMessages.Add("The first-ever emoji was a smiley face!");
        motivationalMessages.Add("The world’s largest snowman was over 122 feet tall!");
        motivationalMessages.Add("The first-ever hashtag was used in 2007!");
        motivationalMessages.Add("The world’s largest lollipop weighed over 7,000 pounds!");
        motivationalMessages.Add("The first-ever GIF was created in 1987!");
        motivationalMessages.Add("The world’s largest cupcake weighed over 1,200 pounds!");
        motivationalMessages.Add("The first-ever meme was created in 1996!");
        motivationalMessages.Add("The world’s largest ice cream cake weighed over 12,000 pounds!");
        motivationalMessages.Add("The first-ever YouTube video was 18 seconds long!");
        motivationalMessages.Add("The world’s largest pizza was made in 2012!");
        motivationalMessages.Add("The first-ever text message was sent in 1992!");
        motivationalMessages.Add("The world’s largest rubber band ball was made in 2008!");
        motivationalMessages.Add("The first-ever emoji was created in 1999!");
        motivationalMessages.Add("The world’s largest jigsaw puzzle was completed in 2011!");
        motivationalMessages.Add("The first-ever Instagram photo was uploaded in 2010!");
        motivationalMessages.Add("The world’s largest Lego tower was built in 2015!");
        motivationalMessages.Add("The first-ever Facebook profile was created in 2004!");
        motivationalMessages.Add("The world’s largest yo-yo was made in 2012!");
        motivationalMessages.Add("The first-ever text message said 'Merry Christmas'!");
        motivationalMessages.Add("The world’s largest rubber duck was made in 2014!");
        motivationalMessages.Add("The first-ever emoji was a smiley face!");
        motivationalMessages.Add("The world’s largest snowman was built in 2008!");
        motivationalMessages.Add("The first-ever hashtag was used in 2007!");
        motivationalMessages.Add("The world’s largest lollipop was made in 2012!");
        motivationalMessages.Add("The first-ever GIF was created in 1987!");
        motivationalMessages.Add("The world’s largest cupcake was made in 2011!");
        motivationalMessages.Add("The first-ever meme was created in 1996!");
        motivationalMessages.Add("The world’s largest ice cream cake was made in 2011!");
    }

    private void Update()
    {
        // Coin will start by falling, when it gets a distance above the floor it will stop falling
        //if (Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundLayer))
        //{
        //    //myBody.bodyType = RigidbodyType2D.Static;
        //    myBody.gravityScale = 0;
        //    landed = true;
        //}
        //else if (myBody.velocity.y == 0 && !landed)  // We are stuck on something
        //{
            //myBody.velocity = new Vector2(myBody.velocity.x, 20);
        //}

        //if (GameManager.instance.playerHasDied)
        //{
        //    RaceToStartPosition(); // Push coin quickly back to earth
        //}
    }

    private void RaceToStartPosition()
    {
        // Used after a player dies.
        myBody.gravityScale = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(MyTags.PLAYER_TAG) && !pointsAwarded)
        {
            pointsAwarded = true; // Set the flag immediately to prevent multiple triggers
            GetComponent<CircleCollider2D>().enabled = false; // Disable the collider immediately

            textBackground.gameObject.SetActive(true);
            AudioManager.instance.Play(MyTags.SOUND_COIN);

            if (gapLength > 5) // There is a big gap (set by the ObjectPlacer.cs script)
            {
                coinImage.gameObject.SetActive(false); // Turn off renderer

                if (!(GameManager.instance.jumpPower > gapLength + 2)) // Make sure we have enough jump power
                {
                
                    myText.text = "TURBO JUMP!";
               
                    GameManager.instance.alterJumpPower(gapLength + 2);
                
                    StartCoroutine(HideCoin()); // Move the coin for reuse
                    
                }
                else
                {
                    myText.text = motivationalMessages[Random.Range(0, motivationalMessages.Count)];
                    StartCoroutine(DestroyCoin());
                }

                
            }
            else
            {
                int rnd = Random.Range(0, 25);

                if (rnd <= 20)
                {
                    myText.text = "+" + rnd.ToString();
                    GameManager.instance.Points(rnd);
                }
                else
                {
                    myText.text = "Extra Life!";
                    GameManager.instance.AddALife(1);
                }

                StartCoroutine(DestroyCoin());
            }
        }
    }

    IEnumerator DestroyCoin()
    {
        transform.Find("BoostThings").gameObject.SetActive(false); // Turn off visibility
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    IEnumerator HideCoin()
    {
        // Turn off visibility and disable the coin
        transform.Find("BoostThings").gameObject.SetActive(false); // Turn off visibility
        coinImage.gameObject.SetActive(false); // Hide the coin sprite
        GetComponent<CircleCollider2D>().enabled = false; // Disable the collider

        
        yield return new WaitForSeconds(5);


        // Hide the message and clear the text
        textBackground.gameObject.SetActive(false);
        myText.text = "";
       
        // Reset the coin's state
        pointsAwarded = false; // Reset the points awarded flag
      
        GameManager.instance.resetJumpPower(); // Reset jump power
     
        // Reactivate the coin
        transform.position = originalPosition; // Reset to the original position
        coinImage.gameObject.SetActive(true); // Make the coin visible
        GetComponent<CircleCollider2D>().enabled = true; // Re-enable the collider
        transform.Find("BoostThings").gameObject.SetActive(true); // Turn on visibility

    }
}