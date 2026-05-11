#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class RewardCsvImporter
{
    private const string RewardCsvPath = "Assets/10.DataSheets/Rewards.csv";
    private const string RewardEffectCsvPath = "Assets/10.DataSheets/RewardEffects.csv";

    private const string RewardOutputFolder = "Assets/07.SOAssets/Reward";
    private const string RewardEffectOutputFolder = "Assets/07.SOAssets/RewardEffect";
    private const string RewardPoolPath = "Assets/07.SOAssets/RewardPoolSO.asset";

    [MenuItem("Tools/Import/Reward CSV")]
    public static void Import()
    {
        EnsureFolder(RewardOutputFolder);
        EnsureFolder(RewardEffectOutputFolder);

        Dictionary<string, StatModifierRewardEffectSO> effectMap = ImportRewardEffects();
        ImportRewards(effectMap);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[RewardCsvImporter] Reward CSV Import 완료");
    }

    private static Dictionary<string, StatModifierRewardEffectSO> ImportRewardEffects()
    {
        Dictionary<string, List<StatModifier>> groupedModifiers = new Dictionary<string, List<StatModifier>>();
        Dictionary<string, string> effectAssetNames = new Dictionary<string, string>();

        List<Dictionary<string, string>> rows = ReadCsv(RewardEffectCsvPath);

        foreach (var row in rows)
        {
            string id = Get(row, "id");
            if (string.IsNullOrWhiteSpace(id))
                continue;

            string assetName = Get(row, "assetName");
            string statTypeText = Get(row, "statType");
            string modeText = Get(row, "mode");
            string valueText = Get(row, "value");
            string durationText = Get(row, "duration");

            if (!Enum.TryParse(statTypeText, out StatType statType))
            {
                Debug.LogError($"[RewardCsvImporter] 잘못된 statType: {statTypeText} / id: {id}");
                continue;
            }

            if (!Enum.TryParse(modeText, out ModifierMode mode))
            {
                Debug.LogError($"[RewardCsvImporter] 잘못된 mode: {modeText} / id: {id}");
                continue;
            }

            if (!float.TryParse(valueText, out float value))
            {
                Debug.LogError($"[RewardCsvImporter] 잘못된 value: {valueText} / id: {id}");
                continue;
            }

            if (!int.TryParse(durationText, out int duration))
                duration = -1;

            if (!groupedModifiers.ContainsKey(id))
                groupedModifiers[id] = new List<StatModifier>();

            groupedModifiers[id].Add(new StatModifier(
                statType,
                mode,
                value,
                duration,
                id
            ));

            if (!string.IsNullOrWhiteSpace(assetName))
                effectAssetNames[id] = assetName;
        }

        Dictionary<string, StatModifierRewardEffectSO> result = new Dictionary<string, StatModifierRewardEffectSO>();

        foreach (var pair in groupedModifiers)
        {
            string id = pair.Key;
            string assetName = effectAssetNames.ContainsKey(id) ? effectAssetNames[id] : id;
            string assetPath = $"{RewardEffectOutputFolder}/{assetName}.asset";

            StatModifierRewardEffectSO effect = AssetDatabase.LoadAssetAtPath<StatModifierRewardEffectSO>(assetPath);

            if (effect == null)
            {
                effect = ScriptableObject.CreateInstance<StatModifierRewardEffectSO>();
                AssetDatabase.CreateAsset(effect, assetPath);
            }

            effect.modifiers = pair.Value;
            EditorUtility.SetDirty(effect);

            result[id] = effect;
        }

        return result;
    }

    private static void ImportRewards(Dictionary<string, StatModifierRewardEffectSO> effectMap)
    {
        List<Dictionary<string, string>> rows = ReadCsv(RewardCsvPath);

        RewardPoolSO rewardPool = AssetDatabase.LoadAssetAtPath<RewardPoolSO>(RewardPoolPath);
        if (rewardPool == null)
        {
            Debug.LogError($"[RewardCsvImporter] RewardPoolSO를 찾을 수 없습니다: {RewardPoolPath}");
            return;
        }

        rewardPool.rewards.Clear();

        foreach (var row in rows)
        {
            string id = Get(row, "id");
            if (string.IsNullOrWhiteSpace(id))
                continue;

            string assetName = Get(row, "assetName");
            if (string.IsNullOrWhiteSpace(assetName))
                assetName = id;

            string assetPath = $"{RewardOutputFolder}/{assetName}.asset";

            RewardSO reward = AssetDatabase.LoadAssetAtPath<RewardSO>(assetPath);

            if (reward == null)
            {
                reward = ScriptableObject.CreateInstance<RewardSO>();
                AssetDatabase.CreateAsset(reward, assetPath);
            }

            reward.rewardName = Get(row, "rewardName");
            reward.description = Get(row, "description");

            string rarityText = Get(row, "rarity");
            if (Enum.TryParse(rarityText, out RewardRarity rarity))
                reward.rarity = rarity;
            else
                Debug.LogWarning($"[RewardCsvImporter] 잘못된 rarity: {rarityText} / id: {id}");

            reward.effects = new List<RewardEffectSO>();

            string effectIds = Get(row, "effectIds");
            foreach (string effectId in SplitIds(effectIds))
            {
                if (effectMap.TryGetValue(effectId, out StatModifierRewardEffectSO effect))
                {
                    reward.effects.Add(effect);
                }
                else
                {
                    Debug.LogError($"[RewardCsvImporter] 연결할 RewardEffect를 찾지 못했습니다. rewardId: {id}, effectId: {effectId}");
                }
            }

            EditorUtility.SetDirty(reward);

            bool inPool = IsTrue(Get(row, "inPool"));
            if (inPool)
                rewardPool.rewards.Add(reward);
        }

        EditorUtility.SetDirty(rewardPool);
    }

    private static List<Dictionary<string, string>> ReadCsv(string path)
    {
        List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

        if (!File.Exists(path))
        {
            Debug.LogError($"[RewardCsvImporter] CSV 파일을 찾을 수 없습니다: {path}");
            return result;
        }

        string[] lines = File.ReadAllLines(path);

        if (lines.Length <= 1)
            return result;

        string[] headers = ParseCsvLine(lines[0]).ToArray();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            List<string> values = ParseCsvLine(lines[i]);
            Dictionary<string, string> row = new Dictionary<string, string>();

            for (int h = 0; h < headers.Length; h++)
            {
                string value = h < values.Count ? values[h] : "";
                row[headers[h]] = value;
            }

            result.Add(row);
        }

        return result;
    }

    private static List<string> ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current += '"';
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);
        return result;
    }

    private static string Get(Dictionary<string, string> row, string key)
    {
        return row.TryGetValue(key, out string value) ? value.Trim() : "";
    }

    private static IEnumerable<string> SplitIds(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            yield break;

        string[] parts = raw.Split('|');

        foreach (string part in parts)
        {
            string id = part.Trim();
            if (!string.IsNullOrWhiteSpace(id))
                yield return id;
        }
    }

    private static bool IsTrue(string value)
    {
        return value.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
            || value.Equals("true", StringComparison.OrdinalIgnoreCase)
            || value == "1";
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] parts = folderPath.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";

            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);

            current = next;
        }
    }
}
#endif
