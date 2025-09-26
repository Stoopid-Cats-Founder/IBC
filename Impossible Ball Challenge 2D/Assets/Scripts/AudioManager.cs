using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    [Header("SFX")]
    public AudioClip baseNote;
    public int audioSourcePoolSize = 5;
    private AudioSource[] pool;
    private int poolIndex = 0;
    [Range(0f, 1f)] public float sfxVolume = 0.2f;

    // UI refs (auto-bound)
    private GameObject settingsPopup;
    private Image bgmOnIcon, bgmOffIcon;
    private Image sfxOnIcon, sfxOffIcon;

    private bool bgmEnabled = true;
    private bool sfxEnabled = true;

    [Header("UI Sounds")]
    public AudioClip[] buttonClips; // assign 2–3 clips in Inspector

    // Pitch multipliers for D minor pentatonic
    private readonly float[] dMinorPentatonic =
        { 0.5f, 0.5946f, 0.667f, 0.749f, 0.8909f, 1.0f, 1.189f };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure BGM source exists
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
        if (bgmClip != null) bgmSource.clip = bgmClip;

        // Create audio source pool for SFX
        pool = new AudioSource[audioSourcePoolSize];
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            var go = new GameObject("AudioSource_" + i);
            go.transform.parent = transform;
            pool[i] = go.AddComponent<AudioSource>();
        }

        // Load saved settings
        bgmEnabled = PlayerPrefs.GetInt("BGM", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFX", 1) == 1;

        ApplyBGMState();

        // Subscribe to scene changes
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        StartCoroutine(RebindUntilSuccess());
    }

    private IEnumerator RebindUntilSuccess()
    {
        int tries = 0;
        while ((bgmOnIcon == null || sfxOnIcon == null) && tries < 10)
        {
            RebindUI();
            if (bgmOnIcon != null && sfxOnIcon != null)
                break;

            tries++;
            yield return new WaitForSeconds(0.2f); // retry every 0.2s
        }
        UpdateIcons();
    }

    // --- Finds children even if inactive ---
    private T FindChildByName<T>(Transform parent, string name) where T : Component
    {
        foreach (var comp in parent.GetComponentsInChildren<T>(true))
            if (comp.name == name) return comp;
        return null;
    }

    private void RebindUI()
    {
        // find popup even if inactive
        var popupObj = GameObject.FindObjectOfType<SettingsPopupMarker>(true);
        if (popupObj == null)
        {
            Debug.LogWarning("SettingsPopup not found in scene.");
            return;
        }

        settingsPopup = popupObj.gameObject;

        // find icons relative to popup
        bgmOnIcon  = settingsPopup.transform.Find("BGM_UnmuteIcon")?.GetComponent<Image>();
        bgmOffIcon = settingsPopup.transform.Find("BGM_MuteIcon")?.GetComponent<Image>();
        sfxOnIcon  = settingsPopup.transform.Find("SFX_UnmuteIcon")?.GetComponent<Image>();
        sfxOffIcon = settingsPopup.transform.Find("SFX_MuteIcon")?.GetComponent<Image>();

        UpdateIcons();
    }


    private void UpdateIcons()
    {
        Color active = Color.white;
        Color inactive = new Color(1f, 1f, 1f, 0.3f);

        if (bgmOnIcon && bgmOffIcon)
        {
            bgmOnIcon.color = bgmEnabled ? active : inactive;
            bgmOffIcon.color = bgmEnabled ? inactive : active;
        }

        if (sfxOnIcon && sfxOffIcon)
        {
            sfxOnIcon.color = sfxEnabled ? active : inactive;
            sfxOffIcon.color = sfxEnabled ? inactive : active;
        }
    }

    // ===== Public API =====

    public void PlayRandomWallNote()
    {
        if (!sfxEnabled || !baseNote) return;

        int idx = Random.Range(0, dMinorPentatonic.Length);
        float pitch = dMinorPentatonic[idx];

        AudioSource src = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Length;

        src.pitch = pitch;
        src.PlayOneShot(baseNote, sfxVolume);
    }

    public void ToggleBGM()
    {
        bgmEnabled = !bgmEnabled;
        PlayerPrefs.SetInt("BGM", bgmEnabled ? 1 : 0);
        PlayerPrefs.Save();

        ApplyBGMState();
        UpdateIcons();
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        PlayerPrefs.SetInt("SFX", sfxEnabled ? 1 : 0);
        PlayerPrefs.Save();

        UpdateIcons();
    }

    private void ApplyBGMState()
    {
        if (bgmSource == null) return;

        if (bgmEnabled)
        {
            if (!bgmSource.gameObject.activeSelf)
                bgmSource.gameObject.SetActive(true);

            bgmSource.mute = false;
            if (!bgmSource.isPlaying && bgmSource.clip != null)
                bgmSource.Play();
        }
        else
        {
            bgmSource.mute = true;
        }
    }

    
    public void PlayUIButton(int type = 0)
    {
        if (!sfxEnabled || buttonClips == null || buttonClips.Length == 0)
        {
            Debug.LogWarning("No button clips assigned in AudioManager!");
            return;
        }

        int idx = Mathf.Clamp(type, 0, buttonClips.Length - 1);
        if (idx >= buttonClips.Length) return; // extra guard

        AudioSource src = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Length;

        src.pitch = 1f;
        src.PlayOneShot(buttonClips[idx], sfxVolume);
    }


}
