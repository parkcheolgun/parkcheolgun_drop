using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 기획서 4장, 9.7: 보드 위 모든 홀을 추적해 클리어를 판정하고, 리셋을 제공합니다.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("클리어 UI (선택)")]
    [SerializeField] private GameObject clearMessage;

    private readonly List<HoleController> activeHoles = new();

    void Awake()
    {
        Instance = this;

        if (clearMessage != null)
        {
            clearMessage.SetActive(false);
        }
    }

    void Update()
    {
        // 실패 조건이 없는 프로토타입이므로, R키로 언제든 스테이지를 처음 배치로 되돌립니다(기획서 5장).
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetStage();
        }
    }

    public void RegisterHole(HoleController hole)
    {
        if (!activeHoles.Contains(hole))
        {
            activeHoles.Add(hole);
        }
    }

    public void NotifyHoleCleared(HoleController hole)
    {
        activeHoles.Remove(hole);

        if (activeHoles.Count == 0)
        {
            OnStageClear();
        }
    }

    public void ResetStage()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private void OnStageClear()
    {
        Debug.Log("Stage Clear!");

        if (clearMessage != null)
        {
            clearMessage.SetActive(true);
        }
    }
}
