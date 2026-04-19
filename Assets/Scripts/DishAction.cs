using UnityEngine;

// Tracks plate gesture. Both hands must be holding for push/pull to register.

public class DishAction : MonoBehaviour
{
    [Header("Connect to DishManager")]
    public DishManager dishManager;

    [Header("Sensitivity Settings")]
    public float pullThreshold = 0.3f;
    public float pushThreshold = 0.3f;

    private Vector3 startPosition;
    private bool isBeingHeld = false;

    // Called when second hand grabs so both hands are now holding
    public void OnGrabbed()
    {
        startPosition = transform.position;
        isBeingHeld = true;

        Debug.Log("Both hands holding. Start position recorded.");
    }

    // Called when a hand releases while both were holding.
    public void OnReleased()
    {
        if (!isBeingHeld) return;

        isBeingHeld = false;

        float zMovement = transform.position.z - startPosition.z;

        Debug.Log($"Z movement: {zMovement}");

        if (zMovement < -pullThreshold)
        {
            Debug.Log("Detected: PULL");
            dishManager.OnDishPulled();
        }
        else if (zMovement > pushThreshold)
        {
            Debug.Log("Detected: PUSH");
            dishManager.OnDishPushed();
        }
        else
        {
            Debug.Log("Not far enough — snapping back.");
            transform.position = startPosition;
        }
    }
}