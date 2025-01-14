using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

public class PlayerLedgeClimbState : PlayerState
{
    private Vector2 detectedPos;
    private Vector2 cornerPos;
    private Vector2 startPos;
    private Vector2 stopPos;
    private bool isClimbing;
    private int xInput;
    private int yInput;
    public PlayerLedgeClimbState(Player _player, PlayerStateMachine _stateMachine,PlayerData _playerData, string _animBoolName) : base(_player, _stateMachine,_playerData, _animBoolName)
    {
        
    }
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        triggerCalled = true;
        Debug.Log("AnimationFinishTrigger player.ishanging"+playerData.isHanging);
        player.anim.SetBool("ClimbLedge", false);
    }
    public override void Enter()
    {
        base.Enter();
        playerData.isClimbLedge = isClimbing;
        player.ZeroVelocity();
        player.transform.position = detectedPos;
        cornerPos = player.DetermineCornerPosition();
        
        startPos.Set(cornerPos.x - (player.facingDirection * playerData.startOffset.x),cornerPos.y - playerData.startOffset.y);
        stopPos.Set(cornerPos.x+(player.facingDirection * playerData.stopOffset.x),cornerPos.y + playerData.stopOffset.y);
        player.transform.position = startPos;
    }

    public override void Exit()
    {
        base.Exit();
        playerData.isHanging = false;
        triggerCalled = false;
        if (isClimbing)
        {
            Debug.Log("isclimbing"+isClimbing);
            player.transform.position = stopPos;
            isClimbing = false;
        }
        // 启用玩家控制
        // player.SetIsBusy(false);
        // // // 重置悬崖攀爬动画
        // // player.anim.SetBool("isClimbing", false);
    }

    public override void Update()
    {
        base.Update();
        if (triggerCalled)
        {
            
            stateMachine.ChangeState(player.idleState);
            Debug.Log("idle from ledge climb");
        }
        else
        {
            xInput = Mathf.RoundToInt(xDirection);
            yInput = Mathf.RoundToInt(yDirection);
            
            player.ZeroVelocity();
            player.transform.position = startPos;
            if (xInput == player.facingDirection && playerData.isHanging && !isClimbing)
            {
                isClimbing = true;
                player.anim.SetBool("ClimbLedge", true);
            }else if (yInput == -1 && playerData.isHanging && !isClimbing)
            { 
                stateMachine.ChangeState(player.straightJumpAirState);
                //change to wall slide state after animation clip is made 
            }
        }
       
        
        // // 如果动画结束，移动角色到平台上
        // if (player.anim.GetCurrentAnimatorStateInfo(0).IsName("EndOfClimb"))
        // {
        //     stateMachine.ChangeState(player.idleState);
        //     player.transform.position = player.climbOverPosition;
        // }
    }
    public void SetDetectedPosition(Vector2 pos)=> detectedPos = pos;
}

