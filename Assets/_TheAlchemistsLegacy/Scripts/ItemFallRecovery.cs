using UnityEngine;

// Attach to a large flat trigger box placed well below each level's floor.
// Any Pickup-tagged item that falls out of bounds is teleported back to the
// recovery point (drag a Transform in the Inspector, e.g. scene centre at
// floor height). Held items are excluded — they can't fall while parented.
public class ItemFallRecovery : MonoBehaviour
{
    [Tooltip("Where recovered items reappear. Drag a scene Transform here.")]
    public Transform recoveryPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Pickup"))
            return;

        // Don't touch items the player is currently holding
        if (ItemPickup.instance != null && ItemPickup.instance.currentItem == other.gameObject)
            return;

        Transform root = other.transform;
        while (root.parent != null)
            root = root.parent;

        Vector3 target = recoveryPoint != null
            ? recoveryPoint.position
            : new Vector3(0f, 1f, 0f);

        root.position = target;

        Rigidbody rb = root.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"[ItemFallRecovery] Recovered {root.name} to {target}");
    }
}
