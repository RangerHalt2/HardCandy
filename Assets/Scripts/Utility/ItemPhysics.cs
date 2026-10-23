using UnityEngine;

public static class ItemPhysics
{
    public static void SetKinematic(GameObject item, bool kinematic)
    {
        Rigidbody rb = item.GetComponentInChildren<Rigidbody>();
        if (rb == null) rb = item.GetComponentInParent<Rigidbody>();
        if (rb == null) return;

        rb.isKinematic = kinematic;
        if (!kinematic)
        {
            rb.linearVelocity = Vector3.zero;    // use rb.velocity on Unity 2022 and older
            rb.angularVelocity = Vector3.zero;
        }
    }
}