using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class TraySpawner : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField]
    private GameObject _trayPrefab;

    [SerializeField]
    private Transform _trayParent;
    [SerializeField]
    private Vector3 _firstPosition = new(-33.4f, 38.6f, 101.8f);
    
    [SerializeField]
    private float _yDistance = 10f;//Ʈ���̰� �Ÿ�
    
    [SerializeField]
    private int _poolSize = 10;

    private Queue<GameObject> pool = new();
    private Vector3 _nextPosition;

    private void Awake()
    {
        _nextPosition = _firstPosition;

        for(int i = 0; i < 4; i++)//ó�� 4���� ���� �� Ȱ��ȭ
        {
            GameObject tray = CreateTray();
            tray.transform.position = _nextPosition;
            tray.SetActive(true);

            _nextPosition += Vector3.up * _yDistance;

            pool.Enqueue(tray);
        }
    }

    private GameObject CreateTray()
    {
        var spawnNewTrayObject = Instantiate(_trayPrefab, _trayParent);
        spawnNewTrayObject.SetActive(false);

        spawnNewTrayObject.GetComponent<TrayState>().Init(this);
        return spawnNewTrayObject;
    }

    public GameObject SpawnTray()
    {
        //�����ϸ� ���� ����
        var spawntray = pool.Count > 0 ? pool.Dequeue() : CreateTray();

        spawntray.transform.position = _nextPosition;
        spawntray.SetActive(true);

        _nextPosition += Vector3.up * _yDistance;

        return spawntray;
    }

    public void RecycleTray(GameObject finishedTray)
    {
        finishedTray.SetActive(false);
        pool.Enqueue(finishedTray);
    }

}
