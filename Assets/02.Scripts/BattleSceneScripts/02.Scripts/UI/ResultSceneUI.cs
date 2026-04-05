using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
public class ResultSceneUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI promptText;

    private bool _waitingInput = false;

    void Start()
    {
        resultText.text = BattleData.isVictory ? "Victory" : "Defaet";
        turnText.text = $"생존 턴 : {BattleData.turnCount}";
        promptText.text = "아무 버튼을 눌러 처음으로";
    }

    // Update is called once per frame
    void Update()
    {
        if (!_waitingInput)
        {
            _waitingInput = true;
            return;
        }

        if (Input.anyKeyDown)
            GameSceneManager.instance.LoadStart();
    }
}
