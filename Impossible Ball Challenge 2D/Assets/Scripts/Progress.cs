using UnityEngine;

public static class Progress
{
    // 최대 클리어 레벨 저장 키
    const string MaxClearedKey = "MaxLevelCleared";

    public static int MaxLevelCleared
    {
        get => PlayerPrefs.GetInt(MaxClearedKey, 0);
        set
        {
            PlayerPrefs.SetInt(MaxClearedKey, value);
            PlayerPrefs.Save();
        }
    }

    // 시도 횟수 저장 키 (레벨마다 다르게)
    static string TriesKey(int level) => $"Tries_Level_{level}";

    public static int GetTries(int level)
    {
        return PlayerPrefs.GetInt(TriesKey(level), 0);
    }

    public static void SetTries(int level, int tries)
    {
        PlayerPrefs.SetInt(TriesKey(level), tries);
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
