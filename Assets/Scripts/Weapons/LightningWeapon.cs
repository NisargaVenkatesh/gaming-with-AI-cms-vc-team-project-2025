using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThunderWeapon : Weapon
{
    [Header("Strike Prefab & Timing")]
    [SerializeField] private GameObject thunderStrikePrefab;
    [SerializeField] private float burstDelay = 0.2f;
    [SerializeField] private float activationRange = 8f;

    private float cooldownTimer;

    void Update()
    {
        if (!PlayerController.Instance.gameObject.activeSelf) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            List<GameObject> pool = FindEnemiesInRange(activationRange);
            if (pool.Count > 0)
            {
                cooldownTimer = stats[weaponLevel].cooldown;
                StartCoroutine(StrikeEnemies(pool));
            }
        }
    }

    private IEnumerator StrikeEnemies(List<GameObject> pool)
    {
        int strikes = Mathf.Max(1, stats[weaponLevel].amount);
        strikes = Mathf.Min(strikes, pool.Count);

        for (int i = 0; i < pool.Count; i++)
        {
            int swap = Random.Range(i, pool.Count);
            (pool[i], pool[swap]) = (pool[swap], pool[i]);
        }

        for (int i = 0; i < strikes; i++)
        {
            Instantiate(thunderStrikePrefab,
                        pool[i].transform.position,
                        Quaternion.identity);
            yield return new WaitForSeconds(burstDelay);
        }
    }

    private List<GameObject> FindEnemiesInRange(float maxDist)
    {
        GameObject[] all = GameObject.FindGameObjectsWithTag("Enemy");
        var result = new List<GameObject>();
        Vector3 origin = transform.position;

        foreach (GameObject e in all)
            if (Vector3.Distance(origin, e.transform.position) <= maxDist)
                result.Add(e);

        return result;
    }
}