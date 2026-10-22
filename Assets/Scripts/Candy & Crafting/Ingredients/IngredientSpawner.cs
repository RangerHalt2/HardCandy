using UnityEngine;

public class IngredientSpawner : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject ingredient;
    [SerializeField] private Transform spawnPoint;

    #region Interactable Addressments
    public bool canInteract { get; set; } = true;

    public string[] denyText { get; set; }

    public void Interact()
    {
        if (!canInteract) return;
        SpawnIngredient();  
    }
    #endregion


    private void SpawnIngredient()
    {
        GameObject spawnedIngredient = Instantiate(ingredient, spawnPoint.position, Quaternion.identity);
    }

}
