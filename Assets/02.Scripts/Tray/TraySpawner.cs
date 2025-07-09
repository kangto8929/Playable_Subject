using System.Collections.Generic;
using UnityEngine;

public class TraySpawner : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField]
    private GameObject _trayPrefab;

    [SerializeField]
    private Transform _trayParent;
    public Transform TrayParent => _trayParent;

    [SerializeField]
    private Vector3 _firstPosition = new(-33.4f, 38.6f, 101.8f);

    [SerializeField]
    private GoodsType _firstTrayGoodsType = GoodsType.BottledDrink; // �� ó�� Ʈ���� Ÿ�� ����

    [SerializeField]
    private float _yDistance = 10f; // Ʈ���� �� �Ÿ�

    [SerializeField]
    private int _poolSize = 10;

    private Queue<GameObject> pool = new();
    private Vector3 _nextPosition;

    private List<GameObject> activeTrays = new List<GameObject>();

    private TrayMovementManager _trayMovementManager;

    private void Awake()
    {
        _trayMovementManager = FindObjectOfType<TrayMovementManager>();

        // ó�� 4�� ���� (���� ����) - Ʃ�丮��
        for (int i = 0; i < 4; i++)
        {
            GameObject tray = CreateTray();

            if (i == 0)
                tray.GetComponent<TrayContentInitializer>().Init(_firstTrayGoodsType);
            else
                tray.GetComponent<TrayContentInitializer>().Init();

            tray.GetComponent<TrayState>().Init(this);
            tray.transform.position = _firstPosition + Vector3.up * (_yDistance * i);
            tray.SetActive(true);

            tray.GetComponent<TrayState>().SetState(TrayStateType.Stop);

            activeTrays.Add(tray);
        }

        for (int i = 0; i < _poolSize - 4; i++)
        {
            GameObject tray = CreateTray();
            pool.Enqueue(tray);
        }

        UpdateNextPosition();
    }

    private GameObject CreateTray()
    {
        var trayObj = Instantiate(_trayPrefab, _trayParent);
        trayObj.SetActive(false);
        trayObj.GetComponent<TrayContentInitializer>().Init();
        trayObj.GetComponent<TrayState>().Init(this);
        return trayObj;
    }

    private void UpdateNextPosition()
    {
        float highestY = float.MinValue;

        foreach (var tray in activeTrays)
        {
            if (tray.activeSelf)
            {
                float y = tray.transform.position.y;
                if (y > highestY)
                    highestY = y;
            }
        }

        if (highestY == float.MinValue)
        {
            // Ȱ�� Ʈ���� ������ �⺻ ��ġ�� �ʱ�ȭ
            _nextPosition = _firstPosition;
        }
        else
        {
            _nextPosition = new Vector3(_firstPosition.x, highestY + _yDistance, _firstPosition.z);
        }
    }

    public GameObject SpawnTray()
    {
        GameObject spawnTray = pool.Count > 0 ? pool.Dequeue() : CreateTray();

        spawnTray.GetComponent<TrayContentInitializer>().Init();
        spawnTray.GetComponent<TrayState>().Init(this);

        UpdateNextPosition();

        spawnTray.transform.position = _nextPosition;
        spawnTray.SetActive(true);
        activeTrays.Add(spawnTray);

        // �̵� �ӵ� �� ���� ����
        spawnTray.GetComponent<TrayState>().SetMoveSpeed(_trayMovementManager.MoveDownSpeed);
        spawnTray.GetComponent<TrayState>().SetState(TrayStateType.MoveDown);

        return spawnTray;
    }

    public void RecycleTray(GameObject tray)
    {
        tray.SetActive(false);
        activeTrays.Remove(tray);
        pool.Enqueue(tray);

        UpdateNextPosition();
    }
}

