using UnityEngine;
using TMPro;
using System.Collections;


/// Represents a single quest entry in the artifact list UI
/// Handles decryption animation when artifact truth is discovered

public class QuestEntry : MonoBehaviour
{
    public TextMeshProUGUI entryText;
    
    private string officialName;
    private bool isDecrypted = false;

    public void InitializeEntry(string name)
    {
        officialName = name;
        entryText.text = "??????????";
        entryText.color = Color.gray;
    }

    public void StartDecryption()
    {
        if (isDecrypted) return;
        isDecrypted = true;

        // If UI is inactive during loading, apply visuals immediately
        if (!gameObject.activeInHierarchy)
        {
            ApplyTruthVisuals();
            return;
        }

        StartCoroutine(DecryptRoutine());
    }

    private IEnumerator DecryptRoutine()
    {
        string scrambled = "!@#$%^&*()";
        float duration = 1.5f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            char[] current = scrambled.ToCharArray();
            for (int i = 0; i < current.Length; i++)
            {
                if (Random.value > 0.5f)
                {
                    current[i] = (char)Random.Range(33, 126);
                }
            }

            entryText.text = new string(current);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        ApplyTruthVisuals();
    }

    private void ApplyTruthVisuals()
    {
        entryText.text = officialName;
        entryText.color = Color.cyan;
    }
}