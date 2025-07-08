using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GoodsType
{
    None,
    TwinBars,//�ֹֽ�
    Yoplait,//���÷�
    BottledDrink,//�� ����(�ݶ�)
    BottleJuice,//�� �ֽ�(�丶�� �ֽ�)
    Danji,//�����ֽ�
    Pepero//������(��Ű�� ũ��)

}

[System.Serializable]
public class OrderImage//�ֹ� ����(�ֹ��� ��ǰ Ÿ���̶� �̹���)
{
    //Ÿ�԰� �̹��� ������Ʈ�� �� ������ ������ Ŭ����
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
        //�ʱ�ȭ

        if(_goodsContentText != null)
        {
            _goodsContentText.text = _initialGoodsCount.ToString();
        }

        if(_orderImages == null || _orderImages.Count == 0)
        {
            Debug.Log("�ֹ� �̹��� �������");
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
