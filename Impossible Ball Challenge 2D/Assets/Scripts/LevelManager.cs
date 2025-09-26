// Scripts/LevelManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Prefabs (set in Inspector)")]
    public GameObject[] levelPrefabs;
    public GameObject ballPrefab;

    [Header("Where to spawn level")]
    public Transform levelRoot;

    GameObject currentLevel;
    public int CurrentLevelNumber { get; private set; } = 1;

    GameManagerSimple gm;
    OneShotSimple ball;

    void Awake()
    {
        gm = FindFirstObjectByType<GameManagerSimple>();
        ball = FindFirstObjectByType<OneShotSimple>();
        if (!ball && ballPrefab)
        {
            var ballGO = Instantiate(ballPrefab);
            ball = ballGO.GetComponent<OneShotSimple>();
        }
    }

    void Start()
    {
        int startLevel = LevelRouter.PendingLevel > 0 ? LevelRouter.PendingLevel : 1;
        LoadLevel(startLevel);
    }

    public void LoadLevel(int levelNumber)
    {
        if (currentLevel) Destroy(currentLevel);

        int idx = Mathf.Clamp(levelNumber - 1, 0, levelPrefabs.Length - 1);
        currentLevel = Instantiate(levelPrefabs[idx]);

        var parent = levelRoot ? levelRoot : transform;
        if (parent && parent.gameObject.scene.IsValid())
            currentLevel.transform.SetParent(parent, false);

        CurrentLevelNumber = levelNumber;

        // cup 처리
        var cup = currentLevel.GetComponentInChildren<Cup>(true);
        if (cup) cup.Close();

        if (gm)
        {
            gm.SetCup(cup);
            gm.OnLevelLoaded(CurrentLevelNumber);
        }

        // BounceTarget 등록
        if (gm)
        {
            var targets = currentLevel.GetComponentsInChildren<BounceTarget>(true);
            foreach (var t in targets)
            {
                gm.RegisterTarget(t);
            }
        }

        // Ball 위치 초기화
        var spawn = FindChildByName(currentLevel.transform, "BallSpawn");
        if (spawn && ball)
        {
            ball.transform.position = spawn.position;
            StartCoroutine(DelayedReset());
        }
        else
        {
            if (!spawn) Debug.LogError($"BallSpawn not found under {currentLevel.name}");
            if (!ball) Debug.LogError("Shared Ball not found or instantiated.");
        }
    }

    IEnumerator DelayedReset()
    {
        yield return null;
        ball.ResetShot();
        if (gm) gm.HookBall(ball);
    }

    public void LoadNext()
    {
        int next = CurrentLevelNumber + 1;
        if (next <= levelPrefabs.Length)
            LoadLevel(next);
        else
            SceneManager.LoadScene("LevelSelect");
    }

    public void ReloadThisLevel()
    {
        LoadLevel(CurrentLevelNumber);
    }

    static Transform FindChildByName(Transform root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    public void BtnGoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }
    
    public void BtnGoHome()
    {
        Time.timeScale = 1f; // reset pause if any
        SceneManager.LoadScene("MainMenu");
    }

    public GameObject settingsPopup;
    
    public void BtnOpenSettings()
    {
        if (settingsPopup) settingsPopup.SetActive(true);
    }

    public void BtnCloseSettings()
    {
        if (settingsPopup) settingsPopup.SetActive(false);
    }

}
