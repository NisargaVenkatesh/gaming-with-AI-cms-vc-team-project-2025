using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float gameTime;
    public bool gameActive;

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    void Start(){
        gameActive = true;
    }

    void Update(){
        if (gameActive){
            gameTime += Time.deltaTime;
            UIController.Instance.UpdateTimer(gameTime);

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)){
                Pause();
            }
        }
    }

    public void GameOver()
    {
        gameActive = false;
        StartCoroutine(ShowGameOverScreen());

        if (HighScoreManager.Instance != null)
        {
            Debug.Log("Adding score: " + gameTime);
            HighScoreManager.Instance.AddScore(gameTime, PlayerPrefs.GetInt("UseAI", 0) == 1);
        }
        else
        {
            Debug.LogWarning("HighScoreManager.Instance is null at GameOver.");
        }
    }

    IEnumerator ShowGameOverScreen()
    {
        
        yield return new WaitForSeconds(0.1f);
        UIController.Instance.gameOverPanel.SetActive(true);
        AudioController.Instance.PlaySound(AudioController.Instance.gameOver);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        if (PlayerPrefs.GetInt("UseAI", 0) == 0)
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            SceneManager.LoadScene("GameAI");
        }
        
        var agent = PlayerController.Instance.GetComponent<CrimsonPact>();
        agent.ApplySpeedSetting();
    }

    public void Pause(){
        if (UIController.Instance.levelUpPanel.activeSelf == false){
            if (
                UIController.Instance.pausePanel.activeSelf == false && 
                UIController.Instance.gameOverPanel.activeSelf == false
                ){
                UIController.Instance.pausePanel.SetActive(true);
                Time.timeScale = 0f;
                AudioController.Instance.PlaySound(AudioController.Instance.pause);
            } else {
                UIController.Instance.pausePanel.SetActive(false);
                Time.timeScale = 1f;
                AudioController.Instance.PlaySound(AudioController.Instance.unpause);
            }
        }
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Time scale before loading menu: " + Time.timeScale);
        SceneManager.LoadScene("Main Menu");
    }
}
