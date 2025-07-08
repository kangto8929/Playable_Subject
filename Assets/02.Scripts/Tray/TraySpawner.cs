using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;

public class TraySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField]
    private GameObject _trayPrefab;
    [SerializeField]
    private Transform _trayParent;
    [SerializeField]
    private Vector3 _firstPosition = new(-33.4f, 38.6f, 101.8f);

    [SerializeField]
    private float _yDistance = 10f;//트레이간 거리

    [SerializeField]
    private int _poolSize = 10;
    private Queue<GameObject> pool = new();
    private Vector3 _nextPosition;


    private TrayMovementManager _trayMovementManager;

    private void Awake()
    {
        _nextPosition = _firstPosition;
        _trayMovementManager = FindObjectOfType<TrayMovementManager>();

        for (int i = 0; i < 4; i++)//처음 4개는 생성 후 활성화
        {
            GameObject tray = CreateTray();
            tray.transform.position = _nextPosition;
            tray.SetActive(true);
            _nextPosition += Vector3.up * _yDistance;
        }

        for (int i = 0; i < _poolSize - 4; i++)
        {
            GameObject tray = CreateTray();
            pool.Enqueue(tray);
        }
    }

    private GameObject CreateTray()
    {
        var spawnNewTrayObject = Instantiate(_trayPrefab, _trayParent);
        spawnNewTrayObject.SetActive(false);

        spawnNewTrayObject.GetComponent<TrayContentInitializer>().Init();
        spawnNewTrayObject.GetComponent<TrayState>().Init(this);

        return spawnNewTrayObject;
    }

    public GameObject SpawnTray()
    {
        //부족하면 새로 생성
        var spawntray = pool.Count > 0 ? pool.Dequeue() : CreateTray();
        spawntray.transform.position = _nextPosition;

        spawntray.GetComponent<TrayContentInitializer>().Init();
        spawntray.GetComponent<TrayState>().Init(this);

        spawntray.SetActive(true);
        //var trayState = spawntray.GetComponent<TrayState>();


        _nextPosition += Vector3.up * _yDistance;
        return spawntray;
    }

    public void RecycleTray(GameObject finishedTray)
    {
        finishedTray.SetActive(false);
        pool.Enqueue(finishedTray);
    }
}
