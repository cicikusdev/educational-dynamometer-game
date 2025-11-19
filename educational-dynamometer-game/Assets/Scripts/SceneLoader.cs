using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli

public class SceneLoader : MonoBehaviour
{
    // Inspector'dan sürükleyeceğimiz UI panelleri
    public GameObject MainMenuPanel; // Ana menü butonlarının olduğu panel (Play, Exit)
    public GameObject PlanetSelectionPanel; // Dünya, Ay butonlarının olduğu panel

    void Start()
    {
        // Oyun başladığında sadece ana menü gözüksün
        MainMenuPanel.SetActive(true);
        PlanetSelectionPanel.SetActive(false);
    }

    // "Oyna" butonuna basıldığında
    public void ShowPlanetSelection()
    {
        
        PlanetSelectionPanel.SetActive(true);
    }

    // "Dünya" butonuna basıldığında
    public void LoadEarthScene()
    {
        SceneManager.LoadScene("EarthScene"); // Sahnenin adını doğru yaz!
    }

    // "Ay" butonuna basıldığında
    public void LoadMoonScene()
    {
        SceneManager.LoadScene("MoonScene"); // Sahnenin adını doğru yaz!
    }

    // Oyundan çıkış (ExitButton'a zaten direkt Application.Quit bağladık ama bu da yedekte durabilir)
    public void QuitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();

        // Editor'de çalışırken Quit() fonksiyonu işe yaramaz, durdurmak için:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void LoadMainMenu()
{
    SceneManager.LoadScene("MainMenuScene");
}
}