using UnityEngine;

/// <summary>
/// If vsync is enabled, it will likely dominate the CPU graph at the start.
/// To get a better idea of how much CPU resources our scene needs, turn vsync off.
/// You can do this via Edit / Project Settings / Quality. It is found at the bottom, under the Other heading.
/// </summary>

public class NucleonSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public float spawnDistance;
    public Nucleon[] nucleonPrefabs;

    float timeSinceLastSpawn;

    void FixedUpdate()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= timeBetweenSpawns)
        {
            timeSinceLastSpawn -= timeBetweenSpawns;
            SpawnNucleon();
        }
    }

    void SpawnNucleon()
    {
        Nucleon prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
        Nucleon spawn = Instantiate(prefab);
        spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }
}