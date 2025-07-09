using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Luna.Unity;

public class UI_GameOver : MonoBehaviour
{
    public static UI_GameOver Instance;

    [SerializeField]
    private GameObject _gameOverPanel;

    private void Start()
    {
        Instance = this;
    }

    public void ShowGameOverPanel()
    {
        Luna.Unity.Playable.InstallFullGame();

        Luna.Unity.LifeCycle.GameEnded();

        Analytics.LogEvent(Analytics.EventType.LevelFailed);

        _gameOverPanel.SetActive(true);
    }

}
