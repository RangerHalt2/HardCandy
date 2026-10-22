using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerRelay : MonoBehaviour
{
    public event Action<Collider> Entered;
    public event Action<Collider> Exited;

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Ingredient"))
        {
            return;
        }
        Entered?.Invoke(other);
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Ingredient"))
        {
            return;
        }
            Exited?.Invoke(other);
    }
}