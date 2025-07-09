using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("설정")]
    [SerializeField]
    private int _width = 8;
    [SerializeField]
    private int _initialHeight = 12;
    [SerializeField]
    private float _cellSize = 1.5f;

    [Header("프리팹")]
    [SerializeField]
    private GameObject _gridCellPrefab;
    [SerializeField]
    private GameObject[] _blockPrefabs; //상품 프리팹

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
        // 클릭 처리
        if (Input.GetMouseButtonDown(0))
        {


            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭된 오브젝트가 GridCell인지 확인
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

    //그리드 안에 들어갈 상품 무작위
    GameObject GetRandomBlock()
    {

        int index = Random.Range(0, _blockPrefabs.Length);
        return _blockPrefabs[index];
    }

    // 블록 제거 및 이동
    public void OnBlockRemoved(int x, int y)
    {
        Debug.Log($"GridManager: 블록 제거됨! 위치: [{x},{y}]");

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

                    // 이동 대상 위치 계산
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

                    // 0.5초 후에 이동 시작
                    blockToMove.transform
                        .DOLocalMove(targetLocalPos, 0.25f)
                        .SetDelay(0.25f)
                        .SetEase(Ease.OutCubic);
                }
            }
        }

        // 가장 아래에 새 블록 생성
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

            // 0.5초 후에 새 블록이 올라옴
            newBlock.transform
                .DOLocalMove(targetLocalPos, 0.5f)
                .SetDelay(0.25f)
                .SetEase(Ease.OutBack);

            _cells[x, 0].CurrentBlock = newBlock;
        }
    }


}