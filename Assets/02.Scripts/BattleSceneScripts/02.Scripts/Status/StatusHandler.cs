using UnityEngine;
using System.Collections.Generic;
using ArrowClash.Common;
public class StatusHandler
{
    private BattleEntity _owner;
    private List<StatusEffectInstance> _activeEffects = new List<StatusEffectInstance>();

    public StatusHandler(BattleEntity owner)
    {
        _owner = owner;
    }
    public void Apply(StatusEffectSO so, BattleEntity attacker, int stackCount = 1, int overrideDuration = -1)
    {
        int duration = (overrideDuration > 0) ? overrideDuration : so.duration;
        StatusEffectInstance existing = _activeEffects.Find(e => e.data == so);

        if (existing == null) {
            StatusEffectInstance instance = new StatusEffectInstance(so);
            instance.remainingDuration = duration;
            so.OnApply(_owner, attacker, instance);

            if (so.stackRule == StackRule.Stack && so.maxStack > 1)
            {
                for (int i = 1; i < stackCount; i++)
                {
                    if (instance.currentStack >= so.maxStack) break;
                    so.OnStack(instance);
                }
            }

            _activeEffects.Add(instance);
            Debug.Log($"<color=yellow>[상태이상] {so.effectName} 적용 / 스택: {instance.currentStack}</color>");
            return;
        }
        switch (so.durationRule)
        {
            case DurationRule.Ignore:
                Debug.Log($"<color=grey>[상태이상] {so.effectName} 이미 적용 중 — 무시</color>");
                return;
            case DurationRule.Refresh: 
                existing.remainingDuration = duration;
                Debug.Log($"<color=yellow>[상태이상] {so.effectName} 갱신 / 남은 턴: {existing.remainingDuration}</color>");
                break;
            case DurationRule.Stack:
                existing.remainingDuration += duration;
                Debug.Log($"<color=yellow>[상태이상] {so.effectName} 스택 {existing.currentStack} / 남은 턴: {existing.remainingDuration}</color>");
                break;
        }
        if (so.stackRule == StackRule.Stack)
        {
            for (int i = 0; i < stackCount; i++)
            {
                if (existing.currentStack >= so.maxStack) break;
                so.OnStack(existing);
            }
        }
    }
    public void Tick()
    {
        List<StatusEffectInstance> expired = new List<StatusEffectInstance>();
        foreach(var effect in _activeEffects)
        {
            effect.data.OnTurnEnd(_owner, effect);
            effect.remainingDuration--;

            if (effect.remainingDuration <= 0)
            {
                expired.Add(effect);
            }
        }
        foreach (var e in expired)
        {
            Debug.Log($"<color=grey>[상태이상] {e.data.effectName} 해제</color>");
            _activeEffects.Remove(e);
        }
    }
    public void OnDirectionInput()
    {
        foreach (var effect in _activeEffects)
            effect.data.OnDirectionInput(_owner, effect);
    }
    public bool IsActionDisabled() =>
        _activeEffects.Exists(e => e.data.isActionDisable);
    public bool IsSkillDisabled() => _activeEffects.Exists(e => e.data.disablesSkill);

    public Direction FilterDirection(Direction dir)
    {
        foreach (var effect in _activeEffects)
        {
            if (effect.data is IDirectionFilter filter)
                dir = filter.FilterDirection(dir, effect);
        }
        return dir;
    }

    public string GetActiveEffectString()
    {
        string result = "";
        foreach (var e in _activeEffects)
            result += $"{e.data.effectName}({e.remainingDuration}턴) ";
        return result.Trim();
    }
}
