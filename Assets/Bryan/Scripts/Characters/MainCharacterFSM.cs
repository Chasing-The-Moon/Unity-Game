using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterFSM : MonoBehaviour, IKillable
{
    [Header("Character Settings")]
    [SerializeField] private bool characterUpOrDown;
    [SerializeField] private MainCharacterFSM otherCharacter;
    [Header("Movements Settings")]
    [SerializeField] private float speed;
    [Header("On Air Settings")]
    [SerializeField][Range(0f,1f)] private float coefficientSpeedOnAir;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [Header("Smash Settings")]
    [SerializeField] private float smashForce;
    [Header("Climb Settings")]
    [SerializeField][Range(0f,1f)] private float coefficientSpeedClimbing;
    [SerializeField][Range(0f,0.1f)] private float thresholdY;
    [Header("Throw Arm Settings")]
    [SerializeField][Range(0f, 1.5f)] private float speedTransition;
    [Header("Spawn Point")]
    [SerializeField] private Transform Spawn; 

    #region Finite State Machine
    private FSM fsmMC;
    #region States of FSM
    private FSMState IdleState;
    private FSMState MovementFloorState;
    private FSMState OnAirState;
    private FSMState ClimbState;
    private FSMState ThrowArmState;
    private FSMState AttackState;
    #endregion
    #region Actions of FSM
    private IdleAction Idle;
    private MovementAction MovementFloor;
    private MovementAction MovementAir;
    private JumpAction JumpFloor;
    private JumpAction JumpAir;
    private JumpAction JumpClimb;
    private SmashAction Smash;
    private AttackPlayer Attack;
    [HideInInspector] public CheckClimbAction CheckClimbOnFloor;
    [HideInInspector] public CheckClimbAction CheckClimbOnAir;
    [HideInInspector] public ClimbAction Climb;
    [HideInInspector] public ThrowArmAction ThrowArm;
    #endregion
    #endregion
    #region Private Variables
    private Rigidbody2D myRigidbody;
    private BoxCollider2D myCollider;
    private Animator myAnimator;
    private Coroutine InteractionCoroutine;
    private string lastState;
    private bool triggeringFloor;
    #endregion
    #region Public Variable
    [HideInInspector] public bool onControl;
    [HideInInspector] public bool onTransitionUnion;
    [HideInInspector] public Vector2 targetPosTransition;
    #endregion
    #region Gets And Sets
    public bool GetCharacterUpOrDown()
    {
        return characterUpOrDown;
    }
    public bool GetTriggeringFloor()
    {
        return triggeringFloor;
    }
    public MainCharacterFSM GetOtherCharacter()
    {
        return otherCharacter;
    }
    public FSMState GetCurrentState()
    {
        return fsmMC.GetCurrentState();
    }
    public FSMState GetMovementState()
    {
        return MovementFloorState;
    }
    public FSMState GetIdleState()
    {
        return IdleState;
    }
    public bool IsSmashing()
    {
        return Smash.GetIsSmashing();
    }
    public void SetOnIdle()
    {
        lastState = GetCurrentState().Name;
        GetCurrentState().SendEvent("ToIdle");
    }
    public void SetOnMovement()
    {
        IdleState.SendEvent("ToMovementFloorState");
    }
    public void SetOnControl(bool onControl)
    {
        this.onControl = onControl;
    }
    #endregion
    private void Awake()
    {
        onControl = true;
        onTransitionUnion = false;
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        // Instantiate FSM
        fsmMC = new FSM("MCTest FSM");
        // Add States of FSM
        IdleState = fsmMC.AddState("IdleState");
        MovementFloorState = fsmMC.AddState("MovementState");
        OnAirState = fsmMC.AddState("OnAirState");
        ClimbState = fsmMC.AddState("ClimbState");
        ThrowArmState = fsmMC.AddState("ThrowArmState");
        AttackState = fsmMC.AddState("Attack");

        // Idle Actions
        Idle = new IdleAction(IdleState);
        // MovementFloorState Actions
        MovementFloor = new MovementAction(MovementFloorState);
        JumpFloor = new JumpAction(MovementFloorState);
        CheckClimbOnFloor = new CheckClimbAction(MovementFloorState);
        // OnAirState Actions
        MovementAir = new MovementAction(OnAirState);
        JumpAir = new JumpAction(OnAirState);
        Smash = new SmashAction(OnAirState);
        CheckClimbOnAir = new CheckClimbAction(OnAirState);
        // Climb Actions
        Climb = new ClimbAction(ClimbState);
        JumpClimb = new JumpAction(ClimbState);
        // ThrowArm Actions
        ThrowArm = new ThrowArmAction(ThrowArmState);
        // Attack State
        Attack = new AttackPlayer(AttackState);

        // Adds actions to the state
        IdleState.AddAction(Idle);
        MovementFloorState.AddAction(MovementFloor);
        MovementFloorState.AddAction(JumpFloor);
        MovementFloorState.AddAction(CheckClimbOnFloor);
        OnAirState.AddAction(MovementAir);
        OnAirState.AddAction(JumpAir);
        OnAirState.AddAction(Smash);
        OnAirState.AddAction(CheckClimbOnAir);
        ClimbState.AddAction(Climb);
        ClimbState.AddAction(JumpClimb);
        ThrowArmState.AddAction(ThrowArm);
        AttackState.AddAction(Attack);

        // Set transition to the states
        IdleState.AddTransition("ToMovement", MovementFloorState);
        IdleState.AddTransition("ToClimb", ClimbState);
        IdleState.AddTransition("ToAttack", AttackState);
        MovementFloorState.AddTransition("ToClimb", ClimbState);
        MovementFloorState.AddTransition("ToOnAir", OnAirState);
        MovementFloorState.AddTransition("ToThrowArm", ThrowArmState);
        MovementFloorState.AddTransition("ToIdle", IdleState);
        MovementFloorState.AddTransition("ToAttack", AttackState);
        OnAirState.AddTransition("ToMovement", MovementFloorState);
        OnAirState.AddTransition("ToClimb", ClimbState);
        OnAirState.AddTransition("ToIdle", IdleState);
        ClimbState.AddTransition("ToMovement", MovementFloorState);
        ClimbState.AddTransition("ToOnAir", OnAirState);
        ClimbState.AddTransition("ToIdle", IdleState);
        ThrowArmState.AddTransition("ToMovement", MovementFloorState);
        ThrowArmState.AddTransition("ToIdle", IdleState);
        AttackState.AddTransition("ToMovement", MovementFloorState);
        AttackState.AddTransition("ToOnAir", OnAirState);

        // Initializes the actions
        MovementFloor.Init(transform, speed);
        JumpFloor.Init(jumpForce, true, "ToOnAir",myRigidbody, myCollider, Smash);
        CheckClimbOnFloor.Init(thresholdY, characterUpOrDown, true, myCollider, "ToClimb");
        MovementAir.Init(transform, speed * coefficientSpeedOnAir);
        JumpAir.Init(jumpForce, false, "",myRigidbody, myCollider, Smash);
        Smash.Init(smashForce, characterUpOrDown, myRigidbody, myCollider);
        CheckClimbOnAir.Init(thresholdY, characterUpOrDown, false, myCollider, "ToClimb");
        Climb.Init(speed * coefficientSpeedClimbing, thresholdY, characterUpOrDown, myRigidbody, myCollider, transform, this, "ToMovement");
        JumpClimb.Init(jumpForce, true, "ToOnAir", myRigidbody, myCollider, Smash);
        ThrowArm.Init(speedTransition, myRigidbody, myCollider, transform);

        // Starts FSM
        fsmMC.Start("OnAirState");
    }
    // Update is called once per frame
    private void Update()
    {
        fsmMC.Update();
    }
    void FixedUpdate()
    {
        fsmMC.FixedUpdate();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("ON FLOOR");
            if(GetCurrentState().Name == "OnAirState" || GetCurrentState().Name == "ThrowArmState")
                GetCurrentState().SendEvent("ToMovement");
            return;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            if(GetCurrentState().Name == "ClimbState")
                triggeringFloor = true;
            if (GetCurrentState().Name == "ThrowArmState")
                ThrowArmState.SendEvent("ToMovement");
            return;
        }
        IInteractable objectInteractable = collision.GetComponent<IInteractable>();
        if (objectInteractable != null)
            InteractionCoroutine = StartCoroutine(PressKeyInteraction(objectInteractable));
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            if(triggeringFloor && GetCurrentState().Name == "OnAirState")
            {
                myCollider.isTrigger = false;
            }
            triggeringFloor = false;
            return;
        }
        IInteractable objectInteractable = collision.GetComponent<IInteractable>();
        if (objectInteractable != null) 
            StopCoroutine(InteractionCoroutine);
    }
    #region Coroutines
    private IEnumerator PressKeyInteraction(IInteractable objectInteractable)
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Interact"));
        Debug.Log("HAZ LA INTERACCION");
        objectInteractable.Use();
        if (!objectInteractable.canMove)
        {
            onControl = false;
            otherCharacter.onControl = false;
            /*GetCurrentState().SendEvent("ToIdle");
            if(otherCharacter.GetCurrentState().Name != "IdleState")
                otherCharacter.GetCurrentState().SendEvent("ToIdle");*/
            SetOnIdle();
            otherCharacter.SetOnIdle();

        } else 
        {
            MovementFloor.SetSpeed(speed*objectInteractable.coefficientSpeed);
        }
    }
    private IEnumerator MoveAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        MoveAgain();
    }
    #endregion
    public void Kill()
    {
        if (GetCurrentState().Name == "ThrowArmState")
            ThrowArmState.SendEvent("ToMovement");
        myRigidbody.velocity = Vector2.zero;
        transform.position = new Vector3(Spawn.position.x, Spawn.position.y, transform.position.z);
        //otherCharacter.Kill();
    }
    #if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        BoxCollider2D myCollider;
        Vector3 origin;
        float boundX;
        myCollider = GetComponent<BoxCollider2D>();
        boundX = myCollider.bounds.min.x + myCollider.size.x/2;
        origin = characterUpOrDown ? new Vector3(boundX, myCollider.bounds.min.y, transform.position.z) : new Vector3(boundX, myCollider.bounds.max.y, transform.position.z);
        Gizmos.DrawRay(new Ray(origin, Vector3.down * GetComponent<Rigidbody2D>().gravityScale));
    }
#endif
    #region Public Functions
    private void FinishAttackState()
    {
        /*
        if (onAir)
            AttackState.SendEvent("ToOnAir");
        else
            AttackState.SendEvent("ToMovement");*/
    }
    public void MoveAgain()
    {
        if (GetCurrentState().Name == "IdleState" && lastState != "IdleState")
        {
            IdleState.SendEvent("ToMovement");
            onControl = true;
        }
           
    }
    public void MoveAfterInteractions(float time)
    {
        StartCoroutine(MoveAfterTime(time));
    }
    #endregion
}