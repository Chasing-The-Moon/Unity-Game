using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction: FSMAction
{
    private Rigidbody2D characterRigidbody;
    private SmashAction smashAction;
    private Collider2D characterCollider;
    private float forceJump;
    private int numJumps;
    private string newState;
    private bool canJump;
    private bool changeState;
    public JumpAction (FSMState owner): base(owner) { }
    public void Init(float forceJump, bool changeState, string newState, Rigidbody2D characterRigidbody, Collider2D characterCollider, SmashAction smashAction)
    {
        this.forceJump = forceJump;
        this.characterRigidbody = characterRigidbody;
        this.smashAction = smashAction;
        this.changeState = changeState;
        this.newState = newState;
        this.characterCollider = characterCollider;
        numJumps = 1;
        canJump = false;
    }
    public override void OnEnter()
    {
        // Change animation to Jump
    }
    public override void OnUpdate()
    {
        if(Input.GetButtonDown("Jump") && numJumps > 0 && !smashAction.GetIsSmashing())
        {
            canJump = true;
            numJumps--;
        }
    }
    public override void OnFixedUpdate()
    {
        if(canJump)
        {
            Debug.Log("JUMP ACTION");
            characterRigidbody.bodyType = RigidbodyType2D.Dynamic;
            //characterCollider.isTrigger = false;
            characterRigidbody.velocity = new Vector2(characterRigidbody.velocity.x, 0);
            characterRigidbody.AddForce(Vector2.up * forceJump * characterRigidbody.gravityScale);
            if(changeState)
                FinishState(newState);
            canJump = false;
        }
    }
    public override void OnExit()
    {
        canJump = false;
        numJumps = 1;
    }
    private void FinishState(string state)
    {
        GetOwner().SendEvent(state);
    }
}
