using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public enum TrayStateType
{
    MoveDown,//�Ʒ��� �̵�
    MoveLeft,//�������� �̵� (�ֹ� ó�� �������� ��)
    Stop,//����(���� ����, Ʃ�丮��)
}

public class TrayState : MonoBehaviour
{
    private TraySpawner _traySpawner;
    private TrayMovementManager _trayMovementManager;
    public TrayStateType CurrentState { get; private set; } = TrayStateType.Stop;//ó������ ����� ��.

    private float _moveDownSpeed;//�Ʒ��� �������� �ӵ�

    [Header("�ֹ� ������ �������� �̵�")]
    [SerializeField]
    private float _moveLeftSpeed = 3f;
    [SerializeField]
    private float _leftTargetX = -62.1f;
    [SerializeField]
    private float _moveLeftDuration = 1.5f; // �� �� ���� �̵�����

    public void Init(TraySpawner traySpawner)
    {
        this._traySpawner = traySpawner;
        _trayMovementManager = FindObjectOfType<TrayMovementManager>();
        _trayMovementManager.RegisterTray(this);
    }

    private void OnEnable()
    {
        //������ ��,Ʃ�丮�󿡼��� ����� ��
        CurrentState = TrayStateType.Stop;
    }

    private void Update()
    {
        switch(CurrentState)
        {
            case TrayStateType.MoveDown:
                MoveDown();
                break;
            case TrayStateType.MoveLeft:
                MoveLeft();
                break;
            case TrayStateType.Stop:
                break;
        }
    }

    private void MoveDown()
    {
        transform.position += Vector3.down * _moveDownSpeed * Time.deltaTime;
    }

    private void MoveLeft()
    {
        //��ǥ ��ġ���� �̵� �� �ٽ� Ǯ�� ����������
        transform.DOMoveX(_leftTargetX, _moveLeftDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                SetState(TrayStateType.Stop);
                ReturnToPool();
            });
    }

    private void ReturnToPool()
    {
        _trayMovementManager.UnregisterTray(this);
        _traySpawner.RecycleTray(gameObject);
    }

    public void SetState(TrayStateType newState)
    {
        CurrentState = newState;
    }

    public void SetMoveSpeed(float speed)
    {
        _moveDownSpeed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GameOver"))
        {
            Debug.Log("Game Over ������!");
            _trayMovementManager.StopMoving();
        }
    }

}
