using UnityEngine;
using ArrowClash.Common;
using System.Collections.Generic;
public class SkillExecutor
{
    private List<SkillSO> _skills;
    private InputBuffer _buffer;
    public bool isInputtung => _buffer.isInputting;

    public SkillExecutor(List<SkillSO> skills)
    {
        _buffer = new InputBuffer();
        _skills = skills;
    }

    public SkillSO OnSpaceBar()
    {
        if (!_buffer.isInputting)
        {
            _buffer.StartInput();
            Debug.Log("<color=cyan>[스킬 입력 시작] 커맨드를 입력하세요.</color>");
            return null;
        }
        else
        {
            Debug.Log($"<color=yellow>버퍼 내용: {_buffer.GetBufferString()} / 개수: {_buffer.GetBuffer().Count}</color>");
            SkillSO matched = CheckSkill();
            _buffer.CancelInput();

            if (matched != null)
                Debug.Log($"<color=lime>[스킬 발동] {matched.skillName}</color>");
            else
                Debug.Log("<color=orange>[스킬 없음] 매칭되는 스킬이 없습니다.</color>");

            return matched;
        }
    }
    public void OnDirectionInput(Direction dir)
    {
        _buffer.Add(dir);
        Debug.Log($"<color=cyan>[버퍼] {_buffer.GetBufferString()}</color>");
    }
    private SkillSO CheckSkill()
    {
        for(int i = 0; i < _skills.Count; i++)
        {
            if (_buffer.Matches(_skills[i].inputCombo))
                return _skills[i];
        }
        return null;
    }

    public void CancelInput()
    {
        _buffer.CancelInput();
        Debug.Log("<color=orange>[스킬 입력 취소]</color>");
    }

}
