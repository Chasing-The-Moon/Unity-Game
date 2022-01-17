using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAction: FSMAction
{
    private Rigidbody2D characterRigidbody;
    private SmashAction smashAction;
    private float forceJump;
    private int numJumps;
    private bool canJump;
    private bool onFloor;
    public JumpAction (FSMState owner): base(owner) { }
    public void Init(float forceJump, bool onFloor, Rigidbody2D characterRigidbody, SmashAction smashAction)
    {
        this.forceJump = forceJump;
        this.characterRigidbody = characterRigidbody;
        this.smashAction = smashAction;
        this.onFloor = onFloor;
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
            Debug.Log("PRESS JUMP BUTTON");
            canJump = true;
            numJumps--;
        }
    }
    public override void OnFixedUpdate()
    {
        if(canJump)
        {
            Debug.Log("JUMP ACTION");
            characterRigidbody.velocity = new Vector2(characterRigidbody.velocity.x, 0);
            characterRigidbody.AddForce(Vector2.up * forceJump * characterRigidbody.gravityScale);
            canJump = false;
            if(onFloor)
                FinishState("ToOnAir");
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
