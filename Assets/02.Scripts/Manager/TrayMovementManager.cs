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

    public void RegisterTray(TrayState tray)//����
    {
        if(!_activeTrays.Contains(tray))
        {
            _activeTrays.Add(tray);
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

        //Ʈ���Ͽ��� ���ǵ� ����
        foreach(var tray in _activeTrays)
        {
            tray.SetMoveSpeed(_moveDownSpeed);
        }
    }

    public void StopMoving()//���� ������ Ʃ�丮��
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
