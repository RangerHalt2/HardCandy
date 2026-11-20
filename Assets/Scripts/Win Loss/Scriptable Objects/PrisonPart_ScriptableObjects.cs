using UnityEngine;

//One buyable part of the prison (Cafeteria, Yard, etc). Make one asset per part
[CreateAssetMenu(menuName = "Prison/Part")]
public class PrisonPart_ScriptableObjects : ScriptableObject
{
    public string PartName;
    public int Cost = 100;
}
