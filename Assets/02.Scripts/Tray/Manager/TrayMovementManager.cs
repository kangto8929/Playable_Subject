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

    public void RegisterTray(TrayState tray)//����
    {
        if (!_activeTrays.Contains(tray))
        {
            _activeTrays.Add(tray);

            // ������ ���� ���̸� �ٷ� MoveDown ���·� ����
            if (_isMovingDown && tray.CurrentState == TrayStateType.Stop)
            {
                tray.SetMoveSpeed(_moveDownSpeed);
                tray.SetState(TrayStateType.MoveDown);
            }
        }
    }

    public void UnregisterTray(TrayState tray)//Ǯ�� ���ư� ��
    {
        if (_activeTrays.Contains(tray))
        {
            _activeTrays.Remove(tray);
        }
    }

    public void StartMoveingDown()
    {
        //�����̾� ��Ʈ�� ���� �۵�
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

    public void StopMoving()//���� ������ Ʃ�丮��
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

