using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public enum TrayStateType
{
    MoveDown,//아래로 이동
    MoveLeft,//왼쪽으로 이동 (주문 처리 성공했을 때)
    Stop,//멈춤(게임 오버, 튜토리얼)
}

public class TrayState : MonoBehaviour
{
    private TraySpawner _traySpawner;
    private TrayMovementManager _trayMovementManager;
    public TrayStateType CurrentState { get; private set; } = TrayStateType.Stop;//처음에는 멈춰야 함.

    private float _moveDownSpeed;//아래로 내려가는 속도

    [Header("주문 성공시 왼쪽으로 이동")]
    [SerializeField]
    private float _moveLeftSpeed = 3f;
    [SerializeField]
    private float _leftTargetX = -62.1f;
    [SerializeField]
    private float _moveLeftDuration = 1.5f; // 몇 초 동안 이동할지

    public void Init(TraySpawner traySpawner)
    {
        this._traySpawner = traySpawner;
        _trayMovementManager = FindObjectOfType<TrayMovementManager>();
        _trayMovementManager.RegisterTray(this);
    }

    private void OnEnable()
    {
        //시작할 때,튜토리얼에서는 멈춰야 함
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
        //목표 위치까지 이동 후 다시 풀로 돌려보내기
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
            Debug.Log("Game Over 감지됨!");
            _trayMovementManager.StopMoving();
        }
    }

}
