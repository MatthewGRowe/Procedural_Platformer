using System.Collections;
using UnityEngine;

public class SteamBoatWillyMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D myBody;
    private Animator anim;
    public static PlayerMovement instance;

    public Transform groundCheckPosition;
    public LayerMask groundLayer;

    private bool isGrounded;
    private bool jumped;
    public float fallMultiplier = 2.5f;
    public float jumpPower = 9f;
    public bool levelReset = true;
    public bool falling = false;
    private float fallingThreshold = 13;
    private float lastY = 0;

    private bool checkTune = false;
    private bool _isTunePlaying = false;
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int TuneHash = Animator.StringToHash("Tune");

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastY = transform.position.y;
    }

    void Start()
    {
        GameManager.instance.ResetPlayerStats();
        GameManager.instance.player = this.gameObject;
        GameManager.instance.jumpPower = jumpPower;
        GameManager.instance.originalJumpPower = jumpPower;
        GameManager.instance.latestCheckPoint = transform.position;
        GameManager.instance.Ammo(5);
        transform.position = GameManager.instance.latestCheckPoint;
    }

    void Update()
    {
        CheckIfGrounded();
        if (!jumped && Input.GetKey(KeyCode.Space))
        {
            PlayerJump();
        }
        CheckIfFalling();
    }

    void FixedUpdate()
    {
        PlayerWalk();
    }

    void PlayerWalk()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if (myBody.bodyType == RigidbodyType2D.Dynamic)
        {
            if (h > 0)
            {
                myBody.velocity = new Vector2(speed, myBody.velocity.y);
                ChangeDirection(1);
                GameManager.instance.playerIsIdle = false;
                anim.SetBool(RunHash, true);
                anim.SetBool(IdleHash, false);
                StopTune();
            }
            else if (h < 0)
            {
                myBody.velocity = new Vector2(-speed, myBody.velocity.y);
                ChangeDirection(-1);
                GameManager.instance.playerIsIdle = false;
                anim.SetBool(RunHash, true);
                anim.SetBool(IdleHash, false);
                StopTune();
            }
            else if (h == 0)
            {
                myBody.velocity = new Vector2(0f, myBody.velocity.y);
                GameManager.instance.playerIsIdle = true;
                anim.SetBool(RunHash, false);
                anim.SetBool(IdleHash, true);
                if (!checkTune && !_isTunePlaying)
                {
                    checkTune = true;
                    StartCoroutine(PlayATune());
                }
            }
        }
    }

    IEnumerator PlayATune()
    {
        float idleTime = 0f;
        const float requiredIdleTime = 10f;

        while (idleTime < requiredIdleTime && !_isTunePlaying)
        {
            if (anim.GetBool(IdleHash))
            {
                idleTime += Time.deltaTime;
            }
            else
            {
                checkTune = false;
                yield break;
            }
            yield return null;
        }

        if (idleTime >= requiredIdleTime)
        {
            _isTunePlaying = true;
            anim.SetBool(TuneHash, true);
            AudioManager.instance.Play(MyTags.SOUND_MICKEYMOUSE);
            yield return new WaitForSeconds(3f);
            anim.SetBool(TuneHash, false);
            _isTunePlaying = false;
            checkTune = false;
        }
    }

    void StopTune()
    {
        if (_isTunePlaying)
        {
            AudioManager.instance.StopPlay(MyTags.SOUND_MICKEYMOUSE);
            anim.SetBool(TuneHash, false);
        }
        _isTunePlaying = false;
        checkTune = false;
        StopAllCoroutines();
    }

    void CheckIfFalling()
    {
        if (myBody.velocity.y < -fallingThreshold)
        {
            falling = true;
        }
        else
        {
            falling = false;
        }

        if (falling && myBody.position.y < -8 && levelReset)
        {
            levelReset = false;
            GameManager.instance.LoseALife();
        }

        if (falling && myBody.position.y < -400f)
        {
            levelReset = true;
        }
    }

    void CheckIfGrounded()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector2.down, 0.1f, groundLayer);
        isGrounded = groundHit.collider != null;

        if (isGrounded)
        {
            if (jumped)
            {
                jumped = false;
                myBody.gravityScale = 1;
                anim.SetBool(JumpHash, false);
            }

            string groundTag = groundHit.collider.tag;
            GameManager.instance.onStairs = groundTag == MyTags.SECRET_TAG;
        }
    }

    void PlayerJump()
    {
        jumped = true;
        AudioManager.instance.Play(MyTags.SOUND_JUMP);
        myBody.velocity = new Vector2(myBody.velocity.x, jumpPower);
        anim.SetBool(JumpHash, true);
    }

    void ChangeDirection(int direction)
    {
        Vector3 tempScale = transform.localScale;
        tempScale.x = direction;
        transform.localScale = tempScale;
    }

    public void setJumpPower(float newPower, string sender)
    {
        if (sender == "GameManager")
        {
            jumpPower = newPower;
        }
        else
        {
            Debug.LogError("Cannot call setJumpPower from anywhere but GameManager!");
        }
    }
}