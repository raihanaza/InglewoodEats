using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dish
{
    public string id;
    public string meal_name;
    public string restaurant;
    public List<string> ingredients;
    public List<string> allergens;
    public string address;
    public string area;
    public bool picture;
    public string cusine_type;
}

[Serializable]
public class DishList
{
    public List<Dish> dishes;
}