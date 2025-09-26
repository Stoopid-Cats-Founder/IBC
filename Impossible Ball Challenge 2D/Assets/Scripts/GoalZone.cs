using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoalZone : MonoBehaviour
{
    Cup cup;

    void Awake()
    {
        cup = GetComponentInParent<Cup>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ball")) return;
        if (cup && !cup.IsOpen) return; // only score when lid open

        var gm = FindFirstObjectByType<GameManagerSimple>();
        if (gm) gm.OnLevelCompleted();
    }
}
