using UnityEngine;

public class ThunderStrike : MonoBehaviour
{
    [Header("Optional visual prefab (leave null to scale root)")]
    [SerializeField] private GameObject strikeEffect;

    private ThunderWeapon weapon;

    void Start()
    {
        weapon = FindObjectOfType<ThunderWeapon>();

        float damage = weapon.stats[weapon.weaponLevel].damage;
        float radius = weapon.stats[weapon.weaponLevel].range;

        if (AudioController.Instance?.lightningWeapon != null)
            AudioController.Instance.PlaySound(AudioController.Instance.lightningWeapon);

        if (strikeEffect != null)
        {
            GameObject vfx = Instantiate(strikeEffect, transform.position, Quaternion.identity);
            vfx.transform.localScale = Vector3.one * radius;
        }
        else
        {
            transform.localScale = Vector3.one * radius;
        }

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject,0.5f);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (weapon != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, weapon.stats[weapon.weaponLevel].range);
        }
    }
#endif
}