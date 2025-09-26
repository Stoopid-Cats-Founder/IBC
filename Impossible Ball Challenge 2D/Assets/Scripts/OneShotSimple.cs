using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OneShotSimple : MonoBehaviour
{
    public Rigidbody2D rb;
    [Tooltip("Overall power scale for a full-strength drag")]
    public float basePower = 12f;

    bool launched;
    public System.Action OnLaunch;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>Launch once. dir = any length; strength01 = 0..1 from drag.</summary>
    public void Launch(Vector2 dir, float strength01)
    {
        if (launched) return;
        if (dir.sqrMagnitude < 0.0001f) return;

        launched = true;

        float s = Mathf.Clamp01(strength01);
        float power = basePower * Mathf.Lerp(0.25f, 1f, s);

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.AddForce(dir.normalized * power, ForceMode2D.Impulse);

        OnLaunch?.Invoke();
    }

    public void ResetShot()
    {
        launched = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.Sleep();   // ⛔ 물리 시뮬레이션 정지
        rb.WakeUp();  // ✅ 다시 깨워주기

        // 포지션 이동은 따로 LevelManager 쪽에서 하고 있을 테니 여기선 생략
    }
}

