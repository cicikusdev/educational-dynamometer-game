using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool IsMouseOverTrash = false;
    
    private Vector3 _originalScale; // Başlangıç boyutunu burada saklayacağız

    void Start()
    {
        // Oyun başladığında nesnenin o anki scale değerini (2.25) kaydediyoruz
        _originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseOverTrash = true;
        
        // Orijinal boyutun %20 fazlasına büyüt (2.25 * 1.2)
        transform.localScale = _originalScale * 1.2f; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOverTrash = false;
        
        // Orijinal boyutuna geri döndür (2.25)
        transform.localScale = _originalScale; 
    }

    void OnDisable()
    {
        IsMouseOverTrash = false;
        // Nesne kapanırken de boyutu eski haline getirelim ki sonraki açılışta dev gibi başlamasın
        transform.localScale = _originalScale; 
    }
}