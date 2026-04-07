using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class BattleEntity : MonoBehaviour
{
    public BaseStatSO statData;
    public int currentHp { get; private set; }
    public StatusHandler statusHandler { get; private set; }

    private void Awake()
    {
        statusHandler = new StatusHandler(this);
    }

    public void Initialize(BaseStatSO data)
    {
        statData = data;
        currentHp = data.maxHp;
        Debug.Log($"{data.jobName} initialized. HP: {currentHp}");
    }

    public void InitializeFromInstance(PlayerInstance pi)
    {
        statData = BattleDataManager.instance.playerStatSO;
        currentHp = pi.currentHp;
        Debug.Log($"Player initialized. HP: {currentHp}");
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        currentHp = Mathf.Max(currentHp, 0);
        Debug.Log($"{statData.jobName} HP: {currentHp}");
        if (currentHp <= 0)
        {
            OnDie();
        }
    }

    public void OnDie()
    {
        Debug.Log($"{statData.jobName} died.");
        TurnManager.instance.OnEntityDied(this);
    }
}
