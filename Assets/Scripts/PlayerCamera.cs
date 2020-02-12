using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCamera : MonoBehaviour
{
    float camDist = 10F;
    float camHeight = 5F;
    float camDeg = 0F;

    public PlayerStatics player;

    string inputNameL1 = "L1_";
    string inputNameHorizontalCamera = "LeftAnalogStick_X_";
    string inputNameVerticalCamera = "LeftAnalogStick_Y_";

    // Start is called before the first frame update
    void Start()
    {
        GetKeyName();
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

        camDeg = Mathf.Clamp( Input.GetAxis(inputNameHorizontalCamera) + camDeg, 0F, 360F);
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
        if (Physics.Linecast( _playerTrf.position, this.transform.position, out hit))
        {
            this.transform.position =  hit.point;
        }
    }

    void VerticalProc()
    {
        //=================================================================
        //カメラ移動処理 上下　（L1を押しながらだと、寄せ・引き
        //=================================================================
        float _leftVertical = Input.GetAxis(inputNameVerticalCamera);
        if ( Input.GetButtonDown(inputNameL1))//L1が押されているかどうか
        {
            camDist = Mathf.Clamp( camDist , 2F, 10F);
        }
        else
        {
            camHeight = Mathf.Clamp( camHeight , -5F, 5F);
        }
    }

    // カメラ情報をプレイヤーに伝える
    void SendDirectionForPlayer()
    {
        Vector3 _vec = this.transform.eulerAngles;
        _vec.y = 0F;
        player.direction = _vec.normalized;
    }

    // 入力名を取得
    void GetKeyName()
    {
        string _s = Convert.ToString( (int)player.playerTag);
        inputNameL1 += _s;
        inputNameHorizontalCamera += _s;
        inputNameVerticalCamera += _s;
    }
}
