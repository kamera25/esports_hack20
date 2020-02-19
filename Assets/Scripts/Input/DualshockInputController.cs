using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DualshockInputController : MonoBehaviour, IVirtualController
{
    public PlayerStatics.Player player = PlayerStatics.Player.one;

    string inputNameVertical = "RightAnalogStick_Y_";
    string inputNameHorizontal = "RightAnalogStick_X_";
    string inputNameJump = "Jump_";
    string inputNameFire1 = "Fire_";    
    string inputNameL1 = "L1_";
    string inputNameHorizontalCamera = "LeftAnalogStick_X_";
    string inputNameVerticalCamera = "LeftAnalogStick_Y_";

    // Start is called before the first frame update
    void Start()
    {
        GetKeyName();
    }

    public float GetMoveVertical()
    {
        return Input.GetAxis(inputNameVertical);
    }
    public float GetMoveHorizontal()
    {
        return Input.GetAxis(inputNameHorizontal);
    }
    public float GetCameraVertical()
    {
        return Input.GetAxis(inputNameVerticalCamera);
    }
    public float GetCameraHorizontal()
    {
        return Input.GetAxis(inputNameHorizontalCamera);
    }
    public bool GetCameraModeButton()
    {
        return Input.GetButton(inputNameL1);
    }
    public bool GetJumpButton()
    {
        return Input.GetButtonDown(inputNameJump);
    }
    public bool GetFireButton()
    {
        return Input.GetButtonDown(inputNameFire1);
    }
    
    void GetKeyName()
    {
        string _s = Convert.ToString( (int) player);
        inputNameL1 += _s;
        inputNameHorizontalCamera += _s;
        inputNameVerticalCamera += _s;
        inputNameFire1 += _s;
        inputNameVertical += _s;
        inputNameHorizontal += _s;
        inputNameJump += _s;
    }
}
