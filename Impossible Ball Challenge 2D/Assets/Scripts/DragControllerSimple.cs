using UnityEngine;

[RequireComponent(typeof(OneShotSimple))]
public class DragControllerSimple : MonoBehaviour
{
    public LineRenderer line;
    public float dragLimit = 10f;

    Camera cam;
    OneShotSimple oneShot;
    bool isDragging;
    Vector3 startPos;

    Vector3 MouseWorld
    {
        get {
            var w = cam.ScreenToWorldPoint(Input.mousePosition);
            w.z = 0f;
            return w;
        }
    }

    void Awake()
    {
        cam = Camera.main;
        oneShot = GetComponent<OneShotSimple>();
    }

    void Start()
    {
        if (line)
        {
            line.positionCount = 2;
            line.SetPosition(0, Vector2.zero);
            line.SetPosition(1, Vector2.zero);
            line.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            isDragging = true;
            startPos = MouseWorld;
            if (line) { line.enabled = true; line.SetPosition(0, startPos); }
        }

        if (isDragging)
        {
            var cur = MouseWorld;
            var delta = cur - startPos;
            if (delta.magnitude > dragLimit)
                cur = startPos + delta.normalized * dragLimit;
            if (line) line.SetPosition(1, cur);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            if (line) line.enabled = false;

            var a = line ? line.GetPosition(0) : startPos;
            var b = line ? line.GetPosition(1) : MouseWorld;

            Vector2 dir = (b - a);
            float strength01 = Mathf.Clamp01(dir.magnitude / dragLimit);

            oneShot.Launch(dir, strength01);
        }
    }
}
