using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] private GameObject skillTreeScrollView;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private SkillLevelLineUI linePrefab;
    [SerializeField] private SkillUI skillPrefab;

    private PlayerInstance _player;
    private JobSkillTreeSO _skillTree;

    private void Awake()
    {
        skillTreeScrollView?.SetActive(false);
    }

    public void Show(PlayerInstance player)
    {
        _player = player;

        if (_player == null || _player.baseStat == null)
            return;

        _skillTree = _player.baseStat.skillTree;

        if (_skillTree == null)
            return;

        skillTreeScrollView.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        skillTreeScrollView.SetActive(false);
    }

    public void OpenSkillTree()
    {
        if (BattleDataManager.instance == null)
            return;

        PlayerInstance player = BattleDataManager.instance.PlayerInstance;

        if (player == null)
            return;

        Show(player);
    }

    public void CloseSkillTree()
    {
        Hide();
    }


    private void Refresh()
    {
        Clear();

        List<SkillUnlockData> unlocks = _skillTree.GetAllUnlocks();
        List<int> levels = GetSortedUnlockLevels(unlocks);

        for (int i = 0; i < levels.Count; i++)
        {
            int level = levels[i];

            SkillLevelLineUI line = Instantiate(linePrefab, contentRoot);
            line.SetLevel(level);

            for (int j = 0; j < unlocks.Count; j++)
            {
                SkillUnlockData unlockData = unlocks[j];

                if (unlockData.unlockLevel != level)
                    continue;

                PlayerSkillInstance instance = _player.GetSkillInstance(unlockData.skill);

                SkillUI skillUI = Instantiate(skillPrefab, line.SkillRoot);
                skillUI.Set(unlockData, instance, _player.Level, OnSkillClicked);
            }
        }
    }

    private void OnSkillClicked(SkillUnlockData unlockData)
    {
        if (_player.Level < unlockData.unlockLevel)
            return;

        if (!_player.HasSkill(unlockData.skill))
            _player.UnlockSkill(unlockData.skill);
        else
            _player.UpgradeSkill(unlockData.skill);

        Refresh();
    }

    private List<int> GetSortedUnlockLevels(List<SkillUnlockData> unlocks)
    {
        List<int> levels = new List<int>();

        for (int i = 0; i < unlocks.Count; i++)
        {
            int level = unlocks[i].unlockLevel;

            if (!levels.Contains(level))
                levels.Add(level);
        }

        levels.Sort();
        return levels;
    }

    private void Clear()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);
    }
}
