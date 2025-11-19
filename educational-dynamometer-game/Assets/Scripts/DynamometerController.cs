using UnityEngine;
using TMPro;

public class DynamometerController : MonoBehaviour
{
    [Header("Settings")]
    public float PlanetGravity = 9.81f; 
    public float NewtonsPerLine = 10f; 
    public float ElongationPerLine = 0.1f; 
    
    [Header("References")]
    public Transform InnerRod; // Hareket eden çubuk
    public Transform HookTip;  // Kancanın ucu (İlk bağlantı noktası)

    [Header("UI")]
    public TextMeshProUGUI InfoText;

    private Vector3 _initialRodPosition;

    void Start()
    {
        if (InnerRod != null) _initialRodPosition = InnerRod.localPosition;

        // --- YENİ EKLENECEK KISIM ---
        // Unity'nin fizik dünyasındaki global yerçekimini değiştiriyoruz.
        // Vector3(0, -9.81, 0) yerine Vector3(0, -PlanetGravity, 0) yapıyoruz.
        // Eksi işareti önemli, yoksa nesneler uzaya uçar!
        Physics.gravity = new Vector3(0, -Mathf.Abs(PlanetGravity), 0);
    }

    // --- ZİNCİRİN EN ALTI NERESİ? ---
    // DragSpawner bu fonksiyonu soracak: "Yeni geleni nereye takayım?"
    public Transform GetChainTail()
    {
        // 1. Kancadan başla
        Transform currentLink = HookTip;
        
        // 2. Aşağı doğru in: Bağlı olan bir sonraki joint var mı?
        // (Sonsuz döngü olmasın diye while yerine for ile limit koyabilirsin ama şimdilik basit tutalım)
        int safeGuard = 0;
        while (safeGuard < 50) // Max 50 nesne sınırı (Crash olmasın diye)
        {
            // Şu anki linkin (currentLink) bağlı olduğu bir Joint var mı?
            // Dikkat: Joint component'i nesnenin kendisinde olur ve 'connectedBody' ile YUKARIYI tutar.
            // Biz ise AŞAĞIDAKİ nesneyi bulmalıyız.
            
            // Bu yüzden sahnedeki tüm FixedJoint'leri tarayıp "Ucu bana bağlı olan var mı?" diye sormak yerine;
            // En son eklenen nesneyi bulmak için basit bir yöntem izleyeceğiz:
            // Bir önceki nesnenin 'ConnectionPoint'ine bakacağız.
            
            // FAKAT EN BASİT YÖNTEM ŞUDUR:
            // DragSpawner'da hesaplarız. Burası sadece TOPLAM AĞIRLIĞI hesaplasın.
            break; 
        }
        
        // Basit Yöntem: DragSpawner'da "GetChainTail" mantığını oraya yazacağız.
        // Burası sadece ağırlık hesaplasın.
        return HookTip;
    }

    // YENİ YÖNTEM: DragSpawner bize neyin takılı olduğunu söylesin, biz sadece matematiği yapalım.
    public void UpdateVisuals(float currentTotalMass)
    {
        float weightNewton = currentTotalMass * PlanetGravity;
        
        float linesNeeded = weightNewton / NewtonsPerLine;
        float totalStretch = linesNeeded * ElongationPerLine;

        Vector3 targetPos = _initialRodPosition + new Vector3(0, -totalStretch, 0);
        InnerRod.localPosition = targetPos;
        
        Debug.Log($"Toplam Kütle: {currentTotalMass} kg -> {weightNewton} N");

        if (InfoText != null)
        {
            weightNewton = currentTotalMass * PlanetGravity;
            
            InfoText.text = 
                           
                            $"KUTLE  : {currentTotalMass:F1} kg\n" +
                            $"KUVVET : {weightNewton:F2} N";
        }
    }
}