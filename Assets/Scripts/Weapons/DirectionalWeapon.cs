using UnityEngine;
using System.Collections;

public class DirectionalWeapon : Weapon
{
    [Header("Projectile & Timing")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float        burstDelay = 0.07f;

    [Header("Fire Conditions")]
    [SerializeField] private float activationRange = 8f;

    private float spawnCounter;

    void Update()
    {
        if (!EnemyInRange()) return;

        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0f)
        {
            spawnCounter = stats[weaponLevel].cooldown;
            StartCoroutine(SpawnBurst());
        }
    }

    private IEnumerator SpawnBurst()
    {
        int shots = Mathf.Max(1, stats[weaponLevel].amount);

        for (int i = 0; i < shots; i++)
        {
            Instantiate(prefab, transform.position, transform.rotation);
            yield return new WaitForSeconds(burstDelay);
        }
    }

    private bool EnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3      myPos   = transform.position;

        foreach (GameObject e in enemies)
        {
            if (Vector3.Distance(myPos, e.transform.position) <= activationRange)
                return true;
        }
        return false;
    }
}