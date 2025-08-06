using UnityEngine;
using TMPro;

public class HighScoreDisplayManager : MonoBehaviour
{
    public TMP_Text[] aiRunTexts;
    public TMP_Text[] manualRunTexts;

    void OnEnable()
    {
        if (HighScoreManager.Instance == null) return;

        var aiScores = HighScoreManager.Instance.AiScores;
        var manualScores = HighScoreManager.Instance.ManualScores;

    for (int i = 0; i < 10; i++)
    {
        if (i < aiScores.Count)
        {
            int minutes = Mathf.FloorToInt(aiScores[i] / 60f);
            int seconds = Mathf.FloorToInt(aiScores[i] % 60f);
            aiRunTexts[i].text = $"Top AI Run {i + 1}: {minutes}:{seconds:D2}";
        }
        else
        {
            aiRunTexts[i].text = $"Top AI Run {i + 1}: —";
        }

        if (i < manualScores.Count)
        {
            int minutes = Mathf.FloorToInt(manualScores[i] / 60f);
            int seconds = Mathf.FloorToInt(manualScores[i] % 60f);
            manualRunTexts[i].text = $"Top Player Run {i + 1}: {minutes}:{seconds:D2}";
        }
        else
        {
            manualRunTexts[i].text = $"Top Player Run {i + 1}: —";
        }
    }
    }
}