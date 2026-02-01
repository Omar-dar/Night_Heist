using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Collecting")]
    [SerializeField] private int totalItemsToWin = 6;
    [SerializeField] private TMP_Text counterText;

    [Header("UI Popups")]
    [SerializeField] private GameObject victoryPopup;   // VictoryPopup image object
    [SerializeField] private GameObject gameOverPopup;  // GameOverPopup image object

    [Header("Popup Delays")]
    [SerializeField] private float gameOverPopupDelay = 1.5f;
    [SerializeField] private float victoryPopupDelay = 1.5f;

    [Header("Return To Main Menu")]
    [SerializeField] private int mainMenuBuildIndex = 0;   // MainMenu scene index in Build Profiles
    [SerializeField] private float returnDelay = 3f;       // seconds before going back

    private int collectedCount = 0;
    private bool ended = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        ended = false;

        if (victoryPopup != null) victoryPopup.SetActive(false);
        if (gameOverPopup != null) gameOverPopup.SetActive(false);

        UpdateCounterUI();
    }

    // Call this when an item is collected
    public void CollectOne()
    {
        if (ended) return;

        collectedCount++;
        UpdateCounterUI();

        if (collectedCount >= totalItemsToWin)
            Victory();
    }

    private void UpdateCounterUI()
    {
        if (counterText != null)
            counterText.text = $"{collectedCount}/{totalItemsToWin}";
    }

    public void GameOver()
    {
        if (ended) return;
        ended = true;

        Time.timeScale = 0f;

        StartCoroutine(GameOverSequence());
    }

    private void Victory()
    {
        if (ended) return;
        ended = true;

        Time.timeScale = 0f;

        StartCoroutine(VictorySequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSecondsRealtime(gameOverPopupDelay);
        Time.timeScale = 0f;


        if (gameOverPopup != null)
            gameOverPopup.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(ReturnToMenuAfterDelay());
    }

    private IEnumerator VictorySequence()
    {
        yield return new WaitForSecondsRealtime(victoryPopupDelay);

        if (victoryPopup != null)
            victoryPopup.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(ReturnToMenuAfterDelay());
    }


    private IEnumerator ReturnToMenuAfterDelay()
    {
        // works even when timeScale = 0
        yield return new WaitForSecondsRealtime(returnDelay);

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuBuildIndex);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
