using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BounceWallParticles : MonoBehaviour
{
    public string sparkKey = "NormalWall"; // match one of SparkPool keys

    private SparkPool sparkPool;

    void Awake()
    {
        sparkPool = FindFirstObjectByType<SparkPool>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ball") && col.contactCount > 0)
        {
            ContactPoint2D contact = col.contacts[0];
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, contact.normal);

            // play sparks
            if (sparkPool != null) sparkPool.PlaySpark(sparkKey, contact.point, rot);

            // play sound
            AudioManager.Instance?.PlayRandomWallNote();
        }
    }

}
