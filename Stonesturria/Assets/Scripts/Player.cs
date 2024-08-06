using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Rigidbody2D playerRigidBody;
    public CapsuleCollider2D playerCollider;
    public Transform playerFoot;
    public Transform playerFootLeft;
    public Transform playerFootRight;
    public Animator playerAnimator;
    public Animator doorAnimator;
    Vector2 moveInput;
    public LayerMask ground;

    [Header("Running")]
    private float moveSpeed;
    public float runSpeed = 3f;

    [Header("Jumping")]
    public float jumpHeight = 1f;
    public float detectionLength = 1f;
    public float gravity = -5f;

    [Header("Falling")]
    private float fallingHorizontalVelocityBoost = 0;

    [Header("Attack")]
    public float AttackDelay = 0.5f;
    public GameObject fireball;
    public Transform magicWand;

    [Header("Current Checks")]
    public bool canMove = true;
    private bool isAlive = true;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isAttacking = false;

    [Header("GameManaging")]
    private bool hasKey;
    public int health = 3;
    public Transform respawnPoint;

    [Header("UI")]
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Sprite emptyHeart;
    public Image key;
    public Sprite fullKey;
    public TextMeshProUGUI tmp;


    //Animations
    const string PlayerIdle = "PlayerIdle";
    const string PlayerRun = "PlayerRun";
    const string PlayerJump = "PlayerJump";
    const string PlayerFalling = "PlayerFalling";
    const string PlayerDoubleJump = "PlayerDoubleJump";
    const string PlayerAttack = "PlayerAttack";
    const string PlayerDie = "PlayerDie";
    const string PlayerRevive = "PlayerRevive";

    private bool onGround = false;
    private bool canDoubleJump = false;

    void Start()
    {
        ground = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        OnGround();
        Move();
        Jumping();
        Falling();
        FlipSprite();
        Idle();
        Hazards();
        Health();
        Keys();
    }
    void OnGround()
    {
        RaycastHit2D hit, hit2, hit3;
        hit = Physics2D.Raycast(playerFoot.position, Vector2.down, detectionLength, ground);
        hit2 = Physics2D.Raycast(playerFootLeft.position, Vector2.down, detectionLength, ground);
        hit3 = Physics2D.Raycast(playerFootRight.position, Vector2.down, detectionLength, ground);
        Debug.DrawRay(playerFoot.position, Vector2.down * detectionLength, Color.red);
        Debug.DrawRay(playerFootLeft.position, Vector2.down * detectionLength, Color.red);
        Debug.DrawRay(playerFootRight.position, Vector2.down * detectionLength, Color.red);

        if (hit || hit2 || hit3)
        {
            onGround = true;
            canDoubleJump = true;
        }
        else
        {
            onGround = false;
        }

    }
    void Hazards()
    {
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")) && isAlive)
        {
            isAlive = false;
            playerAnimator.Play(PlayerDie);
            playerCollider.isTrigger = true;
            playerRigidBody.bodyType = RigidbodyType2D.Static;
            playerRigidBody.velocity = new Vector2(0, 0);
            Invoke("Respawn", 2);
            isJumping = false;
            isFalling = false;
            isAttacking = false;
            isRunning = false;

        }
    }
    void Respawn()
    {
        if (health > 1)
        {
            playerCollider.isTrigger = false;
            playerRigidBody.bodyType = RigidbodyType2D.Dynamic;
            transform.position = respawnPoint.position;
            --health;
            playerAnimator.Play(PlayerRevive);
        }
        else
        {
            SceneManager.LoadScene(7);
        }
    }
    void Alive()
    {
        isAlive = true;
    }
    void Idle()
    {
        if (!isRunning && !isJumping && !isFalling && !isAttacking && isAlive)
        {
            playerAnimator.Play(PlayerIdle);
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Move()
    {
        if (isAlive && !isAttacking && canMove)
        {
            moveSpeed = moveInput.x * (runSpeed + fallingHorizontalVelocityBoost);
            playerRigidBody.velocity = new Vector2(moveSpeed, playerRigidBody.velocity.y); ;
        }
        if (Mathf.Abs(moveSpeed) > Mathf.Epsilon && onGround && !isAttacking && isAlive)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
        if (isRunning)
        {
            playerAnimator.Play(PlayerRun);
        }
    }
    void OnJump()
    {
        if (onGround && !isAttacking && !isFalling && isAlive && canMove)
        {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpHeight);
        }
        else if (!onGround && !isAttacking && canDoubleJump && isAlive)
        {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpHeight + (jumpHeight / 8));
            canDoubleJump = false;
            playerAnimator.Play(PlayerDoubleJump);
        }
    }
    void Jumping()
    {
        if (!onGround && playerRigidBody.velocity.y > 0 && !isAttacking && isAlive)
        {
            isJumping = true;

        }
        else
        {
            isJumping = false;
        }

        if (isJumping)
        {
            playerAnimator.Play(PlayerJump);
        }
    }

    void Falling()
    {
        if (!onGround && playerRigidBody.velocity.y < Mathf.Epsilon && !isAttacking && isAlive)
        {
            isFalling = true;
            playerRigidBody.gravityScale = 5;
            fallingHorizontalVelocityBoost = runSpeed / 4;
        }
        else
        {
            isFalling = false;
            fallingHorizontalVelocityBoost = 0;
            playerRigidBody.gravityScale = 2.7f;
        }
        if (isFalling)
        {
            playerAnimator.Play(PlayerFalling);
        }
    }

    void FlipSprite()
    {
        if (moveSpeed < 0f)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (moveSpeed > 0f)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    void OnAttack()
    {
        if (isAlive && !isAttacking && onGround && !isJumping && !isFalling)
        {
            playerRigidBody.velocity = new Vector2(0, 0);
            isAttacking = true;
            playerAnimator.Play(PlayerAttack);
        }
    }
    void ShootFireball()
    {
        Instantiate(fireball, magicWand.position, transform.rotation);
    }
    void DoneAttacking()
    {
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (playerCollider.IsTouchingLayers(ground) && isJumping)
        {
            playerRigidBody.velocity = new Vector2(0, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Door") && hasKey)
        {
            doorAnimator.Play("Open");
            Invoke("LoadNextLevel", 1.5f);
        }
        if (other.CompareTag("Checkpoint"))
        {
            respawnPoint.position = other.transform.position;
        }
    }
    void LoadNextLevel()
    {
        int currentScene;
        currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(++currentScene);
    }

    void Health()
    {
        if (health <= 2) heart3.sprite = emptyHeart;
        if (health <= 1) heart2.sprite = emptyHeart;
        if (health <= 0) heart1.sprite = emptyHeart;
    }
    void Keys()
    {
        if (hasKey)

        {
            key.sprite = fullKey;
            tmp.SetText("Unlock the Door");
        }
    }
}
