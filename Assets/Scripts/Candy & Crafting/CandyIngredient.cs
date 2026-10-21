using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class CandyIngredient : MonoBehaviour, IPickUp, IInteractable
{
    //Ingredient spawners will override this field with their Scriptable Object of Ingredient and then the code will follow the start function updating the ingredient to be the new item
    public CandyItem_ScriptableObjects candyIngredient; 

    private MeshFilter m_filter;

    [SerializeField] private Sprite inventoryImg;
    public Sprite InventoryImg => inventoryImg;

    #region IInteractable addressments
    public bool canInteract { get; set; } = true;

    public string[] denyText { get; set; }

    public void Interact()
    {
        PlayerInventory playerInventory = GameObject.FindAnyObjectByType<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogError("CANDY INGREDIENT - CRITICAL ERROR THE PLAYER INVENTORY IS NOT FOUND");
            return;
        }

        playerInventory.PickUpItem(gameObject);
    }
    #endregion

    private void Start()
    {
        m_filter = GetComponent<MeshFilter>();
        if (m_filter != null && candyIngredient != null && candyIngredient.model)
        {
            m_filter.mesh = candyIngredient.model;

            BoxCollider collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.center = m_filter.mesh.bounds.center;
                collider.size = m_filter.mesh.bounds.size;
            }

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
