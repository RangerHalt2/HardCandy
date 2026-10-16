using UnityEngine;

//A reusable "taste profile" for NPCs. Make one per personality (Sweet Tooth, Sour Lover, etc.) and assign it to an NPC or the spawner
[CreateAssetMenu(menuName = "NPC/Taste")]
public class NPCTaste_ScriptableObjects : ScriptableObject
{
    public string TasteName;

    [Tooltip("Candy with this trait is the NPC's favorite, they pay extra for it")]
    public Traits_ScriptableObjects FavoriteTrait;

    [Tooltip("Candy with ANY of these traits will be refused")]
    public Traits_ScriptableObjects[] RefusedTraits;
}
