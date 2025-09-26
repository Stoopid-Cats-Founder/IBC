using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    // hook this to each level button with its level number
    public void BtnLoadLevel(int levelNumber)
    {
        LevelRouter.PendingLevel = levelNumber;
        SceneManager.LoadScene("Gameplay");
    }

    public void BtnBackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
