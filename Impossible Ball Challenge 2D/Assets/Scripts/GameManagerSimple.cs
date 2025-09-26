// ✅ GameManagerSimple.cs (최신)
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameManagerSimple : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI attemptsText;
    public WinPopup popup;

    [Header("Refs")]
    public OneShotSimple ball;

    Cup cup;
    int currentLevel;
    int attempts = 0;
    bool levelWon = false;

    readonly HashSet<BounceTarget> allTargets = new();
    readonly HashSet<BounceTarget> hitTargets = new();

    public int Attempts => attempts;

    void Start()
    {
        if (ball) ball.OnLaunch += OnBallLaunched;
        UpdateAttemptsUI();
    }

    public void HookBall(OneShotSimple b)
    {
        if (ball != null) ball.OnLaunch -= OnBallLaunched;
        ball = b;
        if (ball != null) ball.OnLaunch += OnBallLaunched;
    }

    void OnBallLaunched()
    {
        attempts++;
        Progress.SetTries(currentLevel, attempts);  // ✅ 항상 저장
        UpdateAttemptsUI();
    }

    void UpdateAttemptsUI()
    {
        if (attemptsText)
            attemptsText.text = $"Tries: {attempts}";
    }

    public void SetCup(Cup c) => cup = c;

    public void RegisterTarget(BounceTarget t)
    {
        allTargets.Add(t);
        CheckOpen();
    }

   public void MarkTargetHit(BounceTarget t)
    {
    if (hitTargets.Add(t)) 
    {
        if (cup) cup.UpdateLidProgress(hitTargets.Count, allTargets.Count);
        CheckOpen();
    }
    }

    void CheckOpen()
    {
        if (cup && allTargets.Count > 0 && hitTargets.Count == allTargets.Count)
            cup.Open();
    }

    public void OnLevelCompleted()
    {
        if (levelWon) return;
        levelWon = true;

        var lm = FindFirstObjectByType<LevelManager>();
        if (lm)
            Progress.MaxLevelCleared = Mathf.Max(Progress.MaxLevelCleared, lm.CurrentLevelNumber);

        if (popup) popup.Show(Attempts);
    }

    public void OnLevelLoaded(int level)
    {
    allTargets.Clear();
    hitTargets.Clear();
    currentLevel = level;
    attempts = Progress.GetTries(level);
    levelWon = false;
    UpdateAttemptsUI();

    if (cup) cup.UpdateLidProgress(0, 1); // reset at start
    }


    public void BtnRestartFromHUD()
    {
        Time.timeScale = 1f;
        var lm = FindFirstObjectByType<LevelManager>();
        if (lm) lm.ReloadThisLevel();
    }
}
