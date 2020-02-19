using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVirtualController 
{
    float GetMoveVertical();
    float GetMoveHorizontal();
    float GetCameraVertical();
    float GetCameraHorizontal();
    bool GetCameraModeButton();
    bool GetJumpButton();
    bool GetFireButton();
}
