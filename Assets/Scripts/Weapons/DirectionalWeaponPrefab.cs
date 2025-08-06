using UnityEngine;

public class DirectionalWeaponPrefab : MonoBehaviour
{
    [Header("Visual FX")]
    [SerializeField] private GameObject destroyEffect;

    private DirectionalWeapon weapon;
    private Rigidbody2D       rb;
    private Collider2D        col;
    private TrailRenderer     trail;
    private bool              destroyed = false;

    private const float SPREAD = 0.2f;

    void Start()
    {
        weapon = FindObjectOfType<DirectionalWeapon>();
        rb     = GetComponent<Rigidbody2D>();
        col    = GetComponent<Collider2D>();
        trail  = GetComponent<TrailRenderer>();

        float lifetime = weapon.stats[weapon.weaponLevel].duration;
        Invoke(nameof(DestroyProjectile), lifetime);

        Vector3 dir = GetDirectionToClosestEnemy(
            transform.position,
            PlayerController.Instance.lastMoveDirection);

        float spread = Random.Range(-SPREAD, SPREAD);
        Vector2 vel  = new Vector2(
            dir.x * weapon.stats[weapon.weaponLevel].speed + spread,
            dir.y * weapon.stats[weapon.weaponLevel].speed + spread);

        rb.linearVelocity = vel;

        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg + 180f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        AudioController.Instance.PlaySound(
            AudioController.Instance.directionalWeaponSpawn);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (destroyed || !other.CompareTag("Enemy")) return;

        Enemy e = other.GetComponent<Enemy>();
        if (e == null) return;

        e.TakeDamage(weapon.stats[weapon.weaponLevel].damage);
        AudioController.Instance.PlaySound(
            AudioController.Instance.directionalWeaponHit);

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (destroyed) return;
        destroyed = true;

        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, transform.rotation);

        if (trail != null)
        {
            trail.autodestruct = true;
            trail.transform.parent = null;
        }

        if (rb != null)  rb.simulated = false;
        if (col != null) col.enabled  = false;

        gameObject.SetActive(false);
    }

    private static Vector3 GetDirectionToClosestEnemy(Vector3 from, Vector3 fallback)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject   closest = null;
        float        bestDst = Mathf.Infinity;

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