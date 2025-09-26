using UnityEngine;

public class Cup : MonoBehaviour
{
    public GameObject lid;
    Vector3 initialScale;
    bool isOpen = false;

    public bool IsOpen => isOpen;

    void Awake()
    {
        if (lid) initialScale = lid.transform.localScale;
    }

    public void UpdateLidProgress(int hitCount, int totalCount)
    {
        if (!lid || totalCount <= 0) return;

        float progress = Mathf.Clamp01((float)hitCount / totalCount);
        lid.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, progress);
    }

    public void Open()
    {
        isOpen = true;
        if (lid) lid.SetActive(false);
    }

    public void Close()
    {
        isOpen = false;
        if (lid)
        {
            lid.SetActive(true);
            lid.transform.localScale = initialScale;
        }
    }
}
