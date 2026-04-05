using UnityEngine;
using ArrowClash.Common;
using System.Collections.Generic;
using System;
public class InputBuffer
{
    public bool isInputting { get; private set; } = false;
    private List<Direction> _buffer = new List<Direction>();

    public void StartInput()
    {
       _buffer.Clear();
        isInputting = true;
    }
    public void CancelInput()
    {
        isInputting = false;
        _buffer.Clear();
    }
    public void Add(Direction dir)
    {
        if (!isInputting) return;
        _buffer.Add(dir);
    }

    public bool Matches(List<Direction> combo)
    {

        if (_buffer.Count < combo.Count) return false;
        int offset = _buffer.Count - combo.Count;
        for (int i = 0; i < combo.Count; i++)
        {
            if (_buffer[offset + i] != combo[i]) return false;
        }
        return true;
    }
    public void Clear() => _buffer.Clear();
    
    public string GetBufferString()
    {
        string result = "";
        foreach(Direction dir in _buffer)
            result += dir.ToString() + "";
        return result.Trim();
    }
    public List<Direction> GetBuffer() => _buffer; // 임시 디버그용
}
