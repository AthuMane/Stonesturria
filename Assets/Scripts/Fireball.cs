using UnityEngine;

public class Fireball : MonoBehaviour
{
    private bool hit = false;
    public float speed = 5;
    public Animator fireballAnimator;
    public Rigidbody2D fireballRigidBody;
    Player player;
    float xspeed;

    void Start()
    {
        player = FindObjectOfType<Player>();
        xspeed = player.transform.localScale.x * speed;
    }
    void Update()
    {
        FlipSprite();
        if (!hit)
        {
            fireballRigidBody.velocity = new Vector2(xspeed, 0);
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "EnemyPatrol" && other.tag != "Door" && other.tag != "Key" && other.tag != "Player")
        {
            hit = true;
            fireballRigidBody.velocity = new Vector2(0, 0);
            fireballAnimator.Play("Explode");
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
    void FlipSprite()
    {
        if (xspeed < 0f)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (xspeed > 0f)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }
}
