using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


public class ArtifactUI  : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject rootPanel;

    [Header("List")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private ArtifactSlotUI ArifactSlotPrefab;

    [Header("Buttons")]
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button closeButton;

    private PlayerInstance _player;
    private PlayerArtifactInstance _artifactInstance;

    private void Awake()
    {
        if(rootPanel != null)
            rootPanel.SetActive(false);

        //if (deleteButton != null)
            //deleteButton.onClick.AddListener(DeleteArtifact);
    }

    public void Show(PlayerInstance player)
    {
        _player = player;

        if (_player == null)
            return;
        if (rootPanel != null)
            rootPanel.SetActive(true);

        //RefreshAll();
    }

    public void Hide()
    {
        if (rootPanel != null)
            rootPanel.SetActive(false);
    }

    public void OpenArtifactCollection()
    {
        if (BattleDataManager.instance == null)
            return;

        PlayerInstance player = BattleDataManager.instance.PlayerInstance;

        if (player == null)
            return;
        Show(player);
    }

    public void CloseArtifactCollection() {
        Hide();
    }

    private void RefreshArtifactList()
    {
        //ClearArtifactList();

        if (_player == null)
            return;

        List<PlayerArtifactInstance> artifact = _player.playerArtifactCollection.obtainedArtifact;

        for(int i = 0; i < artifact.Count; i++)
        {
            
        }
    }
}
