using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// Manages artifact discovery quests and triggers finale when all artifacts are decoded

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("UI Setup")]
    public Transform listContainer;
    public GameObject entryPrefab;
    public TextMeshProUGUI titleText;
    public SimpleNeonLogic frameLogic;

    [Header("Finale UI")]
    public GameObject finalePanel;
    public Color truthColor = Color.cyan;

    [Header("World Events")]
    public DoorController securityDoor;
    public AlarmSystem alarmSystem;

    private Dictionary<string, QuestEntry> questEntries = new Dictionary<string, QuestEntry>();
    private List<string> solvedArtifacts = new List<string>();
    private const int totalArtifacts = 8;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (titleText != null) titleText.color = Color.white;
        if (finalePanel != null) finalePanel.SetActive(false);
        
        GenerateList();
    }

    private void GenerateList()
    {
        foreach (Transform child in listContainer) Destroy(child.gameObject);
        questEntries.Clear();
        solvedArtifacts.Clear();

        string[] names = {
            "Old Doll",
            "Hourglass",
            "Mining Helmet",
            "Synth-Insulin Vial",
            "Hand of Tyrant",
            "Propaganda Book",
            "Copper Tree",
            "Access Card"
        };

        foreach (string name in names)
        {
            GameObject entryObject = Instantiate(entryPrefab, listContainer);
            QuestEntry entry = entryObject.GetComponent<QuestEntry>();
            
            if (entry != null)
            {
                entry.InitializeEntry(name);
                questEntries.Add(name.ToLower(), entry);
            }
        }
    }

    public void ArtifactSolved(string artifactName)
    {
        string key = artifactName.ToLower().Trim();

        if (questEntries.ContainsKey(key))
        {
            if (!solvedArtifacts.Contains(key))
            {
                solvedArtifacts.Add(key);
                questEntries[key].StartDecryption();
                AutoSaveProgress();
                CheckFullDecryption();
            }
        }
    }

    public bool IsArtifactSolved(string artifactName)
    {
        string key = artifactName.ToLower().Trim();
        return solvedArtifacts.Contains(key);
    }

    public bool IsQuestComplete()
    {
        return solvedArtifacts.Count >= totalArtifacts;
    }

    private void AutoSaveProgress()
    {
        PauseManager pauseManager = FindFirstObjectByType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.SaveGame();
        }
    }

    public List<string> GetSolvedArtifactsList() => solvedArtifacts;

    public void LoadProgress(List<string> savedArtifacts)
    {
        foreach (string artifact in savedArtifacts)
        {
            ArtifactSolved(artifact);
        }
    }

    private void CheckFullDecryption()
    {
        if (solvedArtifacts.Count >= totalArtifacts)
        {
            if (frameLogic != null) frameLogic.SwitchToTruth();
            TriggerFinaleUI();
        }
    }

    private void TriggerFinaleUI()
    {
        if (securityDoor != null) securityDoor.OpenDoor();
        if (alarmSystem != null) alarmSystem.StartAlarm();
        
        if (finalePanel != null)
        {
            finalePanel.SetActive(true);
            FinaleUIEffect effect = finalePanel.GetComponent<FinaleUIEffect>();
            if (effect != null) effect.TriggerEffect(truthColor);
        }
    }
}