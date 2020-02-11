using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MeshRenderer invincibleSphere;
    public GameObject bulletPrefab;
    private CharacterController charaCtrl; 
    private Vector3 moveDirection = Vector3.zero;   //  移動する方向とベクトルの変数（最初は初期化しておく）
    float cameraDist = 5F;
    public float jumpPower = 10.0F;  //ジャンプのスピード

    PlayerStatics stat;

    public float gravity = 20.0F;   //重力の強さ（Public＝インスペクタで調整可能）

    private const float FIX_RELOAD_TIME = 3.0f;        //玉がなくなった状態で発射ボタンを押すと、一定時間リロード動作を行う。その時間。
    private float nowReloadTime = 0F;
    private float nowInvincibleTime = 0F;
    float ReloadSpeedRatio = 0.3f;  //リロード中に移動速度が遅くなる比率。
    public float moveSpeed = 100.0f;         // 移動速度（Public＝インスペクタで調整可能）
    private float stunAnimeTime = 0F;


    // Start is called before the first frame update
    void Start()
    {
        stat = this.GetComponent<PlayerStatics>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        InvincibleProc();
        RespawnChoco();
    }

    // プレイヤーの動きに関する処理
    void MovePlayer()
    {
        stunAnimeTime -= Time.deltaTime;

        /*=================================================================
        // ▼▼▼移動処理▼▼▼
        //=================================================================*/
        if ( IsReload() == true)
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
            MukiWoKaeru(direction);  //  向きを変える動作の処理を実行する（後述）
            //animator.Play("walk");
        }

        //=================================================================
        // ▼▼▼ジャンプと落下の処理▼▼▼
        //=================================================================
        if (charaCtrl.isGrounded)    //CharacterControllerの付いているこのオブジェクトが接地している場合の処理
        {
            moveDirection.y = 0f;  //Y方向への速度をゼロにする
            moveDirection = direction * moveSpeed;  //移動スピードを向いている方向に与える

            if ( Input.GetAxis(inputNameJump) != 0.0f) //ジャンプボタンが押されている場合
            {
                moveDirection.y = jumpPower; //Y方向への速度に「ジャンプパワー」の変数を代入する
                //SE_JUMP.Play();
                return;
            }
        }

        //マイナスのY方向（下向き）に重力を与える
        moveDirection.y -= gravity * Time.deltaTime;  
    }

        
    //**********************************************************************************************************************:
    // ■向きを変える動作の処理
    //**********************************************************************************************************************:
    void MukiWoKaeru(Vector3 mukitaiHoukou)
    {
        Quaternion q = Quaternion.LookRotation(mukitaiHoukou);          // 向きたい方角をQuaternion型に直す
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, kaitenSpeed * Time.deltaTime);   // 向きを q に向けてじわ～っと変化させる.
    }

    // 無敵時間処理
    void InvincibleProc()
    {
        nowInvincibleTime -= Time.deltaTime;

        if ( stat.isInvincible == true)
        {
            // Sphereを表示
            invincibleSphere.enabled = true;

            if( nowInvincibleTime > 0F)
            {
                stat.isInvincible = false;
            }
        }
        else
        {
            invincibleSphere.enabled = false;
        }
    }

    //チョコボール発射（リロード方式）
    void RespawnChoco()
    {
        nowReloadTime -= Time.deltaTime;

        if( IsReload() == true && Input.GetAxis(inputNameFire1) == false)
        {
            return;
        }

        if( stat.bulletRemain >= 0F)
        {
            
                // 発射処理
                Vector3 _pos = this.transform.position + this.transform.forward;// + Vector3.forward;
                GameObject _objBullet = Instantiate( bulletPrefab, _pos, Quaternion.identity);
                ChocoStatics _chocoStat = _objBullet.GetComponent<ChocoStatics>();
                _chocoStat.SetOwnPlayer( stat.playerTag);

                // 力を加える
                Rigidbody rg = _objBullet.GetComponent<Rigidbody>();
                rg.AddForce(this.transform.forward * 10, ForceMode.Impulse);
                stat.bulletRemain--;
                //SE_BALL.Play();
        }
        else
        {
            nowReloadTime = FIX_RELOAD_TIME;
            stat.ResetBullet();
        }
    }

    bool IsReload()
    {
        return nowReloadTime > 0F;
    }

    bool useMoveKey()
    {
        return Input.GetAxis(inputNameVertical) != 0F || Input.GetAxis(inputNameHorizontal) != 0F;
    }
}
