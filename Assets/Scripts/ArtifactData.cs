using UnityEngine;

/// ScriptableObject containing artifact propaganda and hidden truth data

[CreateAssetMenu(fileName = "NewArtifactData", menuName = "Museum/Artifact Data")]
public class ArtifactData : ScriptableObject
{
    [Header("UI Display Settings")]
    public string statusText = "STATUS: CLASSIFIED";

    [Header("Government Propaganda")]
    public string officialName;
    [TextArea(3, 10)]
    public string propagandaDescription;

    [Header("The Hidden Truth")]
    public string trueName;
    [TextArea(3, 10)]
    public string hiddenTruthDescription;
    
    [Header("Reveal Settings")]
    public Vector3 revealRotation;
}

