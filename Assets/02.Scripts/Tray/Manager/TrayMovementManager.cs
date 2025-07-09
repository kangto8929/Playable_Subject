using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayMovementManager : MonoBehaviour
{
    [SerializeField]
    private ConveyorBeltScrolling _conveyorBeltScrolling;


    private float _moveDownSpeed = 20f;
    public float MoveDownSpeed => _moveDownSpeed;

    private List<TrayState> _activeTrays = new();
    private bool _isMovingDown = false;

    public bool IsMovingDown => _isMovingDown;

    public void RegisterTray(TrayState tray)//스폰
    {
        if (!_activeTrays.Contains(tray))
        {
            _activeTrays.Add(tray);

            // 게임이 진행 중이면 바로 MoveDown 상태로 설정
            if (_isMovingDown && tray.CurrentState == TrayStateType.Stop)
            {
                tray.SetMoveSpeed(_moveDownSpeed);
                tray.SetState(TrayStateType.MoveDown);
            }
        }
    }

    public void UnregisterTray(TrayState tray)//풀로 돌아갈 때
    {
        if (_activeTrays.Contains(tray))
        {
            _activeTrays.Remove(tray);
        }
    }

    public void StartMoveingDown()
    {
        //컨베이어 벨트랑 같이 작동
        _isMovingDown = true;
        _conveyorBeltScrolling.StartScrolling(_moveDownSpeed);

        foreach (var tray in _activeTrays)
        {
            tray.SetMoveSpeed(_moveDownSpeed);
            if (tray.CurrentState == TrayStateType.Stop)
            {
                tray.SetState(TrayStateType.MoveDown);
            }
        }
    }

    public void StopMoving()//게임 오버나 튜토리얼
    {
        _isMovingDown = false;
        _conveyorBeltScrolling.StopScrolling();
        foreach (var tray in _activeTrays)
        {
            tray.SetState(TrayStateType.Stop);
        }
    }

    public void OnStartMoveButtonPressed()
    {
        StartMoveingDown();
    }
}

