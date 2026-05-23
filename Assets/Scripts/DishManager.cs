using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DishManager : MonoBehaviour
{
    [Header("Dish Models (match order to dishes.json)")]
    public GameObject[] dishModels; 

    [Header("Panel Text Fields")]
    public TMP_Text restaurantText;   // NameTent
    public TMP_Text mealText;         // NameTent
    public TMP_Text ingredientsText;  // RightPanel
    public TMP_Text allergensText;    // RightPanel
    public TMP_Text addressText;      // LeftPanel

    public TMP_Text areaText;

    public TMP_Text cuisineTypeText;

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
        if (areaText) areaText.text = "Area:\n" + dish.area;
        if (cuisineTypeText) cuisineTypeText.text = "Cuisine Type:\n" + dish.cusine_type;

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