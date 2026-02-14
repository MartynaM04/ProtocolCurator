using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Manages game pause state, save/load functionality, and pause menu UI.
/// Handles player state transitions and alarm system pause/resume.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup pauseCanvasGroup;
    public PlayerMovement playerMovement;
    public PickupSystem pickupSystem;

    [Header("Save Feedback UI")]
    public CanvasGroup saveIconGroup;
    public float iconFlashSpeed = 2f;

    [Header("Settings")]
    public float fadeSpeed = 10f;
    
    public AudioSource[] ambientSounds;

    private bool isPaused = false;
    private bool isSaving = false;

    void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseCanvasGroup != null)
        {
            pauseCanvasGroup.alpha = 0f;
            pauseCanvasGroup.blocksRaycasts = false;
            pauseCanvasGroup.interactable = false;
            pauseCanvasGroup.gameObject.SetActive(false);
        }

        if (saveIconGroup != null)
        {
            saveIconGroup.alpha = 0f;
            saveIconGroup.gameObject.SetActive(false);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseCanvasGroup != null) 
            pauseCanvasGroup.gameObject.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (playerMovement != null) 
            playerMovement.canMove = false;

        // Pause alarm sounds
        AlarmSystem alarmSystem = FindFirstObjectByType<AlarmSystem>();
        if (alarmSystem != null)
        {
            alarmSystem.PauseAlarm();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (playerMovement != null)
        {
            playerMovement.canMove = (pickupSystem != null && pickupSystem.isInspecting) ? false : true;
        }

        // Resume alarm sounds
        AlarmSystem alarmSystem = FindFirstObjectByType<AlarmSystem>();
        if (alarmSystem != null)
        {
            alarmSystem.ResumeAlarm();
        }
    }

    void Update()
    {
        if (!enabled || pauseCanvasGroup == null) return;
        
        float targetAlpha = isPaused ? 1f : 0f;
        pauseCanvasGroup.alpha = Mathf.Lerp(
            pauseCanvasGroup.alpha, 
            targetAlpha, 
            Time.unscaledDeltaTime * fadeSpeed
        );
        
        if (!isPaused && pauseCanvasGroup.alpha < 0.01f)
        {
            pauseCanvasGroup.gameObject.SetActive(false);
        }
        
        pauseCanvasGroup.blocksRaycasts = isPaused;
        pauseCanvasGroup.interactable = isPaused;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        
        if (pickupSystem != null && pickupSystem.descriptionUI != null)
            pickupSystem.descriptionUI.SetActive(false);

        MainMenuManager menuManager = FindFirstObjectByType<MainMenuManager>();
        if (menuManager != null) 
            menuManager.ResetToMenu();

        this.enabled = false;
    }

    public void SaveGame()
    {
        if (playerMovement == null || QuestManager.Instance == null || isSaving)
            return;
        
        Vector3 pos = playerMovement.transform.position;
        float rotY = playerMovement.transform.eulerAngles.y;
        
        PlayerPrefs.SetFloat("PlayerX", pos.x);
        PlayerPrefs.SetFloat("PlayerY", pos.y);
        PlayerPrefs.SetFloat("PlayerZ", pos.z);
        PlayerPrefs.SetFloat("PlayerRotY", rotY);
        
        List<string> progress = QuestManager.Instance.GetSolvedArtifactsList();
        string progressString = string.Join(",", progress);
        PlayerPrefs.SetString("SavedProgress", progressString);

        AlarmSystem alarmSystem = FindFirstObjectByType<AlarmSystem>();
        if (alarmSystem != null)
        {
            PlayerPrefs.SetInt("AlarmActive", alarmSystem.IsAlarmActive() ? 1 : 0);
        }
        
        PlayerPrefs.Save();
        
        StopAllCoroutines();
        StartCoroutine(FlashSaveIcon());
    }

    private IEnumerator FlashSaveIcon()
    {
        isSaving = true;
        saveIconGroup.gameObject.SetActive(true);
        
        // Fade in
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * 3f;
            saveIconGroup.alpha = Mathf.Lerp(0, 1, timer);
            yield return null;
        }
        
        // Pulse effect
        for (int i = 0; i < 2; i++)
        {
            float pulse = 0;
            while (pulse < 1f)
            {
                pulse += Time.unscaledDeltaTime * iconFlashSpeed;
                saveIconGroup.alpha = 0.5f + Mathf.Abs(Mathf.Sin(Time.unscaledTime * 5f)) * 0.5f;
                yield return null;
            }
        }
        
        // Fade out
        timer = 0;
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * 2f;
            saveIconGroup.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
        
        saveIconGroup.alpha = 0;
        saveIconGroup.gameObject.SetActive(false);
        isSaving = false;
    }

    public void LoadGame()
    {
        if (playerMovement == null || QuestManager.Instance == null) return;
        
        // Load player position
        if (PlayerPrefs.HasKey("PlayerX"))
        {
            playerMovement.controller.enabled = false;
            playerMovement.transform.position = new Vector3(
                PlayerPrefs.GetFloat("PlayerX"),
                PlayerPrefs.GetFloat("PlayerY"),
                PlayerPrefs.GetFloat("PlayerZ")
            );
            playerMovement.transform.rotation = Quaternion.Euler(
                0, 
                PlayerPrefs.GetFloat("PlayerRotY"), 
                0
            );
            playerMovement.controller.enabled = true;
        }
        
        // Load quest progress
        string savedProgress = PlayerPrefs.GetString("SavedProgress", "");
        if (!string.IsNullOrEmpty(savedProgress))
        {
            List<string> loadedList = new List<string>(savedProgress.Split(','));
            QuestManager.Instance.LoadProgress(loadedList);
        }

        // Load alarm state
        int alarmWasActive = PlayerPrefs.GetInt("AlarmActive", 0);
        if (alarmWasActive == 1)
        {
            AlarmSystem alarmSystem = FindFirstObjectByType<AlarmSystem>();
            if (alarmSystem != null)
            {
                alarmSystem.StartAlarm();
            }
        }

        StartAmbientSounds();
        ResumeGame();
    }

    void StartAmbientSounds()
    {
        if (ambientSounds != null)
        {
            foreach (AudioSource source in ambientSounds)
            {
                if (source != null && !source.isPlaying)
                {
                    source.Play();
                }
            }
        }
    }

    public void OnPause(InputValue value)
    {
        if (!enabled) return;
        
        if (value.isPressed)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ExitGame()
    {
        SaveGame();
        Application.Quit();
    }
}