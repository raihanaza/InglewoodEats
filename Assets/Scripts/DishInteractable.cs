using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Focuses on how many hands are interacting with the dish. Both hands must be holding for the gesture to count.
// Gives the plate properties 
// Every interaction that the user is doing, you have to write these rules
// Recognize how many hands, which will lead to other things 
// This is why we need to write this script outside the other files, because we want to separate the logic of how many hands are holding from the actual dish action (pull/push).
// EXPLAIN: What rules do i need to write to make this apply. 

// Explain purpose not the what

// POST ITS
// DishInteractiavle - responsible for ...
// DishAction - responsible for ...
// PlateSpawner - responsible for ...

// Students should place the user flow (post its) to really understand the purpose of this script

// why is it necessary to separarte the scripts? 
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

        if (handCount == 1)
        {
            // Both hands are now holding 
            dishAction.OnGrabbed();
        }
    }

    private void OnHandReleased(SelectExitEventArgs args)
    {
        if (handCount == 1)
        {
            dishAction.OnReleased();
        }

        handCount--;
        if (handCount < 0) handCount = 0;

        Debug.Log($"Hand released. Total hands: {handCount}");
    }
}