using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine,PlayerData _playerData, string _animBoolName) : base(_player,
        _stateMachine,_playerData, _animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        // rb.linearVelocity = new Vector2(rb.linearVelocity.x, playerData.jumpForce);
        player.SetVelocityY( playerData.jumpForce);
        isAbilityDone = true;
    }

    public override void Exit()
    {
        base.Exit();
        
    }

    public override void Update()
    {
        base.Update();
        if (rb.linearVelocity.y < 0)
        {
            Debug.Log("jump state"+" "+player.CurrentVelocity.y);
            stateMachine.ChangeState(player.airState);
        }
       
    }
}
