using System.Collections.Generic;
using UnityEngine;

//Put this on the candy prefab. CandyController fills in the traits from the ingredients used to make it, NPCs read them to decide if they'll buy it
public class Candy : MonoBehaviour
{
    public string candyName = "Candy";
    public List<Traits_ScriptableObjects> traits = new List<Traits_ScriptableObjects>();

    public bool HasTrait(Traits_ScriptableObjects trait)
    {
        return trait != null && traits.Contains(trait);
    }
}
