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
    Pepero//������(��Ű �� ũ��)

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
    [Header("��� ��ġ 3��")]
    [SerializeField]
    private List<Transform> _dropPositions;

    [Header("����� �����յ�")]
    [SerializeField]
    private List<GameObject> _dropPrefabs;

    [SerializeField]
    private TrayState _trayState;

    [SerializeField]
    private List<OrderImage> _orderImages;

    public GoodsType CurrentGoodsType { get; private set; } = GoodsType.None;

    [Header("�ʿ��� ��ǰ ����")]
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
        //�ʱ�ȭ
        if(_goodsContentText != null)
        {
            _currentGoodsCount = 3;
            _goodsContentText.text = _initialGoodsCount.ToString();//�ʱ� �ֹ� ����(3��)
        }

        if(_orderImages == null || _orderImages.Count == 0)
        {
            Debug.Log("�ֹ� �̹��� �������");
            return;
        }

        int goodsRandomIndex = Random.Range(0, _orderImages.Count);//��������

        for(int i = 0; i<_orderImages.Count; i++)//���õ� �͸� Ȱ��ȭ ������ false
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
       //ù ��° Ʈ���̸� Ÿ�� �����ϴ� �ڵ�

        if (_goodsContentText != null)
        {
            _currentGoodsCount = 3;
            _goodsContentText.text = _initialGoodsCount.ToString();
        }

        if (_orderImages == null || _orderImages.Count == 0)
        {
            Debug.Log("�ֹ� �̹��� �������");
            return;
        }

        for (int i = 0; i < _orderImages.Count; i++)
        {
            bool isTarget = _orderImages[i].GoodsType == fixedType;//���޹��� �Ͱ� ������
            if (_orderImages[i].GoodsImageObject != null)
            {
                _orderImages[i].GoodsImageObject.SetActive(isTarget);//�� ����(�ݶ�)�� Ȱ��ȭ
            }

            if (isTarget)//���� ��ǰ Ÿ���� ���޹��� ������ ����
            {
                CurrentGoodsType = fixedType;
            }
        }

       // DropGoodsReset();
    }

    //�ֹ� ó�� ����
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
                
                _trayState.SetState(TrayStateType.MoveLeft);//�������� �̵�
            }

           
        }

        else
        {
            AddGoods();

            _currentGoodsCount -= amount;
            _goodsContentText.text = _currentGoodsCount.ToString();
        }

    }

    //���
    public void AddGoods()
    {
        Transform dropPos = GetRandomDropPosition();
        GameObject dropPrefab = GetDropPrefabByGoodsType(CurrentGoodsType);

        if (dropPrefab != null && dropPos != null)
        {
            GameObject droppedObj = Instantiate(dropPrefab, dropPos.position, Quaternion.identity);
            droppedObj.transform.SetParent(dropPos, true); // �ڽ����� ����
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
                // dropPos(��� ��ġ ������Ʈ)�� �ڽ� ������Ʈ���� ��� ����
                for (int i = dropPos.childCount - 1; i >= 0; i--)
                {
                    GameObject child = dropPos.GetChild(i).gameObject;
                    Destroy(child);
                }
            }
        }
    }
}
