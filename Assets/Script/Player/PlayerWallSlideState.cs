using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallSlideState : PlayerState
{
    private bool isTouchingGround;
    private bool isTouchingWall;
    private bool isTouchingLedge;
    private int xInput;
    private int yInput;
    private bool isClimbing;
    private bool isWallBottomDetected;
    private bool runJumpInput;
    private bool sprintJumpInput;
    private bool straightJumpInput;
    private bool isEdgeGrounded;
    private bool isAnimationReversed;
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, PlayerData _playerData,
        string _animBoolName) : base(_player,
        _stateMachine, _playerData, _animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        playerData.isWallSliding = true;
        player.inputController.isJumping = false;
        
    }

    public override void Exit()
    {
        base.Exit();
        playerData.isWallSliding = false;
        isTouchingGround = false;
        isTouchingWall = false;
        isTouchingLedge = false;
        isClimbing = false;
        isWallBottomDetected = false;
        runJumpInput = false;
        sprintJumpInput = false;
        straightJumpInput = false;
        playerData.reachedApex = false;
        isEdgeGrounded = false;
        isAnimationReversed = false;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isTouchingGround = player.IsGroundDetected();
        isTouchingWall = player.IsWallDetected();
        isTouchingLedge = player.CheckIfTouchingLedge();
        xInput = player.inputController.norInputX;
        yInput = player.inputController.norInputY;
        runJumpInput = player.inputController.runJumpInput;
        sprintJumpInput = player.inputController.sprintJumpInput;
        straightJumpInput = player.inputController.straightJumpInput;
        isWallBottomDetected = player.IsWallBottomDetected();
        isEdgeGrounded = player.IsEdgeGroundDetected();
        if (!isTouchingLedge && isTouchingWall)
        {
            if (isTouchingLedge)
            {
              isClimbing = true;
              Debug.Log("touching ledge");
              player.ledgeClimbState.SetDetectedPosition(player.transform.position);
            }
        }
    }
    

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        if (!isTouchingLedge && isTouchingWall && isClimbing)
        {
          
            if (isTouchingLedge)
            { 
                Debug.Log("touching ledge");
              isClimbing = false;
              stateMachine.ChangeState(player.ledgeClimbState);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        

         if (!player.IsWallBottomDetected())
        {
            playerData.isWallSliding = false;
            rb.AddForce(Vector2.right * -player.facingDirection * playerData.exitSlideForce, ForceMode2D.Impulse);
            Debug.Log("change to air state !isTouchingWall || xInput != player.facingDirection");
            if (!isTouchingWall)
            {
                Debug.Log("not is touching wall");
               playerData.isWallSliding = false; 
              stateMachine.ChangeState(player.airState);
            }
        }


        if ( playerData.isWallSliding && isTouchingWall)
        {
         
            // Debug.Log("change to air state xinput != 0 && player.facingDirection != xInput");
            // stateMachine.ChangeState(player.straightJumpAirState);
            rb.linearVelocity = new Vector2(0, 0);
            if ( runJumpInput || sprintJumpInput || straightJumpInput)
            {
              
                playerData.highestPoint = player.transform.position.y;
                playerData.isWallSliding = false;
                stateMachine.ChangeState(player.wallJumpState);
                return;
            }
            

            if (xInput != 0 && player.facingDirection != xInput)
            { 
                playerData.isWallSliding = false;
                rb.AddForce(Vector2.right * playerData.exitSlideForce * -player.facingDirection,ForceMode2D.Impulse);
                if (!isTouchingWall)
                {
                  stateMachine.ChangeState(player.airState);
                }
                // change to wall fall state if animation finished
            }
            
            else
            {
              
                player.SetVelocityY(player.rb.linearVelocity.y * .7f);
            }
            
        }
        if (yInput > 0 && playerData.isWallSliding)
        {
            
           
            stateMachine.ChangeState(player.climbState);
        }
        else if (yInput < 0 && playerData.isWallSliding)
        {
            
            
            player.SetVelocityY(playerData.wallSlideDownForce);
        }
        

        

        if (isTouchingGround)
        {
            playerData.isWallSliding = false;
            rb.AddForce(Vector2.right * -player.facingDirection * playerData.exitSlideForce, ForceMode2D.Impulse);  
            stateMachine.ChangeState(player.idleState);
        }

        

    }


}


