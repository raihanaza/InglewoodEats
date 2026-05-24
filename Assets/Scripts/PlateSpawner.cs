using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Using AR Foundation package to detect solid plane (a.k.a. table) and have plate appear on it. 
// Has a public method to spawn the plate between the cup and fork positions received from MediaPipeReceiver.
// Unused script in final demo, but can be used for future AR table detection instead of relying on MediaPipe.

public class PlateSpawner : MonoBehaviour
{
    [SerializeField] private GameObject platePrefab;

    [Header("Table Settings")]
    [SerializeField] private float tableHeight = 0.78f;
    [SerializeField] private float tableDepth = 1.0f;

    private ARPlaneManager planeManager;
    private GameObject spawnedPlate;
    private bool plateSpawned = false;

    void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
        if (planeManager == null)
        {
            Debug.LogError("PlateSpawner: No ARPlaneManager found!");
            return;
        }
        planeManager.trackablesChanged.AddListener(OnPlanesChanged);
        Debug.Log("PlateSpawner: Listening for planes...");
    }

    void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> args)
    {
        if (plateSpawned) return;
        foreach (ARPlane plane in args.added)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                SpawnPlateAtPosition(plane.transform.position + Vector3.up * 0.02f);
                return;
            }
        }
    }

    public void OnObjectsDetected(SerializableVector3 cupPos, SerializableVector3 forkPos)
    {
        if (plateSpawned) return;

        Vector3 cup = NormalizedToWorld(cupPos.ToVector3());
        Vector3 fork = NormalizedToWorld(forkPos.ToVector3());
        Vector3 midpoint = (cup + fork) / 2f;
        midpoint.y = tableHeight + 0.02f;

        Debug.Log($"PlateSpawner: Placing plate between cup and fork at {midpoint}");
        SpawnPlateAtPosition(midpoint);
    }

    Vector3 NormalizedToWorld(Vector3 normalized)
    {
        float worldX = (normalized.x - 0.5f) * 2f;
        float worldZ = tableDepth;
        return new Vector3(worldX, tableHeight, worldZ);
    }

    void SpawnPlateAtPosition(Vector3 position)
    {
        if (spawnedPlate != null)
            Destroy(spawnedPlate);

        spawnedPlate = Instantiate(platePrefab, position, Quaternion.identity);
        plateSpawned = true;
        Debug.Log($"PlateSpawner: Plate spawned at {position}");

        if (planeManager != null)
            planeManager.enabled = false;
    }

    void OnDisable()
    {
        if (planeManager != null)
            planeManager.trackablesChanged.RemoveListener(OnPlanesChanged);
    }
}