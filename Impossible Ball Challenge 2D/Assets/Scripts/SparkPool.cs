using UnityEngine;
using System.Collections.Generic;

public class SparkPool : MonoBehaviour
{
    [System.Serializable]
    public class PoolEntry
    {
        public string key;                // e.g. "NormalWall", "SuperWall"
        public ParticleSystem prefab;     // prefab to spawn
        public int poolSize = 10;
    }

    public List<PoolEntry> prefabs = new List<PoolEntry>();

    private Dictionary<string, Queue<ParticleSystem>> pools = new();

    void Awake()
    {
        foreach (var entry in prefabs)
        {
            var q = new Queue<ParticleSystem>();
            for (int i = 0; i < entry.poolSize; i++)
            {
                ParticleSystem ps = Instantiate(entry.prefab, transform);
                ps.gameObject.SetActive(false);
                q.Enqueue(ps);
            }
            pools[entry.key] = q;
        }
    }

    public void PlaySpark(string key, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(key))
        {
            Debug.LogWarning($"No spark pool for key {key}");
            return;
        }

        var q = pools[key];
        var ps = q.Dequeue();

        ps.transform.position = position;
        ps.transform.rotation = rotation;

        ps.gameObject.SetActive(true);
        ps.Play();

        StartCoroutine(ReturnToPool(key, ps, ps.main.duration + ps.main.startLifetime.constantMax));
        q.Enqueue(ps);
    }

    private System.Collections.IEnumerator ReturnToPool(string key, ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        ps.Stop();
        ps.gameObject.SetActive(false);
    }
}
