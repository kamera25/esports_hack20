using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCamera : MonoBehaviour
{
    float camDist = 10F;
    float camHeight = 5F;
    float camDeg = 0F;
    const float CAMERA_MOVE_SPEED = 0.1F;

    public PlayerStatics player;
    public GameObject padController;
    IVirtualController iController;

    int targetLayerMask = 1 << 9;

    void Start()
    {
        iController = padController.GetComponent<IVirtualController>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        VerticalProc();
        SendDirectionForPlayer();
    }

    void MoveCamera()
    {
        //=================================================================
        //カメラ移動処理
        //=================================================================
        Transform _playerTrf = player.transform;

        camDeg = Mathf.Repeat( iController.GetCameraHorizontal() + camDeg, 360F);
        Vector3 _vec = new Vector3( 0F, camDeg, 0F);

        this.transform.eulerAngles = _vec;
        this.transform.position = _playerTrf.position 
                                + this.transform.forward * camDist // 後ろ
                                + this.transform.up * camHeight; // 高さ
        this.transform.LookAt( _playerTrf, Vector3.up);

        //=================================================================
        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        //=================================================================
        RaycastHit hit;
        if (Physics.Linecast( _playerTrf.position, this.transform.position, out hit, targetLayerMask))
        {
            this.transform.position =  hit.point;
        }
    }

    void VerticalProc()
    {
        //=================================================================
        //カメラ移動処理 上下　（L1を押しながらだと、寄せ・引き
        //=================================================================
        float _leftVertical = iController.GetCameraVertical();
        if ( iController.GetCameraModeButton())//L1が押されているかどうか
        {
            camHeight = Mathf.Clamp( camHeight + _leftVertical * CAMERA_MOVE_SPEED , 1F, 8F);
        }
        else
        {
            camDist = Mathf.Clamp( camDist + _leftVertical * CAMERA_MOVE_SPEED, 5F, 15F);
        }
    }

    // カメラ情報をプレイヤーに伝える
    void SendDirectionForPlayer()
    {
        player.myCamDirection = this.transform.rotation;
    }
}
