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
    Pepero//빼빼로(쿠키앤 크림)

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
    [SerializeField]
    private List<OrderImage> _orderImages;

    [SerializeField]
    private TextMeshProUGUI _goodsContentText;

    [SerializeField]
    private int _initialGoodsCount = 3;

    public GoodsType CurrentGoodsType { get; private set; } = GoodsType.None;

    public void Init()
    {
        //초기화

        if(_goodsContentText != null)
        {
            _goodsContentText.text = _initialGoodsCount.ToString();
        }

        if(_orderImages == null || _orderImages.Count == 0)
        {
            Debug.Log("주문 이미지 비어있음");
            return;
        }

        int goodsRandomIndex = Random.Range(0, _orderImages.Count);

        for(int i = 0; i<_orderImages.Count; i++)
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
    }

}
