using UnityEngine;

[ExecuteAlways]
public class ScreenBoundaries2D : MonoBehaviour
{
    public Camera cam;
    [Min(0.01f)] public float thickness = 1f;
    public bool insideEdges = true;

    [Header("Visuals")]
    public bool visible = true;
    public Color color = Color.black;
    public string sortingLayer = "Default";
    public int sortingOrder = 1000;
    public Sprite pixelSprite;   // drag your 1x1 Pixel.png sprite here

    [Header("Physics")]
    public PhysicsMaterial2D physicsMaterial;

    BoxCollider2D top, bottom, left, right;
    SpriteRenderer rt, rb, rl, rr;

    void OnEnable()  { EnsureColliders(); EnsureRenderers(); UpdateBounds(); }
    void OnValidate(){ EnsureColliders(); EnsureRenderers(); UpdateBounds(); }
    void Update()
    {
        if (!cam) cam = Camera.main;
        if (!cam) return;
        UpdateBounds();
    }

    void Start()
    {
        // assign pixel sprite safely here (no editor warnings)
        AssignPixelSprites();
    }

    // ----------------- Colliders -----------------
    void EnsureColliders()
    {
        top    = GetOrCreateCollider("Top");
        bottom = GetOrCreateCollider("Bottom");
        left   = GetOrCreateCollider("Left");
        right  = GetOrCreateCollider("Right");

        ApplyMaterial(top);
        ApplyMaterial(bottom);
        ApplyMaterial(left);
        ApplyMaterial(right);
    }

    BoxCollider2D GetOrCreateCollider(string name)
    {
        var t = EnsureChild(name);
        var col = t.GetComponent<BoxCollider2D>();
        if (!col) col = t.gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = false;
        return col;
    }

    void ApplyMaterial(BoxCollider2D col)
    {
        if (col) col.sharedMaterial = physicsMaterial;
    }

    // ----------------- Renderers -----------------
    void EnsureRenderers()
    {
        rt = GetOrCreateRenderer("Top");
        rb = GetOrCreateRenderer("Bottom");
        rl = GetOrCreateRenderer("Left");
        rr = GetOrCreateRenderer("Right");

        ApplyRendererSettings(rt);
        ApplyRendererSettings(rb);
        ApplyRendererSettings(rl);
        ApplyRendererSettings(rr);
    }

    SpriteRenderer GetOrCreateRenderer(string name)
    {
        var t = EnsureChild(name);
        var sr = t.GetComponent<SpriteRenderer>();
        if (!sr) sr = t.gameObject.AddComponent<SpriteRenderer>();
        // don’t assign sprite here → that caused warnings
        return sr;
    }

    void AssignPixelSprites()
    {
        if (pixelSprite == null) return;
        if (rt && rt.sprite == null) rt.sprite = pixelSprite;
        if (rb && rb.sprite == null) rb.sprite = pixelSprite;
        if (rl && rl.sprite == null) rl.sprite = pixelSprite;
        if (rr && rr.sprite == null) rr.sprite = pixelSprite;
    }

    void ApplyRendererSettings(SpriteRenderer sr)
    {
        if (!sr) return;
        sr.enabled = visible;
        sr.color = color;
        sr.sortingLayerName = sortingLayer;
        sr.sortingOrder = sortingOrder;
        sr.drawMode = SpriteDrawMode.Sliced; // scales 1x1 sprite
    }

    // ----------------- Child Helper -----------------
    Transform EnsureChild(string name)
    {
        var t = transform.Find(name);
        if (!t)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            t = go.transform;
        }
        return t;
    }

    // ----------------- Update Bounds -----------------
    void UpdateBounds()
    {
        if (!cam) return;

        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 c = cam.transform.position;
        float offset = insideEdges ? -thickness * 0.5f : thickness * 0.5f;

        Vector2 horizSize = new Vector2(halfW * 2f, thickness);
        Vector2 vertSize  = new Vector2(thickness, halfH * 2f);

        if (top)    { top.size = horizSize; top.transform.position    = new(c.x, c.y + halfH + offset, 0f); }
        if (bottom) { bottom.size = horizSize; bottom.transform.position = new(c.x, c.y - halfH - offset, 0f); }
        if (left)   { left.size = vertSize;  left.transform.position   = new(c.x - halfW - offset, c.y, 0f); }
        if (right)  { right.size = vertSize; right.transform.position  = new(c.x + halfW + offset, c.y, 0f); }

        FitRenderer(rt, top);
        FitRenderer(rb, bottom);
        FitRenderer(rl, left);
        FitRenderer(rr, right);

        ApplyRendererSettings(rt);
        ApplyRendererSettings(rb);
        ApplyRendererSettings(rl);
        ApplyRendererSettings(rr);
    }

    void FitRenderer(SpriteRenderer sr, BoxCollider2D col)
    {
        if (!sr || !col) return;
        sr.transform.position = col.transform.position;
        sr.transform.localScale = new Vector3(col.size.x, col.size.y, 1f);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!visible || !cam) return;
        Gizmos.color = new Color(color.r, color.g, color.b, 0.25f);
        if (top)    Gizmos.DrawCube(top.transform.position,    new Vector3(top.size.x,    top.size.y,    0.01f));
        if (bottom) Gizmos.DrawCube(bottom.transform.position, new Vector3(bottom.size.x, bottom.size.y, 0.01f));
        if (left)   Gizmos.DrawCube(left.transform.position,   new Vector3(left.size.x,   left.size.y,   0.01f));
        if (right)  Gizmos.DrawCube(right.transform.position,  new Vector3(right.size.x,  right.size.y,  0.01f));
    }
#endif
}
