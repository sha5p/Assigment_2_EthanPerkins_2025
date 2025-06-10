using PathCreation;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Vehicle Spawning")]
    public GameObject vehiclePrefab;
    public List<PathCreator> vehiclePathsToSpawnOn;
    public int numberOfVehiclesPerPath = 3;
    public Vector3 vehicleRotationOffset = new Vector3(0, 0, 0);

    [Header("NPC Spawning")]
    public GameObject npcPrefab;
    public List<PathCreator> npcPathsToSpawnOn;
    public int numberOfNpcsPerPath = 5;
    public Vector3 npcRotationOffset = new Vector3(0, 0, 0);

    [Header("General Spawning Settings")]
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 15f;
    public float spawnHeight = 0f; 

    void Start()
    {
        SpawnEntities(vehiclePrefab, vehiclePathsToSpawnOn, numberOfVehiclesPerPath, vehicleRotationOffset);
        SpawnEntities(npcPrefab, npcPathsToSpawnOn, numberOfNpcsPerPath, npcRotationOffset);
    }

    void SpawnEntities(GameObject prefab, List<PathCreator> paths, int numberOfEntitiesPerPath, Vector3 rotationOffset)
    {
        if (prefab == null)
        {
            Debug.LogError($"Prefab not assigned in Spawner!");
            return;
        }

        if (paths == null || paths.Count == 0)
        {
            Debug.LogWarning($"No paths assigned for {prefab.name} spawning.");
            return;
        }

        foreach (PathCreator pathCreator in paths)
        {
            SpawnEntitiesOnPath(prefab, pathCreator, numberOfEntitiesPerPath, rotationOffset);
        }
    }

    void SpawnEntitiesOnPath(GameObject prefab, PathCreator pathCreator, int numberOfEntities, Vector3 rotationOffset)
    {
        if (pathCreator == null) return;

        List<float> occupiedDistances = new List<float>();
        float pathLength = pathCreator.path.length;

        for (int i = 0; i < numberOfEntities; i++)
        {
            float spawnDistance;
            bool validPosition = false;
            int attempts = 0;
            int maxAttempts = 10;

            do
            {
                spawnDistance = Random.Range(0f, pathLength);
                validPosition = true;

                foreach (float occupiedDistance in occupiedDistances)
                {
                    if (Mathf.Abs(spawnDistance - occupiedDistance) < minSpawnDistance)
                    {
                        validPosition = false;
                        break;
                    }
                }

                attempts++;
                if (attempts > maxAttempts)
                {
                    Debug.LogWarning($"Could not find a suitable spawn position for entity {i + 1} (prefab: {prefab.name}) on {pathCreator.gameObject.name} after {maxAttempts} attempts.");
                    break;
                }

            } while (!validPosition);

            if (validPosition)
            {
                Vector3 spawnPosition = pathCreator.path.GetPointAtDistance(spawnDistance, EndOfPathInstruction.Stop);
                Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(spawnDistance, EndOfPathInstruction.Stop);

                spawnRotation *= Quaternion.Euler(rotationOffset);

                spawnPosition.y = spawnHeight;

                GameObject newEntity = Instantiate(prefab, spawnPosition, spawnRotation);

                NPC_Movment npcMovementScript = newEntity.GetComponent<NPC_Movment>();
                if (npcMovementScript != null)
                {
                    npcMovementScript.pathCreator = pathCreator;
                }
                else
                {
                    Follow followScript = newEntity.GetComponent<Follow>();
                    if (followScript != null)
                    {
                        followScript.PathCreator = pathCreator;
                        followScript.distanceTraveled = spawnDistance;
                    }
                    else
                    {
                        Debug.LogWarning($"Spawned prefab {prefab.name} does not have a 'NPC_Movment' or 'Follow' script attached. Please ensure your prefabs have the correct movement script.");
                    }
                }

                occupiedDistances.Add(spawnDistance);
            }
        }
    }
}