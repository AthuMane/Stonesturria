using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Speed = 1f;
    float MoveSpeed;
    float MoveDirection = 1;
    public Rigidbody2D enemyRigidBody;
    public Animator enemyAnimator;
    public Transform enemyTransform;
    public Transform enemyMouth;
    public GameObject fireball;
    private bool isAlive = true;

    void Start()
    {
        MoveSpeed = Speed;
    }
    void Update()
    {
        if (isAlive) { Running(); }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyPatrol")
        {
            MoveSpeed = 0;
            enemyAnimator.SetTrigger("Attacking");
        }
        if (other.tag == "PlayerFireball")
        {
            isAlive = false;
            enemyRigidBody.velocity = new Vector2(0, 0);
            enemyAnimator.SetTrigger("EnemyDeath");
        }

    }

    void Running()
    {
        enemyRigidBody.velocity = new Vector2(MoveSpeed * MoveDirection, 0);
    }

    void Flip()
    {
        enemyTransform.localScale = new(enemyTransform.localScale.x * -1, enemyTransform.localScale.y, enemyTransform.localScale.z);
        MoveDirection = -MoveDirection;
        MoveSpeed = Speed;
    }
    void ShootFireball()
    {
        Instantiate(fireball, enemyMouth.position, transform.rotation);
    }
    void NotAttacking()
    {
        Flip();
    }
    void DestroyEnemy()
    {
        Destroy(gameObject);
    }

}


