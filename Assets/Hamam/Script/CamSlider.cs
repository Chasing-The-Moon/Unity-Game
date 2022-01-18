using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CamSlider : MonoBehaviour
{
    public Camera CharacterCamera;
    public Slider MySlider;
    public float sliderMaximmum ;
    public float ResetOV;

    void Start()
    {
        sliderMaximmum = CharacterCamera.IdleZoomOutOriginValue; // zoom in 0.42 , will change depends on the slider value , when the player change it ,when the player will move will have this value
        ResetOV = CharacterCamera.IdleZoomOutOriginValue; //  // not changable value
        MySlider.minValue = 0;
        MySlider.maxValue = 2;
        MySlider.value = CharacterCamera.VarCinemachineFramingTransposer.m_GroupFramingSize; // to make the slider value at the begenning be the same of the camera
    }
    public void ResetButton()
    {
        CharacterCamera.VarCinemachineFramingTransposer.m_GroupFramingSize = ResetOV;
        MySlider.value = ResetOV;
        CharacterCamera.IdleZoomOutOriginValue = ResetOV;

    }
    public void WhenWeChangeSliderValue()
    {

        CharacterCamera.VarCinemachineFramingTransposer.m_GroupFramingSize = MySlider.value;
        CharacterCamera.IdleZoomOutOriginValue = MySlider.value;
    }
    
   
    void Update()
    {
        
    }
}
