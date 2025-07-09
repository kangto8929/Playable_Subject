using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        _gameOverPanel.SetActive(true);
    }
}
