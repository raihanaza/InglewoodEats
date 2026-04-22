using UnityEngine;
using UnityEngine.InputSystem;

// Manages the current dish on the table and handles user actions.

public class DishManager : MonoBehaviour
{
    [Header("Current Dish on the Table")]
    public GameObject currentDish;
    private DishAnimation dishAnimation;

    void Awake()
    {
        if (currentDish != null)
        {
            dishAnimation = currentDish.GetComponent<DishAnimation>();
            Debug.Log(dishAnimation != null ? "DishAnimation found!" : "DishAnimation NOT found!");
        }
        else
        {
            Debug.LogWarning("currentDish is not assigned in Inspector!");
        }
    }

    public void OnDishPulled()
    {
        Debug.Log("Action: PULL — User wants to eat now!");
        if (dishAnimation != null)
            dishAnimation.PlayPullAnimation(() => { });
        else
            HideDish();
    }

    public void OnDishPushed()
    {
        Debug.Log("Action: PUSH — User is not interested.");

        if (dishAnimation != null)
            dishAnimation.PlayPushAnimation(() => HideDish());
        else
            HideDish();
    }

    // TODO: Called when the user picks up the FORK.
    public void OnDishFavorited()
    {
        Debug.Log("Action: FORK — User favorited this dish!");

        HideDish();

        // TODO: Save this restaurant to a favorites list
    }

    private void HideDish()
    {
        if (currentDish != null)
        {
            currentDish.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DishManager: No dish assigned! Drag your Plate into the Inspector.");
        }
    }

    void Update()
    {
        // Press P to test Pull
        if (Keyboard.current.pKey.wasPressedThisFrame)
            OnDishPulled();

        // Press X to test Push  
        if (Keyboard.current.xKey.wasPressedThisFrame)
            OnDishPushed();
    }
}