using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerController))]
public class CrimsonPact : Agent
{
    private PlayerController controller;
    public float maxEnemyDistance = 20f;
    public Tilemap boundaryTilemap;
    public Tilemap[] obstacleTilemaps;
    public Collider2D[] obstacleColliders;
    private Vector3 previousDirection = Vector2.zero;
    private Bounds boundaryWorldBounds;
    private float previousMinEnemyDistance = 5f;
    private Vector3 previousPosition;
    private float survivalTimer = 0f;

    public float rewardFromWall = 0f;
    public float rewardFromEnemy = 0f;
    public float rewardFromMovement = 0f;
    public float rewardFromLevelUp = 0f;
    public float rewardFromSurvival = 0f;
    public float rewardFromDeath = 0f;
    public float rewardFromObstacle = 0f;
    public float rewardFromEnemyDamage = 0f;
    public float rewardFromEscape = 0f;

    private float[] weaponLevels;

    public override void Initialize()
    {
        controller = GetComponent<PlayerController>();
        boundaryWorldBounds = boundaryTilemap.localBounds;
        
        if (Application.isBatchMode || Academy.Instance.IsCommunicatorOn)
            Time.timeScale = 20f;

        weaponLevels = new float[4];
    }

    private bool hasInitialized = false;

    public override void OnEpisodeBegin()
    {
        if (PlayerPrefs.GetInt("UseAI", 0) == 1 && survivalTimer > 0f)
        {
            Debug.Log($"[Agent Survival Time] {survivalTimer:F1} seconds");
        }
        survivalTimer = 0f;

        WeaponSelectionTracker.Print();
        float total = rewardFromWall + rewardFromEnemy + rewardFromMovement + rewardFromLevelUp +
                    rewardFromSurvival + rewardFromDeath + rewardFromObstacle + rewardFromEnemyDamage + rewardFromEscape;
        if (rewardFromWall == 0 && rewardFromEnemy == 0 && rewardFromMovement == 0 && 
            rewardFromLevelUp == 0 && rewardFromSurvival == 0 && rewardFromDeath == 0 && 
            rewardFromObstacle == 0 && rewardFromEnemyDamage == 0 && rewardFromEscape == 0)
        {
            return;
        }
        Debug.Log($"[{name}] [Reward Breakdown] Wall: {rewardFromWall:F2}, Enemy: {rewardFromEnemy:F2}, Movement: {rewardFromMovement:F2}, " +
                $"LevelUp: {rewardFromLevelUp:F2}, Survival: {rewardFromSurvival:F2}, Death: {rewardFromDeath:F2}, " +
                $"Obstacle: {rewardFromObstacle:F2}, EnemyDamage: {rewardFromEnemyDamage:F2}, Escape: {rewardFromEscape:F2}, Total: {total:F2}");

        rewardFromWall = 0f;
        rewardFromEnemy = 0f;
        rewardFromMovement = 0f;
        rewardFromLevelUp = 0f;
        rewardFromSurvival = 0f;
        rewardFromDeath = 0f;
        rewardFromObstacle = 0f;
        rewardFromEnemyDamage = 0f;
        rewardFromEscape = 0f;

        controller.playerHealth = controller.playerMaxHealth;
        controller.transform.position = Vector3.zero;
        controller.playerMoveDirection = Vector3.zero;
        controller.lastMoveDirection = Vector3.down;

        UIController.Instance.LevelUpPanelClose();
        UIController.Instance.UpdateHealthSlider();
        UIController.Instance.UpdateExperienceSlider();

        previousPosition = controller.transform.position;
        if (hasInitialized)
        {
            controller.experience = 0;
            controller.currentLevel = 1;

            foreach (Weapon weapon in controller.activeWeapons)
                weapon.gameObject.SetActive(false);

            controller.InactiveWeapons.AddRange(controller.activeWeapons);
            controller.activeWeapons.Clear();
            controller.UpgradeableWeapons.Clear();
            controller.maxLevelWeapons.Clear();

            if (controller.StartingWeapon != null && controller.InactiveWeapons.Contains(controller.StartingWeapon))
                controller.ActivateWeapon(controller.StartingWeapon);
            else if (controller.InactiveWeapons.Count > 0)
                controller.ActivateWeapon(controller.InactiveWeapons[0]);
        }

        previousMinEnemyDistance = 5f;
        hasInitialized = true;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(controller.playerHealth / controller.playerMaxHealth);

        sensor.AddObservation(new Vector2(controller.lastMoveDirection.x, controller.lastMoveDirection.y));

        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy")
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .Take(3)
            .ToArray();

        foreach (var enemy in allEnemies)
        {
            Vector3 rel = enemy.transform.position - transform.position;
            Vector2 clamped = Vector2.ClampMagnitude(new Vector2(rel.x, rel.y), maxEnemyDistance) / maxEnemyDistance;
            sensor.AddObservation(clamped);
        }

        for (int i = allEnemies.Length; i < 3; i++)
        {
            sensor.AddObservation(Vector2.zero);
        }

        float cellSize = 4f;
        int[,] grid = new int[3, 3];

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector3 local = enemy.transform.position - transform.position;

            int col = Mathf.FloorToInt(local.x / cellSize) + 1; 
            int row = Mathf.FloorToInt(local.y / cellSize) + 1;

            if (col >= 0 && col < 3 && row >= 0 && row < 3)
            {
                grid[row, col]++;
            }
        }

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                float density = Mathf.Clamp01(grid[r, c] / 10f);
                sensor.AddObservation(density);
            }
        }

        Vector3 pos = controller.transform.position;
        float mapHalfSize = 16f;

        float distLeft = Mathf.Abs(pos.x - boundaryWorldBounds.min.x) / mapHalfSize;
        float distRight = Mathf.Abs(boundaryWorldBounds.max.x - pos.x) / mapHalfSize;
        float distBottom = Mathf.Abs(pos.y - boundaryWorldBounds.min.y) / mapHalfSize;
        float distTop = Mathf.Abs(boundaryWorldBounds.max.y - pos.y) / mapHalfSize;

        sensor.AddObservation(distLeft);
        sensor.AddObservation(distRight);
        sensor.AddObservation(distTop);
        sensor.AddObservation(distBottom);
        sensor.AddObservation(controller.playerMoveDirection.magnitude);

        for (int i = 0; i < weaponLevels.Length; i++)
        {
            sensor.AddObservation(weaponLevels[i]);
        }
    }
    public void UpdateWeaponLevels()
    {
        for (int i = 0; i < weaponLevels.Length; i++) weaponLevels[i] = 0f;

        for (int i = 0; i < controller.activeWeapons.Count && i < weaponLevels.Length; i++)
        {
            var weapon = controller.activeWeapons[i];
            if (weapon.stats.Count > 0)
            {
                weaponLevels[i] = (float)(weapon.weaponLevel + 1) / weapon.stats.Count;
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (UIController.Instance.levelUpPanel.activeSelf)
        {
            int upgradeChoice = actions.DiscreteActions[0];
            if (upgradeChoice >= 0 && upgradeChoice < UIController.Instance.levelUpButtons.Length)
            {
                var btn = UIController.Instance.levelUpButtons[upgradeChoice];
                if (btn.gameObject.activeSelf)
                {
                    btn.SelectUpgrade();
                    return;
                }
            }

            for (int i = 0; i < UIController.Instance.levelUpButtons.Length; i++)
            {
                if (UIController.Instance.levelUpButtons[i].gameObject.activeSelf)
                {
                    UIController.Instance.levelUpButtons[i].SelectUpgrade();
                    break;
                }
            }

            return;
        }

        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        controller.playerMoveDirection = new Vector3(moveX, moveY).normalized;

        if (previousDirection != Vector3.zero && controller.playerMoveDirection != Vector3.zero)
        {
            float angleChange = Vector3.Angle(previousDirection, controller.playerMoveDirection);
            if (angleChange > 45f)
            {
                AddReward(-0.05f);
            }
        }
        previousDirection = controller.playerMoveDirection;

        AddReward(+0.1f);
        rewardFromSurvival += 0.1f;

        if (controller.playerMoveDirection.magnitude < 0.1f)
        {
            AddReward(-0.02f);
            rewardFromMovement -= 0.02f;
        }

        float worldMoveDist = Vector3.Distance(transform.position, previousPosition);
        if (worldMoveDist < 0.05f)
        {
            AddReward(-0.03f);
            rewardFromMovement -= 0.03f;
        }
        previousPosition = transform.position;

        float moveReward = 0.02f * controller.playerMoveDirection.magnitude;
        AddReward(moveReward);
        rewardFromMovement += moveReward;

        ApplyWallPenalty();
        ApplyTileObstaclePenalty();
        ApplyColliderObstaclePenalty();
        ApplyEnemyProximityPenalty();
        ApplyEscapeReward();

        float closest = 5f;
        var all = GameObject.FindGameObjectsWithTag("Enemy");
        if (all.Length > 0)
        {
            closest = all.Min(e => Vector3.Distance(transform.position, e.transform.position));
        }

        float delta = Mathf.Clamp(closest - previousMinEnemyDistance, -5f, 5f);
        float deltaReward = delta * 0.03f;

        if (!float.IsNaN(deltaReward) && !float.IsInfinity(deltaReward))
        {
            AddReward(deltaReward);
            rewardFromEnemy += deltaReward;
        }

        previousMinEnemyDistance = closest;

        if (StepCount % 500 == 0)
        {
            AddReward(+0.5f);
        }
    }
    public void PenalizeDeath()
    {
        AddReward(-200f);
        rewardFromDeath -= 200f;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private bool isFastForward = false;

    private void ToggleGameSpeed()
    {
        if (PlayerPrefs.GetInt("UseAI", 0) == 1)
        {
            isFastForward = !isFastForward;
            Time.timeScale = isFastForward ? 5f : 1f;
            Debug.Log("Game speed: " + Time.timeScale + "x");
        }
    }
    public void ApplySpeedSetting()
    {
        if (PlayerPrefs.GetInt("UseAI", 0) == 1)
        {
            Time.timeScale = isFastForward ? 5f : 1f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        survivalTimer += Time.deltaTime;

        RequestDecision();

        if ((Application.isBatchMode || Academy.Instance.IsCommunicatorOn) && Time.timeScale != 25f)
        {
            Time.timeScale = 25f;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleGameSpeed();
        }
    }


    public void ReloadScene()
    {
        Invoke(nameof(DoSceneReload), 0.1f);
    }

    private void DoSceneReload()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GiveReward(float value)
    {
        AddReward(value);

        if (value < 0f)
            rewardFromEnemy += value;
        else
            rewardFromLevelUp += value;

        UpdateWeaponLevels();
    }


    private float ApplyWallPenalty()
    {
        Vector3 pos = controller.transform.position;
        float minDist = Mathf.Min(
            Mathf.Abs(pos.x - boundaryWorldBounds.min.x),
            Mathf.Abs(boundaryWorldBounds.max.x - pos.x),
            Mathf.Abs(pos.y - boundaryWorldBounds.min.y),
            Mathf.Abs(boundaryWorldBounds.max.y - pos.y)
        );

        float penalty = 0f;
        if (minDist < 4f)
        {
            penalty = Mathf.Clamp(-0.15f * (4f - minDist), -1.5f, 0f);
            AddReward(penalty);
            rewardFromWall += penalty;

            if (minDist <= 0.1f)
            {
                AddReward(-3.0f);
                rewardFromWall -= 3.0f;
            }
        }
        return penalty;
    }

    private float ApplyTileObstaclePenalty()
    {
        Vector3 pos = controller.transform.position;
        float minDist = float.MaxValue;

        foreach (var tilemap in obstacleTilemaps)
        {
            Vector3Int cellPos = tilemap.WorldToCell(pos);
            if (tilemap.HasTile(cellPos))
            {
                Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);
                float dist = Vector3.Distance(pos, cellCenter);
                minDist = Mathf.Min(minDist, dist);
            }
        }

        float penalty = 0f;
        if (minDist < 4f)
        {
            penalty = Mathf.Clamp(-0.08f * (3f - minDist), -2f, 0f);
            AddReward(penalty);
            rewardFromObstacle += penalty;

            if (minDist == 0f)
            {
                AddReward(-0.5f);
                rewardFromObstacle -= 0.5f;
            }
        }
        return penalty;
    }

    private float ApplyColliderObstaclePenalty()
    {
        Vector3 pos = controller.transform.position;
        float minDist = float.MaxValue;

        foreach (var col in obstacleColliders)
        {
            if (col != null)
            {
                float dist = Vector2.Distance(pos, col.ClosestPoint(pos));
                minDist = Mathf.Min(minDist, dist);
            }
        }

        float penalty = 0f;
        if (minDist < 3f)
        {
            penalty = Mathf.Clamp(-0.06f * (3f - minDist), -2f, 0f);
            AddReward(penalty);
            rewardFromObstacle += penalty;

            if (minDist == 0f)
            {
                AddReward(-0.5f);
                rewardFromObstacle -= 0.5f;
            }
        }
        return penalty;
    }

    private float ApplyEnemyProximityPenalty()
    {
        float minDistance = GameObject.FindGameObjectsWithTag("Enemy")
            .Select(e => Vector3.Distance(transform.position, e.transform.position))
            .DefaultIfEmpty(10f)
            .Min();

        float penalty = 0f;
        if (minDistance < 5f)
        {
            penalty = -Mathf.Clamp((5f - minDistance) * 0.05f, 0f, 1.3f);
            AddReward(penalty);
            rewardFromEnemy += penalty;
        }
        return penalty;
    }

    private void ApplyEscapeReward()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0 || controller.playerMoveDirection.magnitude < 0.1f)
            return;

        Vector3 dangerVector = Vector3.zero;
        foreach (var e in enemies)
        {
            Vector3 toEnemy = e.transform.position - transform.position;
            float distance = toEnemy.magnitude;
            if (distance < 5f)
            {
                dangerVector += toEnemy.normalized * (1f - distance / 5f);
            }
        }

        dangerVector.Normalize();
        float escapeScore = -Vector3.Dot(controller.playerMoveDirection.normalized, dangerVector);
        float escapeReward = Mathf.Clamp(escapeScore, -1f, 1f) * controller.playerMoveDirection.magnitude * 0.1f;

        AddReward(escapeReward);
        rewardFromEscape += escapeReward;
        
    }

    // gizmo for visualizing distances from the agent (for debug)
/*
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
    */
}
