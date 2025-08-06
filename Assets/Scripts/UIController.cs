using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider playerExperienceSlider;
    [SerializeField] private TMP_Text experienceText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject levelUpPanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text weaponLevelsText;
    
    public LevelUpButton[] levelUpButtons;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void UpdateHealthSlider()
    {
        playerHealthSlider.maxValue = PlayerController.Instance.playerMaxHealth;
        playerHealthSlider.value = PlayerController.Instance.playerHealth;
        healthText.text = playerHealthSlider.value + " / " + playerHealthSlider.maxValue;
    }

    public void UpdateExperienceSlider()
    {
        playerExperienceSlider.maxValue = PlayerController.Instance.playerLevels[PlayerController.Instance.currentLevel - 1];
        playerExperienceSlider.value = PlayerController.Instance.experience;
        experienceText.text = playerExperienceSlider.value + " / " + playerExperienceSlider.maxValue;
    }

    public void UpdateTimer(float timer)
    {
        float min = Mathf.FloorToInt(timer / 60f);
        float sec = Mathf.FloorToInt(timer % 60f);

        timerText.text = min + ":" + sec.ToString("00");
    }

    public void LevelUpPanelOpen()
    {
        levelUpPanel.SetActive(true);

        if (PlayerPrefs.GetInt("UseAI", 0) == 0)
        {
            Time.timeScale = 0f;
        }
    }

    public void LevelUpPanelClose()
    {
        levelUpPanel.SetActive(false);

        var agent = PlayerController.Instance.GetComponent<CrimsonPact>();
        if (agent != null)
        {
            agent.ApplySpeedSetting();
        }
    }

    
    public void UpdateWeaponLevelsDisplay()
    {
        var activeWeapons = PlayerController.Instance.activeWeapons;

        string text = "Weapons:\n";
        foreach (var w in activeWeapons)
        {
            text += $"{w.gameObject.name}: Lv. {w.weaponLevel + 1}\n";
        }

        weaponLevelsText.text = text;
    }
}
