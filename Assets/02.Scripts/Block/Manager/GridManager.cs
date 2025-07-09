using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("����")]
    [SerializeField]
    private int _width = 8;
    [SerializeField]
    private int _initialHeight = 12;
    [SerializeField]
    private float _cellSize = 1.5f;

    [Header("������")]
    [SerializeField]
    private GameObject _gridCellPrefab;
    [SerializeField]
    private GameObject[] _blockPrefabs; //��ǰ ������

    private GridCell[,] _cells;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BoxCollider box = _gridCellPrefab.GetComponent<BoxCollider>();
        if (box != null)
        {
            _cellSize = Mathf.Max(box.size.x, box.size.y, box.size.z);
        }

        _cells = new GridCell[_width, 1000];

        for (int y = 0; y < _initialHeight; y++)
        {
            GenerateRow(y);
        }

    }


    void Update()
    {
        // Ŭ�� ó��
        if (Input.GetMouseButtonDown(0))
        {


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Ŭ���� ������Ʈ�� GridCell���� Ȯ��
                GridCell cell = hit.collider.GetComponent<GridCell>();
                if (cell != null)
                {
                    cell.OnClicked();
                }
            }
        }
    }

    void GenerateRow(int y)
    {


        for (int x = 0; x < _width; x++)
        {
            Vector3 pos = transform.position + new Vector3(x * _cellSize, y * _cellSize, 0);
            GameObject cellGO = Instantiate(_gridCellPrefab, pos, Quaternion.identity, transform);
            GridCell cell = cellGO.GetComponent<GridCell>();
            cell.SetCoordinates(x, y);
            _cells[x, y] = cell;

            GameObject randomBlockPrefab = GetRandomBlock();
            if (randomBlockPrefab != null)
            {
                PlaceBlockInCell(randomBlockPrefab, cell);
            }
        }
    }

    public void PlaceBlockInCell(GameObject prefab, GridCell cell)
    {

        GameObject block = Instantiate(prefab);
        block.transform.SetParent(cell.transform);

        BlockData heightData = block.GetComponent<BlockData>();
        if (heightData != null)
        {
            block.transform.localPosition = new Vector3(0, heightData.YOffset, 0);
        }

        cell.CurrentBlock = block; 
    }

    //�׸��� �ȿ� �� ��ǰ ������
    GameObject GetRandomBlock()
    {

        int index = Random.Range(0, _blockPrefabs.Length);
        return _blockPrefabs[index];
    }

    // ��� ���� �� �̵�
    public void OnBlockRemoved(int x, int y)
    {
        Debug.Log($"GridManager: ��� ���ŵ�! ��ġ: [{x},{y}]");

        for (int i = y - 1; i >= 0; i--)
        {
            if (_cells[x, i] != null && _cells[x, i].CurrentBlock != null)
            {
                GameObject blockToMove = _cells[x, i].CurrentBlock;
                _cells[x, i].CurrentBlock = null;

                if (_cells[x, i + 1] != null)
                {
                    _cells[x, i + 1].CurrentBlock = blockToMove;
                    blockToMove.transform.SetParent(_cells[x, i + 1].transform);

                    // �̵� ��� ��ġ ���
                    Vector3 targetLocalPos;
                    BlockData heightData = blockToMove.GetComponent<BlockData>();
                    if (heightData != null)
                    {
                        targetLocalPos = new Vector3(0, heightData.YOffset, 0);
                    }
                    else
                    {
                        targetLocalPos = Vector3.zero;
                    }

                    // 0.5�� �Ŀ� �̵� ����
                    blockToMove.transform
                        .DOLocalMove(targetLocalPos, 0.25f)
                        .SetDelay(0.25f)
                        .SetEase(Ease.OutCubic);
                }
            }
        }

        // ���� �Ʒ��� �� ��� ����
        if (_cells[x, 0] != null && _cells[x, 0].CurrentBlock == null)
        {
            GameObject prefab = GetRandomBlock();
            GameObject newBlock = Instantiate(prefab);
            newBlock.transform.SetParent(_cells[x, 0].transform);

            Vector3 startLocalPos = new Vector3(0, -_cellSize, 0);
            Vector3 targetLocalPos;

            BlockData heightData = newBlock.GetComponent<BlockData>();
            if (heightData != null)
            {
                targetLocalPos = new Vector3(0, heightData.YOffset, 0);
                startLocalPos.y = heightData.YOffset - _cellSize;
            }
            else
            {
                targetLocalPos = Vector3.zero;
            }

            newBlock.transform.localPosition = startLocalPos;

            // 0.5�� �Ŀ� �� ����� �ö��
            newBlock.transform
                .DOLocalMove(targetLocalPos, 0.5f)
                .SetDelay(0.25f)
                .SetEase(Ease.OutBack);

            _cells[x, 0].CurrentBlock = newBlock;
        }
    }


}