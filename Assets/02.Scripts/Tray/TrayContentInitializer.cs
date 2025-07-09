using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GoodsType
{
    None,
    TwinBars,//쌍쌍바
    Yoplait,//요플레
    BottledDrink,//병 음료(콜라)
    BottleJuice,//병 주스(토마토 주스)
    Danji,//포도주스
    Pepero//빼빼로(쿠키 앤 크림)

}

[System.Serializable]
public class OrderImage//주문 세팅(주문할 상품 타입이랑 이미지)
{
    //타입과 이미지 오브젝트를 한 쌍으로 관리할 클래스
    public GoodsType GoodsType;
    public GameObject GoodsImageObject;
}


public class TrayContentInitializer : MonoBehaviour
{
    [Header("드랍 위치 3곳")]
    [SerializeField]
    private List<Transform> _dropPositions;

    [Header("드랍할 프리팹들")]
    [SerializeField]
    private List<GameObject> _dropPrefabs;

    [SerializeField]
    private TrayState _trayState;

    [SerializeField]
    private List<OrderImage> _orderImages;

    public GoodsType CurrentGoodsType { get; private set; } = GoodsType.None;

    [Header("필요한 상품 갯수")]
    [SerializeField]
    private TextMeshProUGUI _goodsContentText;

    [SerializeField]
    private int _initialGoodsCount = 3;

    private int _currentGoodsCount;


    private void Start()
    {
        _currentGoodsCount = _initialGoodsCount;

        _trayState = GetComponent<TrayState>();
    }

    public void Init()
    {
        //초기화
        if(_goodsContentText != null)
        {
            _currentGoodsCount = 3;
            _goodsContentText.text = _initialGoodsCount.ToString();//초기 주문 개수(3개)
        }

        if(_orderImages == null || _orderImages.Count == 0)
        {
            Debug.Log("주문 이미지 비어있음");
            return;
        }

        int goodsRandomIndex = Random.Range(0, _orderImages.Count);//랜덤으로

        for(int i = 0; i<_orderImages.Count; i++)//선택된 것만 활성화 나머지 false
        {
            bool goodIsActive = (i == goodsRandomIndex);
            if (_orderImages[i].GoodsImageObject != null)
            {
                _orderImages[i].GoodsImageObject.SetActive(goodIsActive);
            }

            if(goodIsActive)
            {
                CurrentGoodsType = _orderImages[i].GoodsType;
            }
        }

      //  DropGoodsReset();
    }

    public void Init(GoodsType fixedType)
    {
       //첫 번째 트레이만 타입 고정하는 코드

        if (_goodsContentText != null)
        {
            _currentGoodsCount = 3;
            _goodsContentText.text = _initialGoodsCount.ToString();
        }

        if (_orderImages == null || _orderImages.Count == 0)
        {
            Debug.Log("주문 이미지 비어있음");
            return;
        }

        for (int i = 0; i < _orderImages.Count; i++)
        {
            bool isTarget = _orderImages[i].GoodsType == fixedType;//전달받은 것과 같은지
            if (_orderImages[i].GoodsImageObject != null)
            {
                _orderImages[i].GoodsImageObject.SetActive(isTarget);//병 음료(콜라)만 활성화
            }

            if (isTarget)//현재 상품 타입을 전달받은 것으로 저장
            {
                CurrentGoodsType = fixedType;
            }
        }

       // DropGoodsReset();
    }

    //주문 처리 관련
    public void DecreaseGoodsCount(int amount = 1)
    {
        if(_currentGoodsCount == 1)
        {
            AddGoods();

            _currentGoodsCount = 0;
            _goodsContentText.text = _currentGoodsCount.ToString();
            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                yield return new WaitForSeconds(1f);
                
                _trayState.SetState(TrayStateType.MoveLeft);//왼쪽으로 이동
            }

           
        }

        else
        {
            AddGoods();

            _currentGoodsCount -= amount;
            _goodsContentText.text = _currentGoodsCount.ToString();
        }

    }

    //드랍
    public void AddGoods()
    {
        Transform dropPos = GetRandomDropPosition();
        GameObject dropPrefab = GetDropPrefabByGoodsType(CurrentGoodsType);

        if (dropPrefab != null && dropPos != null)
        {
            GameObject droppedObj = Instantiate(dropPrefab, dropPos.position, Quaternion.identity);
            droppedObj.transform.SetParent(dropPos, true); // 자식으로 설정
        }
    }

    
    private Transform GetRandomDropPosition()
    {
        if (_dropPositions == null || _dropPositions.Count == 0)
            return null;
        int idx = Random.Range(0, _dropPositions.Count);
        return _dropPositions[idx];
    }

    private GameObject GetDropPrefabByGoodsType(GoodsType type)
    {
        foreach (var prefab in _dropPrefabs)
        {
            var dropGoods = prefab.GetComponent<DropGoods>();
            if (dropGoods != null && dropGoods.GoodsType == type)
            {
                return prefab;
            }
        }
        return null;
    }


    public void DropGoodsReset()
    {
        if (_dropPositions != null)
        {
            foreach (var dropPos in _dropPositions)
            {
                // dropPos(드랍 위치 오브젝트)의 자식 오브젝트들을 모두 삭제
                for (int i = dropPos.childCount - 1; i >= 0; i--)
                {
                    GameObject child = dropPos.GetChild(i).gameObject;
                    Destroy(child);
                }
            }
        }
    }
}
