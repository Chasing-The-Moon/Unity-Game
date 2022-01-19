using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacters : MonoBehaviour
{
    [Header("Main Characters")]
    [SerializeField] private MainCharacterFSM characterUp;
    [SerializeField] private MainCharacterFSM characterDown;

    private bool canChange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Change Character") && !canChange)
        {
            canChange = false;
        }
    }

    void FixedUpdate()
    {

    }
}
