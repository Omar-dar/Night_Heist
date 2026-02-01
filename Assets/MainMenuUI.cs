using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene(1); // Apartment_Blockout
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // works only in build
    }
}
