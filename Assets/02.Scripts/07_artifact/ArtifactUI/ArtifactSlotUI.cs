using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ArtifactSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    //[SerializeField] private TMP_Text nameText;

    private PlayerArtifactInstance _artifact;

    public void Set(PlayerArtifactInstance artifact)
    {
        _artifact = artifact;

        if (_artifact == null || _artifact.artifactSO == null)
            return;

        if (icon != null)
        {
            icon.sprite = _artifact.artifactSO.icon;
            icon.enabled = _artifact.artifactSO.icon != null;
        }
    }
}
