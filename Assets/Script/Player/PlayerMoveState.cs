using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class PlayerMoveState : PlayerGroundedState
{
    
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine,PlayerData _playerData, string _animBoolName) : base(_player,
        _stateMachine,_playerData, _animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        playerData.isRun = true;
    }  

    public override void Exit()
    {
        base.Exit();
        playerData.isRun = false;
    }

    public override void Update()
    {
        base.Update();
        

        if (!player.isAttacking && !player.isBusy && xDirection != 0) 
        {
            
        player.SetVelocityX(xDirection * playerData.movementSpeed);
        
        }

        if (player.inputController.sprintInput && xDirection != 0)
        {
            stateMachine.ChangeState(player.sprintState);
        }else if (!player.inputController.sprintInput && xDirection == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }else if (player.inputController.sprintInput && xDirection == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
       
    }

    // private void SprintInput()
    // {
    //     if (Keyboard.current.leftShiftKey.isPressed && xDirection != 0)
    //     {
    //         
    //         stateMachine.ChangeState(player.sprintState); // 切换到冲刺状态
    //     }
    // }

}
