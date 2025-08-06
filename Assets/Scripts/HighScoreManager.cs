using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    public IReadOnlyList<float> AiScores => aiScores;
    public IReadOnlyList<float> ManualScores => manualScores;

    private List<float> aiScores = new List<float>();
    private List<float> manualScores = new List<float>();

    private const string AiKey = "AIScores";
    private const string ManualKey = "ManualScores";


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadScores();
    }

    public void AddScore(float timeSeconds, bool isAI)
    {
        List<float> list = isAI ? aiScores : manualScores;

        list.Add(timeSeconds);
        list.Sort((a, b) => b.CompareTo(a));

        if (list.Count > 10)
            list.RemoveAt(10);

        SaveScores();
    }

    private void SaveScores()
    {
        PlayerPrefs.SetString(AiKey, string.Join(",", aiScores));
        PlayerPrefs.SetString(ManualKey, string.Join(",", manualScores));
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        aiScores = LoadList(AiKey);
        manualScores = LoadList(ManualKey);
    }

    private List<float> LoadList(string key)
    {
        List<float> list = new List<float>();
        string raw = PlayerPrefs.GetString(key, "");

        if (!string.IsNullOrEmpty(raw))
        {
            string[] parts = raw.Split(',');
            foreach (string part in parts)
            {
                if (float.TryParse(part, out float val) && val > 0f)
                    list.Add(val);
            }
        }

        return list;
    }
}