using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class BattleEntity : MonoBehaviour
{
    public BaseStatSO statData;
    public int currentHp;

    public void Initialize(BaseStatSO data)
    {
        statData = data;
        currentHp = data.maxHp;
        gameObject.SetActive(true);
        Debug.Log($"{data.jobName} 초기화 완료! HP: {currentHp}");
    }
    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if(currentHp <= 0)
        {
            currentHp = 0;
            OnDie();
        }
    }
    public void OnDie()
    {
        Debug.Log($"{statData.jobName}이(가) 쓰러졌습니다.");
    }
}
