using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // depends on the event number we do what we want
    // here we only make this script as trigger a click to turn on the event so the camera change to the event camera , inside the camera script we deactivate what we are doing here
    // This Script when the Character be inside the box collider we do [ change the priority of the current camera to go to the Event Camera , then we do zoom in , and when we finishing zoom in we change the periority to the main camera and we reset the event camera values, in case we want to do the event again]
    // the main Camera Controll the Event and the Event Camera, main camera for character & character idle function
    [Header("The Character Main Camera")]
    public GameObject MainCamera; // the main camera object because from it we control the poriorities and the functions of the camera
    [Header("The Event Camera")]
    public GameObject SecCamera; // we put the seccond camera which is the Event Camera to set the values we want after we change the periorities
    [Header("Event Number")]
    public int EventNumber; // to set the event number into the Main Camera Script (1 is for zoomIn , 2 for transition only , 3 zoomOut)
    [Header("Zoom Value [ZoomIn-ZoomOut] (0.1 to 15)")]
    [Range(0.1f, 15f)]
    public float EventZoomValue;  // to control the Zoom Value whether was it ZoomIn or ZoomOut
    [Header("Speed Value [Fast-Slow] (0.1 to 5)")]
    [Range(0.1f, 5f)]
    public float Speed = 0.5f; // to control the Speed Value in all the functions when it is low number is mean we are moving faster
    [Header("Choose one of the Zoom Function To Applay it")] // to choose which of the zoom function we want , we only can choose one
    public bool isZoominEvent;
    public bool isZoomOutEvent;
    private BoxCollider2D ObjectBoxCollider;
    [Header("Timers")] // To control the Timers individually
    [Range(0f, 5f)]
    public float TransitionTimer; // (1)the transition timer from the Character Camera to the Event Camera
    [Range(0f, 10f)]
    public float StartEventAfterTimer; // (2) After doing the Transition timer we make the event Start
    [Range(0f, 10f)]
    public float EndEventAfterTimer; // (3) the Amount of timer to finish the Event
    [Header("Object Number To Follow")]
    public int NumberOfObjectToFollow; // to put the number of the object the camera will follow

    void Start()
    {
        

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.tag == "Player") // we activate the triggers of the camera from the event script
            {                             // the enter function all are the same , because we are activating the camera
            EnterEventFunction();
            }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // on the exit it means the player is outside the collider so we deactivate the things we did in the enter
            if (collision.tag == "Player") // also are the same 
            {
            ExitEventFunction();
            }
    }
    public void EnterEventFunction()
    {
        EventNumber = MainCamera.GetComponent<Camera>().CurrentEventNumber = EventNumber;  // set the event number
        MainCamera.GetComponent<Camera>().NumberOfObjectToFollow = NumberOfObjectToFollow;
        WhichZoom();
        TimersFunction();
        MainCamera.GetComponent<Camera>().EventChangeCamera = true; // change camera true
        Debug.Log("Event test script");
        MainCamera.GetComponent<Camera>().StartCoroutine("MyTesting"); // activate the coroutine in the main camera , from there it will check the event number
    }
    public void ExitEventFunction()
    {
        EventNumber = MainCamera.GetComponent<Camera>().CurrentEventNumber = EventNumber;
        MainCamera.GetComponent<Camera>().EventChangeCamera = false;
        Debug.Log(" player is outside1");
        MainCamera.GetComponent<Camera>().StartCoroutine("MyTesting");
        DeActivade2DBoxCollider();
    }
    public void WhichZoom()
    {
        if (isZoominEvent)
        {
            SecCamera.GetComponent<Camera>().ZoomInValueTarget = EventZoomValue;
            SecCamera.GetComponent<Camera>().ZoomInSpeed = Speed;
        }
        if (isZoomOutEvent)
        {
            SecCamera.GetComponent<Camera>().ZoomOutValueTarget = EventZoomValue;
            SecCamera.GetComponent<Camera>().ZoomOutSpeed = Speed;
        }
    }
    public void DeActivade2DBoxCollider()
    {
         ObjectBoxCollider = this.gameObject.GetComponent<BoxCollider2D>();
         ObjectBoxCollider.enabled = !ObjectBoxCollider.enabled;
    }
    public void TimersFunction()
    {
        MainCamera.GetComponent<Camera>().TransitionTimer = TransitionTimer;
        MainCamera.GetComponent<Camera>().StartEventAfterTimer = StartEventAfterTimer;
        MainCamera.GetComponent<Camera>().EndEventAfterTimer = EndEventAfterTimer;
    }
    // Update is called once per frame
    void Update()
    {
       
        
    }
}
