using UnityEngine;

[CreateAssetMenu(menuName = "CandyItem/Traits")]
public class Traits_ScriptableObjects : ScriptableObject
{
    public string TraitName;
    public string TraitNameMod; //This will be used in the candy naming convention
    public string TraitDescription;

}
