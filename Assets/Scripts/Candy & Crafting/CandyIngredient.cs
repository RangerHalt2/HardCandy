using System;
using UnityEngine;

public class CandyIngredient : MonoBehaviour
{
    //Ingredient spawners will override this field with their Scriptable Object of Ingredient and then the code will follow the start function updating the ingredient to be the new item
    public CandyItem_ScriptableObjects candyIngredient; 

    private MeshFilter m_filter;

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
