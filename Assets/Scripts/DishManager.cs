using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DishManager : MonoBehaviour
{
    [Header("Dish Models (match order to dishes.json)")]
    public GameObject[] dishModels; 

    [Header("Panel Text Fields")]
    public TMP_Text restaurantText;
    public TMP_Text mealText;
    public TMP_Text ingredientsText;
    public TMP_Text allergensText; 
    public TMP_Text addressText;

    public TMP_Text areaText;

    public TMP_Text cuisineTypeText;

    [Header("Animation")]
    public DishAnimation dishAnimation;

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

        // Update text on panels
        if (restaurantText) restaurantText.text = dish.restaurant;
        if (mealText) mealText.text = dish.meal_name;
        if (ingredientsText) ingredientsText.text = "Ingredients:\n" + string.Join("\n", dish.ingredients);
        if (allergensText) allergensText.text = "Allergens:\n" + string.Join("\n", dish.allergens);
        if (addressText) addressText.text = dish.address;
        if (areaText) areaText.text = "Area:\n" + dish.area;
        if (cuisineTypeText) cuisineTypeText.text = "Cuisine Type:\n" + dish.cusine_type;

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
                dishAnimation.ResetPosition();
            });
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame) OnDishPulled();
        if (Keyboard.current.xKey.wasPressedThisFrame) OnDishPushed();
    }
}