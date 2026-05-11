using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArtifactSO", menuName = "Scriptable Objects/ArtifactSO")]
public class ArtifactSO : ScriptableObject
{
    [Header("기본 정보")]
    public string artifactName;
    public string Rariry;
    public Sprite icon;
    [TextArea] public string description;

    public int artifactId;
    public List<ArtifactEffectSO> artifactEffect;
}
