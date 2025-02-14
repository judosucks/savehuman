using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }

    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(PlayerState _newState)
    {
        //     if (currentState != null)
        //     {
        //         currentState.Exit();
        //     }
        //     currentState = _newState;
        //     if (currentState != null)
        //     {
        //         currentState.Enter();
        //     }
        //     Debug.Log($"[Player State] Current state: {currentState}"+PlayerManager.instance.player.isAiming);
        //     //
        //     // currentState.Exit();
        //     // currentState = _newState;
        //     // currentState.Enter();
        // }
        if (currentState != null)
        {
            Debug.Log("Exiting state: "+currentState.GetType().Name+""+PlayerManager.instance.player.inputController.isJumping);

            // 添加保护，防止递归切换
            if (currentState == _newState)
            {
                Debug.LogWarning("Attempting to switch to the same state. Ignoring to prevent recursion.");
                return;
            }

            currentState.Exit(); // 调用当前状态的 Exit 方法
        }

        currentState = _newState; // 更新状态
        Debug.Log("Entering state:" +_newState.GetType().Name+""+PlayerManager.instance.player.inputController.isJumping);
        currentState.Enter(); // 调用新状态的 Enter 方法
    }
}
