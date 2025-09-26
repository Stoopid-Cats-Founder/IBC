using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinPopup : MonoBehaviour
{
    public TextMeshProUGUI congratsText;
    LevelManager lm;

    void Awake()
    {
        lm = FindFirstObjectByType<LevelManager>();
        gameObject.SetActive(false);   // keep root hidden until Show()
        Time.timeScale = 1f;
    }

    public void Show(int tries)
    {
        if (congratsText) congratsText.text = $"Congrats!\nYou did it in {tries} tries.";
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    // public void BtnNext()
    // {
    //     Time.timeScale = 1f;
    //     gameObject.SetActive(false);
    //     if (lm) lm.LoadNext();
    // }

    public void BtnRetry()
{
    Time.timeScale = 1f;

        gameObject.SetActive(false);  // 👉 먼저 WinPopup 비활성화

    lm.ReloadThisLevel();         // 👉 그다음 레벨 리셋
}


    public void BtnLevelSelect()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        SceneManager.LoadScene("LevelSelect");
    }

    public void BtnShare()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        string shareText = "I just cleared a level in the Bounce Cup game! 🏆🔥";
        Application.OpenURL("https://twitter.com/intent/tweet?text=" +
                            UnityEngine.Networking.UnityWebRequest.EscapeURL(shareText));
    }
}
