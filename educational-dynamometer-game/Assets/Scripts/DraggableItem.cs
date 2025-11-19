using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Rigidbody _rb;
    private float _zDistance;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // --- 1. SÜRÜKLEME BAŞLADIĞINDA ---
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Kanca Bağını Kopar
        FixedJoint joint = GetComponent<FixedJoint>();
        if (joint != null)
        {
            Destroy(joint);
            
            // Ağırlığı güncelle
            if (DragSpawner.Instance != null) 
                DragSpawner.Instance.Invoke("RecalculateAllWeight", 0.05f);
        }

        // 2. Fiziği Tamamen Kapat (Glitchlenmeyi önleyen en önemli ayar)
        _rb.isKinematic = true; // Yerçekimi ve çarpmalar iptal
        _rb.useGravity = false;
        
        // 3. Derinliği Hesapla
        _zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
    }

    // --- 2. SÜRÜKLERKEN (HER FRAME ÇALIŞIR) ---
    public void OnDrag(PointerEventData eventData)
    {
        // Mouse pozisyonuna pürüzsüz git
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _zDistance; 
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        
        transform.position = worldPos;
    }

    // --- 3. BIRAKINCA ---
    public void OnEndDrag(PointerEventData eventData)
    {
        // A) ÇÖP KUTUSU KONTROLÜ (ZİNCİRLEME SİLME)
        if (TrashBin.IsMouseOverTrash)
        {
            // Kendimi ve bana bağlı olan alt zinciri sil
            DestroyChainRecursive(this.gameObject);
            
            // Ağırlığı güncelle (Eğer zincirin ortasından silindiyse)
            if (DragSpawner.Instance != null) 
                DragSpawner.Instance.RecalculateAllWeight();
        }
        // B) KANCAYA GERİ TAKMA KONTROLÜ (YENİ!)
        else if (DragSpawner.Instance != null)
        {
            Transform target = DragSpawner.Instance.GetTargetTransform();
            float distance = Vector3.Distance(transform.position, target.position);
            
            // Spawner'daki mesafe ayarını kullan
            if (distance <= DragSpawner.Instance.SnapDistance)
            {
                // Spawner'ın takma fonksiyonunu kullan
                DragSpawner.Instance.AttachToChain(this.gameObject, target);
            }
            else
            {
                // Boşluğa bırak (Yere düşsün)
                DropItem();
            }
        }
        else
        {
            DropItem();
        }
    }

    private void DropItem()
    {
        _rb.isKinematic = false; 
        _rb.useGravity = true;   
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    // --- ZİNCİRLEME SİLME FONKSİYONU ---
    private void DestroyChainRecursive(GameObject objToDelete)
    {
        // 1. Bu objenin altına bağlı bir nesne var mı bul
        // (Bunu yapmak için sahnedeki jointleri tarıyoruz)
        GameObject child = FindChildConnectedTo(objToDelete.GetComponent<Rigidbody>());
        
        // 2. Varsa, önce onu sil (Recursive: kendini tekrar çağır)
        if (child != null)
        {
            DestroyChainRecursive(child);
        }

        // 3. Alt taraf temizlendikten sonra kendisi silinsin
        Destroy(objToDelete);
    }

    private GameObject FindChildConnectedTo(Rigidbody parentRB)
    {
        // Bu fonksiyon biraz ağır çalışır ama sadece silerken çağrıldığı için sorun olmaz.
        FixedJoint[] allJoints = FindObjectsOfType<FixedJoint>();
        foreach(FixedJoint joint in allJoints)
        {
            if (joint.connectedBody == parentRB)
            {
                return joint.gameObject;
            }
        }
        return null;
    }
}