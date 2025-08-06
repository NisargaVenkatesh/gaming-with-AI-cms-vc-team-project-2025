using UnityEngine;

public class PiercingWeaponProjectile : MonoBehaviour
{
    private PiercingWeapon weapon;
    private Rigidbody2D rb;
    private TrailRenderer trail;
    private float duration;

    private const float SPREAD = 0.2f;

    void Start()
    {
        weapon = GameObject.FindObjectOfType<PiercingWeapon>();
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        duration = weapon.stats[weapon.weaponLevel].duration;

        Vector3 from = transform.position;
        Vector3 fallback = PlayerController.Instance.lastMoveDirection;
        Vector3 direction = GetDirectionToClosestEnemy(from, fallback);

        float spread = Random.Range(-SPREAD, SPREAD);
        Vector2 velocity = new Vector2(
            direction.x * weapon.stats[weapon.weaponLevel].speed + spread,
            direction.y * weapon.stats[weapon.weaponLevel].speed + spread);

        rb.linearVelocity = velocity;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (AudioController.Instance.piercingWeaponSpawn != null)
            AudioController.Instance.PlaySound(AudioController.Instance.piercingWeaponSpawn);

        Invoke(nameof(DestroySelf), duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(weapon.stats[weapon.weaponLevel].damage);

            if (AudioController.Instance.piercingWeaponHit != null)
                AudioController.Instance.PlaySound(AudioController.Instance.piercingWeaponHit);
        }
    }

    private void DestroySelf()
    {
        if (trail != null)
        {
            trail.autodestruct = true;
            trail.transform.parent = null;
        }

        Destroy(gameObject);
    }

    private static Vector3 GetDirectionToClosestEnemy(Vector3 from, Vector3 fallback)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float bestDst = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            float d = Vector3.Distance(from, e.transform.position);
            if (d < bestDst)
            {
                bestDst = d;
                closest = e;
            }
        }

        if (closest != null)
        {
            Vector3 targetPos = closest.transform.position + new Vector3(0f, 0.5f, 0f);
            return (targetPos - from).normalized;
        }

        return fallback.normalized;
    }
}