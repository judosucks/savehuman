using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public class Player : Entity
{
    
    private Vector2 workSpace;
    public EntityFX entityFX;
    public HeadDetection headDetection;
    public SkillManager skill { get; private set; }
    public GameObject grenade { get; private set; }
    public PlayerInput playerInput;
    public bool isBusy { get; private set; } // 私有字段

    public void SetIsBusy(bool value) // 公开方法设置属性
    {
        isBusy = value;
    }
    public bool isAttacking { get; set; } // 公开属性，用于指示玩家当前是否处于攻击状态

    public Vector2 CurrentVelocity { get; private set; }
    private Vector2 workspace;
    

    public PlayerInputController inputController { get; private set; }
    public PlayerStateMachine stateMachine { get; private set; }
    
    public PlayerState playerState { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerStraightJumpState straightJumpState { get; private set; }
    public PlayerStraightJumpAirState straightJumpAirState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerStraightJumpLandState straightJumpLandState { get; private set; }
    public PlayerRunJumpLandState runJumpLandState { get; private set; }
    public PlayerSprintJumpInAirState sprintJumpInAirState { get; private set; }
    public PlayerSprintJumpState sprintJumpState { get; private set; }
    public PlayerSprintJumpLandState sprintJumpLandState { get; private set; }
    public PlayerClimbState climbState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }

    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerCrossKickState crossKickState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    public PlayerSprintState sprintState { get; private set; }
    public PlayerLedgeClimbState ledgeClimbState { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerThrowGrenadeState throwGrenadeState { get; private set; }
    
    public PlayerBlackholeState blackholeState { get; private set; }
   
    public PlayerKneeKickState kneeKickState { get; private set; }
    
    public PlayerDeadState deadState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        
        stateMachine = new PlayerStateMachine();
        playerInput = GetComponent<PlayerInput>();
        inputController = GetComponent<PlayerInputController>();
        entityFX = GetComponent<EntityFX>();
        leftEdgeTrigger.isNearLeftEdge = true;
        rightEdgeTrigger.isNearRightEdge = true;
        if (anim == null)
        {
            Debug.LogError("Animator component is missing in children.");
        }

        sprintJumpState = new PlayerSprintJumpState(this, stateMachine, playerData, "SprintJump");
        sprintJumpInAirState = new PlayerSprintJumpInAirState(this, stateMachine, playerData, "SprintJump");
        sprintJumpLandState = new PlayerSprintJumpLandState(this, stateMachine, playerData, "SprintJumpLand");
        runJumpLandState = new PlayerRunJumpLandState(this, stateMachine, playerData, "Land");
        straightJumpLandState = new PlayerStraightJumpLandState(this, stateMachine, playerData, "StraightLand");
        sprintState = new PlayerSprintState(this, stateMachine,playerData, "Sprint");
        crossKickState = new PlayerCrossKickState(this, stateMachine, playerData,"CrossKick");
        straightJumpState = new PlayerStraightJumpState(this, stateMachine,playerData, "Jump");
        straightJumpAirState = new PlayerStraightJumpAirState(this, stateMachine, playerData,"Jump");
        idleState = new PlayerIdleState(this, stateMachine, playerData,"Idle");
        moveState = new PlayerMoveState(this, stateMachine, playerData,"Move");
        jumpState = new PlayerJumpState(this, stateMachine, playerData,"RunJump");
        airState = new PlayerAirState(this, stateMachine, playerData,"RunJump");
        dashState = new PlayerDashState(this, stateMachine,playerData, "Dash");
        wallJumpState = new PlayerWallJumpState(this, stateMachine,playerData, "WallJump");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, playerData,"WallSlide");
        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, playerData,"Attack");
        ledgeClimbState = new PlayerLedgeClimbState(this,stateMachine,playerData,"LedgeClimbState");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, playerData,"CounterAttack");
        kneeKickState = new PlayerKneeKickState(this, stateMachine, playerData,"KneeKick");
        throwGrenadeState = new PlayerThrowGrenadeState(this, stateMachine, playerData,"AimGrenade");
        blackholeState = new PlayerBlackholeState(this, stateMachine,playerData, "Blackhole");
        climbState = new PlayerClimbState(this, stateMachine,playerData, "Climb");
        deadState = new PlayerDeadState(this, stateMachine,playerData, "Dead");
    }

    protected override void Start()
    {
        EnsureSkillManagerIsInitialized(); // 确保初始化
        base.Start();
        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);
        playerData.defaultJumpForce = playerData.jumpForce;
        playerData.defaultMoveSpeed = playerData.movementSpeed;
        playerData.defaultDashSpeed = playerData.dashSpeed;
        if (SkillManager.instance == null)
        {
            Debug.LogError("SkillManager is null");
            return;
        }
        skill = SkillManager.instance;

        if (skill.dashSkill == null)
        {
            Debug.LogError("dashSkill is not initialized in SkillManager.");
            return;
        }
    }
    private void EnsureSkillManagerIsInitialized()
    {
        if (SkillManager.instance == null)
        {
            SkillManager.instance = Object.FindFirstObjectByType<SkillManager>();
            if (SkillManager.instance == null)
            {
                Debug.LogError("SkillManager instance could not be initialized.");
            }
        }
    }
    protected override void Update()
    {
       
        
        stateMachine?.currentState?.Update();
        if (isBusy)
        { 
            return; // 如果玩家处于忙碌状态，禁止其他输入
        } 
        DashInput(); // 冲刺输入处理
    }

    public void CheckForCurrentVelocity()
    {
        CurrentVelocity = rb.linearVelocity;
    }
    public override void SlowEntityBy(float _slowPercent, float _slowDuration)
    {
        playerData.movementSpeed = playerData.movementSpeed * (1 - _slowPercent);
        playerData.dashSpeed = playerData.dashSpeed * (1 - _slowPercent);
        playerData.jumpForce = playerData.jumpForce * (1 - _slowPercent);
        playerData.straightJumpForce = playerData.straightJumpForce * (1 - _slowPercent);
        anim.speed = anim.speed * (1 - _slowPercent);
        Invoke("ReturnDefaultSpeed", _slowDuration);
        
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        playerData.movementSpeed = playerData.defaultMoveSpeed;
        playerData.dashSpeed = playerData.defaultDashSpeed;
        playerData.jumpForce = playerData.defaultJumpForce;
        playerData.straightJumpForce = playerData.defaultStraightJumpForce;

    }
    public void AssignNewGrenade(GameObject _newGrenade)
    {
        if (grenade)
        {
            Debug.LogWarning("Grenade already assigned! Destroying the new one.");
            Destroy(_newGrenade);
            return;
        }

        Debug.Log("Assigning new grenade...");
        grenade = _newGrenade;
    }

    public void ClearGrenade()
    {
        if (grenade == null) {
            Debug.LogWarning("No grenade to clear.");
            return;
        }

        Debug.Log("[ClearGrenade] Destroying grenade after delay...");
        StartCoroutine(DestroyGrenadeAfterDelay(.1f));
    }

    private IEnumerator DestroyGrenadeAfterDelay(float delay)
    {
        if (grenade == null) {
            Debug.LogWarning("[DestroyGrenadeAfterDelay] No grenade to destroy.");
            yield break;
        }

        yield return new WaitForSeconds(delay);

        if (grenade != null) {
            Debug.Log("[DestroyGrenadeAfterDelay] Destroying grenade...");
            Destroy(grenade);
            grenade = null;
        }
    }

    public void CancelThrowGrenade()
    {
        Debug.Log("[CancelThrowGrenade] Called");
        playerData.grenadeCanceled = true;
        // Destroy grenade object
        if (grenade != null)
        {
            Debug.Log("[CancelThrowGrenade] Destroying grenade");
            Destroy(grenade);
            grenade = null;
        }

        // Disable grenade aiming dots for visualization
        Debug.Log("[CancelThrowGrenade] Disabling aim dots");
        SkillManager.instance.grenadeSkill.DotsActive(false);

        // Reset player states
        OnAimingStop();
        if (anim.GetBool("AimGrenade"))
        {
            Debug.Log("set to false aimgrenade");
          anim.SetBool("AimGrenade", false);
          anim.SetTrigger("AimAbort");
        }
        
        stateMachine.ChangeState(idleState);
        if (!anim.GetBool("Idle"))
        {
            Debug.Log("idle is not set setting it now");
            anim.SetBool("Idle",true);
        }
        
        

    }


    private void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        
        yield return new WaitForSeconds(_seconds);

        isBusy = false;
        
    }

    
    
    private void DashInput()
    {
       
        if (isBusy || IsWallDetected() || !IsGroundDetected())
        {
            
            return;
        }


        if ((Keyboard.current.fKey.wasPressedThisFrame && skill.dashSkill.CanUseSkill()) || (Gamepad.current != null &&
                Gamepad.current.buttonEast.wasPressedThisFrame && skill.dashSkill.CanUseSkill()))
        {

            stateMachine.ChangeState(dashState);
        }

    }
    #region velocity

    public void StopUpwardVelocity()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, 0f));
        CurrentVelocity = rb.linearVelocity; // Keep this in sync
    }
    
    public void ZeroVelocity()
    {
        if (isKnocked)
        {
            return; //if knocked can not move
        }
        rb.linearVelocity = new Vector2(0f, 0f);
        CurrentVelocity = rb.linearVelocity; // Keep this in sync
    }
    
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
        {
            return; //if knocked can not move
        }
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        CurrentVelocity = rb.linearVelocity; // Keep this in sync
        if (IsGroundDetected()) FlipController(xVelocity);
    }
    public void MoveInAir()
    {
        CurrentVelocity = rb.linearVelocity;
        var velocity = CurrentVelocity;
        velocity.x = inputController.norInputX * playerData.horizontalSpeed;
        CurrentVelocity = velocity;
        
    }
    public void SetVelocityY(float yVelocity)
    {
        if (isKnocked)
        {
            return;
        }
        workspace.Set(rb.linearVelocity.x, yVelocity);
        rb.linearVelocity = workspace;
        CurrentVelocity = workspace; // Keep this in sync
        
    }

    public void SetVelocityX(float xVelocity)
    {
        if (isKnocked)
        {
            return;
        }
        workspace.Set(xVelocity, rb.linearVelocity.y);
        rb.linearVelocity = workspace;
        CurrentVelocity = workspace; // Keep in sync
        if (IsGroundDetected()) FlipController(xVelocity);
        
    }
    #endregion

    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(transform.position, Vector2.right * facingDirection, playerData.ledgeCheckDistance, playerData.whatIsGround);
        float xDistance = xHit.distance;
        workSpace.Set(xDistance * facingDirection, 0f);

        // 将workSpace转换为Vector3
        Vector3 workspace3 = new Vector3(workSpace.x, workSpace.y, 0f);
    
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + workspace3, Vector2.down, ledgeCheck.position.y - ledgeCheck.position.y, playerData.whatIsGround);
        float yDistance = yHit.distance;
        workSpace.Set(ledgeCheck.position.x + (xDistance * facingDirection), ledgeCheck.position.y - yDistance);
        
        return workSpace;
    }
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    
    public void OnDashAttackFrame()
    {
        
        stateMachine.currentState.CanPerformDashAttack();
    }
    public void OnDashAttackComplete()
    {
        stateMachine.currentState.CanNotPerformDashAttack();
    }

    public void OnPerformCrossKick()
    {
        
        stateMachine.currentState.PerformCrossKick();
    }

    public void OnCrossKickComplete()
    {
        
        stateMachine.currentState.PerformRegularAttack();
    }

    public void OnAimingStart()
    {
        playerData.isAiming = true;
        Debug.Log("isaiming is set to true");
    }

    public void OnAimingStop()
    {
        Debug.Log("isaiming is set to false");
        playerData.isAiming = false;
    }

    

   
    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
   
}