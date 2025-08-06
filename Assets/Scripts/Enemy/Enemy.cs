using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Unity refs")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Base stats (scaled by spawner)")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float health = 20f;
    private float baseHealth;

    void Start()
    {
        baseHealth = health;
    }

    [SerializeField] private int experienceToGive = 1;

    [Header("On-hit behaviour")]
    [SerializeField] private float pushTime = .25f;
    [SerializeField] private GameObject destroyEffect;

    private Vector3 direction;
    private float pushCounter;

    public void ScaleStats(float healthMul, float speedMul, float damageMul)
    {
        health *= healthMul;
        moveSpeed *= speedMul;
        damage *= damageMul;
    }

    void FixedUpdate()
    {
        if (!PlayerController.Instance.gameObject.activeSelf)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        spriteRenderer.flipX = PlayerController.Instance.transform.position.x > transform.position.x;

        if (pushCounter > 0f)
        {
            pushCounter -= Time.deltaTime;
            if (moveSpeed > 0f) moveSpeed = -moveSpeed;
            if (pushCounter <= 0f) moveSpeed = Mathf.Abs(moveSpeed);
        }

        direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
            PlayerController.Instance.TakeDamage(damage);
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        DamageNumberController.Instance.CreateNumber(dmg, transform.position);
        pushCounter = pushTime;

        var agent = PlayerController.Instance.GetComponent<CrimsonPact>();

        if (agent != null)
            agent.GiveReward(+0.3f);
            agent.rewardFromEnemyDamage += 0.3f;
        if (health > 0f) return;

        Instantiate(destroyEffect, transform.position, transform.rotation);
        PlayerController.Instance.GetExperience(experienceToGive);
        AudioController.Instance.PlayModifiedSound(AudioController.Instance.enemyDie);

        if (agent != null)
            agent.GiveReward(+1.5f);
        agent.rewardFromEnemyDamage += 1.5f;
        Destroy(gameObject);
    }
    public float GetHealthNormalized()
    {
    return baseHealth > 0f ? health / baseHealth : 0f;
    }
}