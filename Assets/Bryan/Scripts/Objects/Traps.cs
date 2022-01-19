using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour, IKillable
{
    private CapsuleCollider2D myCollider;
    private void Awake() 
    {
        myCollider = GetComponent<CapsuleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            IKillable killable = collision.GetComponent<IKillable>();
            if (killable != null)
            {
                Debug.Log("KILL PLAYER");
                killable.Kill();
            }
        }
    }
    public void Kill()
    {
        myCollider.isTrigger = true;
        Destroy(gameObject);
    }
}
