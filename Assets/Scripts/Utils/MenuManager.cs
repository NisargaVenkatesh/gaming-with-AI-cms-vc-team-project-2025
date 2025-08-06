using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private Button aiButton;
    private Button manualButton;

    void Start()
    {
        // DEBUG ONLY: Reset on every launch (REMOVE after testing)
        //PlayerPrefs.SetString("ManualScores", "0,0,0,0,0,0,0,0,0,0");
        //PlayerPrefs.Save();
        //Debug.Log("Manual scores forcibly reset.");
        //PlayerPrefs.DeleteKey("HasInitializedScores");

        if (!PlayerPrefs.HasKey("HasInitializedScores"))
        {
            string aiScoreString = "742,712,630,593,575,573,545,518,0,0";
            PlayerPrefs.SetString("AIScores", aiScoreString);

            string manualScoreString = "0,0,0,0,0,0,0,0,0,0";
            PlayerPrefs.SetString("ManualScores", manualScoreString);

            PlayerPrefs.SetInt("HasInitializedScores", 1);
            PlayerPrefs.Save();

            Debug.Log("High scores initialized for the first time.");
        }

        aiButton = GameObject.Find("AI Button")?.GetComponent<Button>();
        manualButton = GameObject.Find("Manual Button")?.GetComponent<Button>();

        if (aiButton != null)
            aiButton.onClick.AddListener(StartAIGame);
        if (manualButton != null)
            manualButton.onClick.AddListener(StartManualGame);
    }

    public void StartAIGame()
    {
        PlayerPrefs.SetInt("UseAI", 1);
        SceneManager.LoadScene("GameAI");
    }

    public void StartManualGame()
    {
        PlayerPrefs.SetInt("UseAI", 0);
        SceneManager.LoadScene("Game");
    }
    public void StartCompareMode()
    {
        SceneManager.LoadScene("GameMultiplayer");
        PlayerPrefs.SetInt("UseAI", 0);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit button pressed.");
    }

}