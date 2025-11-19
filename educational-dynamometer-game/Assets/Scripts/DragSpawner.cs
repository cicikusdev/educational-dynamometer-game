using UnityEngine;
using UnityEngine.EventSystems;

public class DragSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // --- SINGLETON (HER YERDEN ERİŞİM İÇİN) ---
    public static DragSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }
    // -------------------------------------------

    [Header("Prefab")]
    public GameObject Item3DPrefab; 
    
    [Header("References")]
    public Transform DynamometerHook; 
    
    [Header("Settings")]
    public float SnapDistance = 1.5f; 

    private GameObject _current3DItem; 

    // --- SÜRÜKLEME BAŞLADI ---
   public void OnBeginDrag(PointerEventData eventData)
    {
        _current3DItem = Instantiate(Item3DPrefab);
        
        // --- KRİTİK DÜZELTME BURASI ---
        // Nesne doğduğu an fiziğini kapatıyoruz ki elimizden kayıp düşmesin.
        Rigidbody rb = _current3DItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Fizik motorunu durdur (Çarpma/Düşme yok)
            rb.useGravity = false; // Yerçekimini kapat
            
            // Varsa dönüş hızını da sıfırla
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        // -----------------------------

        MoveObjectToMouse(GetTargetTransform(), _current3DItem);
    }

    // --- SÜRÜKLERKEN ---
    public void OnDrag(PointerEventData eventData)
    {
        if (_current3DItem == null) return;
        MoveObjectToMouse(GetTargetTransform(), _current3DItem);
    }

    // --- BIRAKINCA ---
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_current3DItem == null) return;
        
        Transform target = GetTargetTransform();
        float distance = Vector3.Distance(_current3DItem.transform.position, target.position);

        if (distance <= SnapDistance)
        {
            AttachToChain(_current3DItem, target);
        }
        else if (TrashBin.IsMouseOverTrash)
        {
            Destroy(_current3DItem);
            RecalculateAllWeight(); 
        }
        else
        {
            EnablePhysics(_current3DItem);
        }
        
        _current3DItem = null;
    }

    // --- PUBLIC YAPILAN FONKSİYONLAR (DraggableItem Erişsin Diye) ---

    // 1. Hedef Bulucu (Zincirin sonu neresi?)
    public Transform GetTargetTransform()
    {
        Transform currentTarget = DynamometerHook;
        GameObject currentObj = DynamometerHook.gameObject;
        
        for(int i=0; i<20; i++) 
        {
            GameObject childObj = FindObjectConnectedTo(currentObj.GetComponent<Rigidbody>());
            
            if (childObj != null)
            {
                currentObj = childObj;
                WeightData wd = currentObj.GetComponent<WeightData>();
                if (wd != null && wd.BottomConnectionPoint != null) // İsim güncellemesi
                {
                    currentTarget = wd.BottomConnectionPoint;
                }
                else
                {
                    currentTarget = currentObj.transform; 
                }
            }
            else
            {
                break; 
            }
        }
        return currentTarget;
    }

    // 2. Takma İşlemi
    public void AttachToChain(GameObject item, Transform targetTransform)
    {
        item.transform.rotation = Quaternion.identity;

        WeightData wd = item.GetComponent<WeightData>();
        if (wd != null && wd.TopAttachmentPoint != null)
        {
            Vector3 centerToTopOffset = wd.TopAttachmentPoint.position - item.transform.position;
            item.transform.position = targetTransform.position - centerToTopOffset;
        }
        else
        {
            item.transform.position = targetTransform.position;
        }

        FixedJoint joint = item.AddComponent<FixedJoint>();
        Rigidbody targetRB = targetTransform.GetComponentInParent<Rigidbody>();
        if (targetRB != null) joint.connectedBody = targetRB;

        EnablePhysics(item, false); // Asılı kalsın
        RecalculateAllWeight();
    }

    // 3. Ağırlık Hesapla
    public void RecalculateAllWeight()
    {
        DynamometerController controller = DynamometerHook.GetComponentInParent<DynamometerController>();
        if (controller == null) return;

        float totalMass = 0;
        GameObject currentObj = DynamometerHook.gameObject;
        for(int i=0; i<20; i++)
        {
            GameObject child = FindObjectConnectedTo(currentObj.GetComponent<Rigidbody>());
            if (child != null)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null) totalMass += rb.mass;
                currentObj = child;
            }
            else break;
        }
        controller.UpdateVisuals(totalMass);
    }

    // --- YARDIMCI FONKSİYONLAR ---
    
    private GameObject FindObjectConnectedTo(Rigidbody parentRB)
    {
        FixedJoint[] allJoints = FindObjectsOfType<FixedJoint>();
        foreach(FixedJoint joint in allJoints)
        {
            if (joint.connectedBody == parentRB) return joint.gameObject;
        }
        return null;
    }

    private void MoveObjectToMouse(Transform target, GameObject item)
    {
        float targetZ = Mathf.Abs(Camera.main.transform.position.z - target.position.z);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = targetZ;
        item.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
    
    private void EnablePhysics(GameObject item, bool useGravity = true)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = useGravity; 
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // Dönmeyi de durdur
        }
    }
}