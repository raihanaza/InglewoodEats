using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DishManager : MonoBehaviour
{
    [Header("Dish Models (match order to dishes.json)")]
    public GameObject[] dishModels; // slot 0 = littlebelize, slot 1 = lafonda

    [Header("Panel Text Fields")]
    public TMP_Text restaurantText;   // NameTent
    public TMP_Text mealText;         // NameTent
    public TMP_Text ingredientsText;  // RightPanel
    public TMP_Text allergensText;    // RightPanel
    public TMP_Text addressText;      // LeftPanel

    [Header("Animation")]
    public DishAnimation dishAnimation; // drag the Plate here

    private DishList dishList;
    private int currentIndex = 0;

    void Start()
    {
        LoadDishes();
        ShowDish(currentIndex);
    }

    void LoadDishes()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("dishes");
        if (jsonFile == null)
        {
            Debug.LogError("DishManager: Could not find dishes.json in Resources folder!");
            return;
        }
        dishList = JsonUtility.FromJson<DishList>(jsonFile.text);
        Debug.Log($"DishManager: Loaded {dishList.dishes.Count} dishes.");
    }

    void ShowDish(int index)
    {
        if (dishList == null || dishList.dishes.Count == 0) return;

        Dish dish = dishList.dishes[index];

        // Update panels
        if (restaurantText) restaurantText.text = dish.restaurant;
        if (mealText) mealText.text = dish.meal_name;
        if (ingredientsText) ingredientsText.text = "Ingredients:\n" + string.Join("\n", dish.ingredients);
        if (allergensText) allergensText.text = "Allergens:\n" + string.Join("\n", dish.allergens);
        if (addressText) addressText.text = dish.address;

        // Show correct 3D model, hide others
        for (int i = 0; i < dishModels.Length; i++)
        {
            if (dishModels[i] != null)
                dishModels[i].SetActive(i == index);
        }

        Debug.Log($"DishManager: Showing dish {index} — {dish.meal_name}");
    }

    void NextDish()
    {
        currentIndex = (currentIndex + 1) % dishList.dishes.Count;
        ShowDish(currentIndex);
    }

    public void OnDishPulled()
    {
        Debug.Log("Action: PULL — User wants to eat now!");
        if (dishAnimation != null)
            dishAnimation.PlayPullAnimation(() => { });
    }

    public void OnDishPushed()
    {
        Debug.Log("Action: PUSH — Moving to next dish.");
        if (dishAnimation != null)
            dishAnimation.PlayPushAnimation(() => {
                NextDish();
                // Re-enable and re-animate the plate
                if (dishAnimation != null)
                    dishAnimation.gameObject.SetActive(true);
            });
    }

    public void OnDishFavorited()
    {
        Debug.Log("Action: FORK — User favorited this dish!");
        // TODO: Backlog to add to favorites/saved
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame) OnDishPulled();
        if (Keyboard.current.xKey.wasPressedThisFrame) OnDishPushed();
    }
}










// using UnityEngine;
// using UnityEngine.InputSystem;

// // Manages the current dish on the table and handles user actions.

// public class DishManager : MonoBehaviour
// {
//     [Header("Current Dish on the Table")]
//     public GameObject currentDish;
//     private DishAnimation dishAnimation;

//     void Awake()
//     {
//         if (currentDish != null)
//         {
//             dishAnimation = currentDish.GetComponent<DishAnimation>();
//             Debug.Log(dishAnimation != null ? "DishAnimation found!" : "DishAnimation NOT found!");
//         }
//         else
//         {
//             Debug.LogWarning("currentDish is not assigned in Inspector!");
//         }
//     }

//     public void OnDishPulled()
//     {
//         Debug.Log("Action: PULL — User wants to eat now!");
//         if (dishAnimation != null)
//             dishAnimation.PlayPullAnimation(() => { });
//         else
//             HideDish();
//     }

//     public void OnDishPushed()
//     {
//         Debug.Log("Action: PUSH — User is not interested.");

//         if (dishAnimation != null)
//             dishAnimation.PlayPushAnimation(() => HideDish());
//         else
//             HideDish();
//     }

//     // TODO: Called when the user picks up the FORK.
//     public void OnDishFavorited()
//     {
//         Debug.Log("Action: FORK — User favorited this dish!");

//         HideDish();

//         // TODO: Save this restaurant to a favorites list
//     }

//     private void HideDish()
//     {
//         if (currentDish != null)
//         {
//             currentDish.SetActive(false);
//         }
//         else
//         {
//             Debug.LogWarning("DishManager: No dish assigned! Drag your Plate into the Inspector.");
//         }
//     }

//     void Update()
//     {
//         // Press P to test Pull
//         if (Keyboard.current.pKey.wasPressedThisFrame)
//             OnDishPulled();

//         // Press X to test Push  
//         if (Keyboard.current.xKey.wasPressedThisFrame)
//             OnDishPushed();
//     }
// }