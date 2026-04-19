using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Focuses on how many hands are interacting with the dish. Both hands must be holding for the gesture to count.

public class DishInteractable : MonoBehaviour
{
    private DishAction dishAction;
    private XRGrabInteractable grabInteractable;

    private int handCount = 0;

    void Awake()
    {
        dishAction = GetComponent<DishAction>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (dishAction == null)
            Debug.LogError("DishInteractable: Missing DishAction on " + gameObject.name);
        if (grabInteractable == null)
            Debug.LogError("DishInteractable: Missing XRGrabInteractable on " + gameObject.name);
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnHandGrabbed);
        grabInteractable.selectExited.AddListener(OnHandReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnHandGrabbed);
        grabInteractable.selectExited.RemoveListener(OnHandReleased);
    }

    private void OnHandGrabbed(SelectEnterEventArgs args)
    {
        handCount++;
        Debug.Log($"Hand grabbed. Total hands: {handCount}");

        if (handCount == 2)
        {
            // Both hands are now holding 
            dishAction.OnGrabbed();
        }
    }

    private void OnHandReleased(SelectExitEventArgs args)
    {
        if (handCount == 2)
        {
            dishAction.OnReleased();
        }

        handCount--;
        if (handCount < 0) handCount = 0;

        Debug.Log($"Hand released. Total hands: {handCount}");
    }
}