using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayMovementManager : MonoBehaviour
{
    [SerializeField]
    private ConveyorBeltScrolling _conveyorBeltScrolling;

    [SerializeField]
    private float _moveDownSpeed = 2f;

    private List<TrayState> _activeTrays = new();
    private bool _isMovingDown = false;

    private void Update()
    {
        if(!_isMovingDown)
        {
            return;
        }

        foreach (var tray in _activeTrays)
        {
            if(tray.CurrentState == TrayStateType.Stop)
            {
                tray.SetState(TrayStateType.MoveDown);
            }
        }
    }

    public void RegisterTray(TrayState tray)//스폰
    {
        if(!_activeTrays.Contains(tray))
        {
            _activeTrays.Add(tray);
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

        //트레일에도 스피드 전달
        foreach(var tray in _activeTrays)
        {
            tray.SetMoveSpeed(_moveDownSpeed);
        }
    }

    public void StopMoving()//게임 오버나 튜토리얼
    {
        _isMovingDown = false;
        _conveyorBeltScrolling.StopScrolling();

        foreach(var tray in _activeTrays)
        {
            tray.SetState(TrayStateType.Stop);
        }
    }

    public void OnStartMoveButtonPressed()
    {
        StartMoveingDown();
    }
}
