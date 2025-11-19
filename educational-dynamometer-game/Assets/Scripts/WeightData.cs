using UnityEngine;

public class WeightData : MonoBehaviour
{
    [Header("Object Info")]
    public string ItemName = "Item";
    public float MassKG = 1.0f; 

    [Header("Chain Settings")]
    // BURASI ÖNEMLİ: Değişkenin adı "BottomConnectionPoint" olmalı.
    // Çünkü DragSpawner.cs bu ismi kullanıyor.
    public Transform BottomConnectionPoint; // Altına başka nesne takılacak nokta
    
    public Transform TopAttachmentPoint;    // Kendisinin kancaya asılacağı nokta

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.mass = MassKG;
    }
}