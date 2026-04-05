using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;

    [Header("씬 전환 대기 시간(초)")]
    public float transitionDelay = 0f;

    private void Awake()
    {
        // → 씬 전환 후 새 GameSceneManager가 생성되면 제거
        // → 첫 번째 인스턴스만 유지
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);  // 씬 전환 후에도 유지
    }

    private void OnEnable()
    {
        TurnManager.OnVictory += HandleVictory;
        TurnManager.OnDefeat += HandleDefeat;
    }

    private void OnDisable()
    {
        TurnManager.OnVictory -= HandleVictory;
        TurnManager.OnDefeat -= HandleDefeat;
    }

    private void HandleVictory(int turnCount) => StartCoroutine(WaitAndLoad(SceneName.Map));
    private void HandleDefeat() => StartCoroutine(WaitAndLoad(SceneName.Result));

    private IEnumerator WaitAndLoad(string sceneName)
    {
        // 최소 0.5초 대기 후 입력 받음
        yield return new WaitForSecondsRealtime(0.5f);

        while (!Input.anyKeyDown)
            yield return new WaitForSecondsRealtime(0.05f);

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadImmediate(string sceneName)
    {
        yield return new WaitForSecondsRealtime(transitionDelay);
        SceneManager.LoadScene(sceneName);
    }

    // 외부에서 직접 씬 전환이 필요할 때 (버튼 등)

    public void LoadMap() => StartCoroutine(WaitAndLoad(SceneName.Map));
    public void LoadStart() => StartCoroutine(WaitAndLoad(SceneName.Start));
    public void LoadBattle() => StartCoroutine(LoadImmediate(SceneName.Battle));  // 즉시 이동
    public void LoadResult() => StartCoroutine(LoadImmediate(SceneName.Result));  // 즉시 이동

}
