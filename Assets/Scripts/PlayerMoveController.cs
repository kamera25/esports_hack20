using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMoveController : MonoBehaviour
{
    private CharacterController charaCtrl; 
    private Vector3 moveDirection = Vector3.zero;   //  移動する方向とベクトルの変数（最初は初期化しておく）
    float cameraDist = 5F;
    public float jumpPower = 10.0F;  //ジャンプのスピード

    PlayerStatics stat;

    public float gravity = 20.0F;   //重力の強さ（Public＝インスペクタで調整可能）

    float ReloadSpeedRatio = 0.3f;  //リロード中に移動速度が遅くなる比率。
    public float moveSpeed = 100.0f;         // 移動速度（Public＝インスペクタで調整可能）
    
    public float kaitenSpeed = 1200.0f;   // プレイヤーの回転速度（Public＝インスペクタで調整可能）
    private float stunAnimeTime = 0F;
    public GameObject padController;
    private IVirtualController iController;

    // Start is called before the first frame update
    void Start()
    {
        stat = this.GetComponent<PlayerStatics>();
        charaCtrl = this.GetComponent<CharacterController>();
        iController = padController.GetComponent<IVirtualController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    // プレイヤーの動きに関する処理
    void MovePlayer()
    {
        stunAnimeTime -= Time.deltaTime;

        /*=================================================================
        // ▼▼▼移動処理▼▼▼
        //=================================================================*/
        if ( stat.IsReload() == true)
        {　 // Reload時
            charaCtrl.Move(moveDirection * Time.deltaTime * ReloadSpeedRatio);  
        }
        else
        {
            charaCtrl.Move(moveDirection * Time.deltaTime);  
        }

        if (stunAnimeTime > 0F)
        {
            return;
        }

        if ( useMoveKey())  //  テンキーや3Dスティックの入力（GetAxis）がゼロの時の動作
        {                         
            // animator.Play("idle");
        }
        else //  テンキーや3Dスティックの入力（GetAxis）がゼロではない時の動作
        {
//            MukiWoKaeru(direction);  //  向きを変える動作の処理を実行する（後述）
            //animator.Play("walk");
        }

        //=================================================================
        // ▼▼▼ジャンプと落下の処理▼▼▼
        //=================================================================
        if (charaCtrl.isGrounded)    //CharacterControllerの付いているこのオブジェクトが接地している場合の処理
        {
            // キーの取得
            float _vInput = iController.GetMoveVertical();
            float _hInput = iController.GetMoveHorizontal();

            // 方向の取得
            Vector3 _vn = stat.direction;
            _vn.y = 0F;
            _vn = _vn.normalized;
            moveDirection = new Vector3( _vn.x * _hInput * moveSpeed, 0F, _vn.z * _vInput * moveSpeed);

            if ( iController.GetJumpButton() ) //ジャンプボタンが押されている場合
            {
                moveDirection.y = jumpPower; //Y方向への速度に「ジャンプパワー」の変数を代入する
                //SE_JUMP.Play();
                return;
            }
        }

        //マイナスのY方向（下向き）に重力を与える
        moveDirection.y -= gravity * Time.deltaTime;  
    }

    bool useMoveKey()
    {
        return iController.GetMoveVertical() != 0F || iController.GetCameraHorizontal() != 0F;
    }
}
