using UnityEngine;

[CreateAssetMenu(menuName = "CandyItem/Ingredient")]
public class CandyItem_ScriptableObjects : ScriptableObject
{
    public string IngredientName;
    public Traits_ScriptableObjects[] IngredientTraits;

    public Mesh model;
}
