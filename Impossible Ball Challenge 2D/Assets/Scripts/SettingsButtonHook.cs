using UnityEngine;

public class SettingsButtonHook : MonoBehaviour
{
    public void OnToggleBGM() => AudioManager.Instance?.ToggleBGM();
    public void OnToggleSFX() => AudioManager.Instance?.ToggleSFX();
    public void OnOpenPopup(GameObject popup) => popup.SetActive(true);
    public void OnClosePopup(GameObject popup) => popup.SetActive(false);
}
