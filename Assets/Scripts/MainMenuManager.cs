using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

/// Manages main menu UI, game state transitions, and save file handling

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup creditsCanvasGroup;
    public GameObject gameplayHUD;
    public GameObject introManagerObject;

    [Header("References")]
    public PauseManager pauseManager;
    public PlayerMovement playerMovement;
    public CanvasGroup globalFader;

    [Header("Menu Buttons")]
    public Button continueButton;
    public Button newGameButton;

    [Header("Settings")]
    public float fadeSpeed = 4f;

    private Vector3 startPosition = new Vector3(-115.3f, 0f, -8.68f);
    private Vector3 startRotation = new Vector3(0f, 90f, 0f);

    void Start() => ResetToMenu();

    public void ResetToMenu()
    {
        Time.timeScale = 0f;

        AudioSource[] allAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audio in allAudio)
        {
            if (audio != null && audio.isPlaying)
            {
                audio.Stop();
            }
        }
        
        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.gameObject.SetActive(true);
            mainMenuCanvasGroup.alpha = 1f;
            mainMenuCanvasGroup.blocksRaycasts = true;
            mainMenuCanvasGroup.interactable = true;
        }

        if (creditsCanvasGroup != null)
        {
            creditsCanvasGroup.alpha = 0f;
            creditsCanvasGroup.blocksRaycasts = false;
            creditsCanvasGroup.interactable = false;
        }

        if (globalFader != null)
        {
            globalFader.alpha = 0f;
            globalFader.blocksRaycasts = false;
            globalFader.gameObject.SetActive(false);
        }

        if (pauseManager != null && pauseManager.pauseCanvasGroup != null)
        {
            pauseManager.pauseCanvasGroup.gameObject.SetActive(false);
            pauseManager.enabled = false;
        }

        if (gameplayHUD != null) gameplayHUD.SetActive(false);

        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(ForceCursorMode());

        if (playerMovement != null) playerMovement.canMove = false;

        UpdateContinueButton();
    }

    void UpdateContinueButton()
    {
        if (continueButton == null) return;

        bool gameCompleted = PlayerPrefs.GetInt("GameCompleted", 0) == 1;
        bool hasSaveFile = PlayerPrefs.HasKey("PlayerX");

        if (gameCompleted)
        {
            continueButton.interactable = false;
            
            var buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                buttonText.text = "CONTINUE [COMPLETED]";
            }
        }
        else if (hasSaveFile)
        {
            continueButton.interactable = true;

            var buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = Color.white;
                buttonText.text = "CONTINUE";
            }
        }
        else
        {
            continueButton.interactable = false;

            var buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                buttonText.text = "CONTINUE [NO SAVE]";
            }
        }
    }

    private IEnumerator ForceCursorMode()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        if (playerMovement != null)
        {
            PickupSystem pickupSystem = playerMovement.GetComponent<PickupSystem>();
            if (pickupSystem != null) pickupSystem.ResetPickupSystem();
        }

        Time.timeScale = 1f;

        if (playerMovement != null)
        {
            playerMovement.controller.enabled = false;
            playerMovement.transform.position = startPosition;
            playerMovement.transform.rotation = Quaternion.Euler(startRotation);
            Physics.SyncTransforms();
            playerMovement.controller.enabled = true;
        }

        StartCoroutine(TransitionToGame(true));
    }
    
    public void ContinueGame()
    {
        bool gameCompleted = PlayerPrefs.GetInt("GameCompleted", 0) == 1;
        if (gameCompleted) return;
        
        if (PlayerPrefs.HasKey("PlayerX"))
        {
            Time.timeScale = 1f;
            StartCoroutine(TransitionToGame(false));
        }
    }

    private IEnumerator TransitionToGame(bool showIntro)
    {
        if (globalFader != null)
        {
            globalFader.gameObject.SetActive(true);
            globalFader.blocksRaycasts = true;
            float t = 0;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * fadeSpeed;
                globalFader.alpha = t;
                yield return null;
            }
        }

        mainMenuCanvasGroup.gameObject.SetActive(false);

        if (showIntro && introManagerObject != null)
        {
            if (globalFader != null)
            {
                globalFader.alpha = 0f;
                globalFader.gameObject.SetActive(false);
            }
            introManagerObject.SetActive(true);
        }
        else if (pauseManager != null)
        {
            if (gameplayHUD != null) gameplayHUD.SetActive(true);
            pauseManager.LoadGame();
            pauseManager.enabled = true;

            float t = 1f;
            while (t > 0f)
            {
                t -= Time.unscaledDeltaTime * fadeSpeed;
                if (globalFader != null) globalFader.alpha = t;
                yield return null;
            }
            if (globalFader != null) globalFader.gameObject.SetActive(false);
        }
    }

    public void OpenCredits() => StartCoroutine(FadeBetweenPanels(mainMenuCanvasGroup, creditsCanvasGroup));
    public void CloseCredits() => StartCoroutine(FadeBetweenPanels(creditsCanvasGroup, mainMenuCanvasGroup));

    private IEnumerator FadeBetweenPanels(CanvasGroup from, CanvasGroup to)
    {
        from.blocksRaycasts = false;
        from.interactable = false;
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * fadeSpeed;
            from.alpha = Mathf.Lerp(1, 0, timer);
            to.alpha = Mathf.Lerp(0, 1, timer);
            yield return null;
        }
        from.alpha = 0;
        to.alpha = 1;
        to.blocksRaycasts = true;
        to.interactable = true;
    }

    public void ExitGame() => Application.Quit();
}