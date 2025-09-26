using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BounceTarget : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color idleColor = Color.white;
    public Color hitColor  = Color.red;

    bool hitOnce;

    void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    if (spriteRenderer) spriteRenderer.color = idleColor;

    // ✅ Register with GameManager
    var gm = FindFirstObjectByType<GameManagerSimple>();
    if (gm) gm.RegisterTarget(this);
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (hitOnce) return;
        if (!c.collider.CompareTag("Ball")) return;

        hitOnce = true;
        if (spriteRenderer) spriteRenderer.color = hitColor;

        var gm = FindFirstObjectByType<GameManagerSimple>();
        if (gm) gm.MarkTargetHit(this);
    }
}
