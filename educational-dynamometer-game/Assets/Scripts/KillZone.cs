using UnityEngine;

public class KillZone : MonoBehaviour
{
    // Trigger alanına bir şey girdiği an çalışır
    private void OnTriggerEnter(Collider other)
    {
        // Sadece "DraggableItem" scripti olanları veya Rigidbody'si olanları yok et
        // Böylece yanlışlıkla oyuncuyu veya kamerayı silmezsin.
        if (other.GetComponent<DraggableItem>() != null || other.attachedRigidbody != null)
        {
            Destroy(other.gameObject);
        }
    }
}