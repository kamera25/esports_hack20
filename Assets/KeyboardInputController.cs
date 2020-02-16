using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputController : MonoBehaviour, IVirtualController
{
    public float GetMoveVertical()
    {
        int _u = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        int _d = Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
        return _u + _d;
    }
    public float GetMoveHorizontal()
    {
        int _r = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        int _l = Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
        return _l + _r;
    }
    public float GetCameraVertical()
    {
        int _u = Input.GetKey(KeyCode.W) ? 1 : 0;
        int _d = Input.GetKey(KeyCode.S) ? -1 : 0;
        return _u + _d;
    }
    public float GetCameraHorizontal()
    {
        int _r = Input.GetKey(KeyCode.D) ? 1 : 0;
        int _l = Input.GetKey(KeyCode.A) ? -1 : 0;
        return _l + _r;
    }
    public bool GetCameraModeButton()
    {
        return Input.GetKeyDown( KeyCode.Z);
    }
    public bool GetJumpButton()
    {
        return Input.GetKeyDown( KeyCode.Space);
    }
    public bool GetFireButton()
    {
        return Input.GetKeyDown( KeyCode.Return);
    }
}
