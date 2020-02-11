using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    private CharacterController charaCon;       // キャラクターコンポーネント用の変数
    private Animator animCon;  //  アニメーションするための変数
    private Vector3 moveDirection = Vector3.zero;   //  移動する方向とベクトルの変数（最初は初期化しておく）

    //publicにしないと挙動がおかしくなる
    public float idoSpeed = 100.0f;         // 移動速度（Public＝インスペクタで調整可能）
    public float rotateSpeed = 3.0F;     // 向きを変える速度（Public＝インスペクタで調整可能）
    public float kaitenSpeed = 1200.0f;   // プレイヤーの回転速度（Public＝インスペクタで調整可能）
    public float gravity = 20.0F;   //重力の強さ（Public＝インスペクタで調整可能）
    public float jumpPower = 10.0F;  //ジャンプのスピード


    private GameObject objPlayer_CameraLink;
    private GameObject objPlayer_invincibleSphere;
    private GameObject objCamera;
    private GameObject objBullet;
    public GameObject BulletPreFab;

    float cameraDist = 5;
    float cameraRotateSpeed = 150;
    float distFlg = 0;
    bool fireFlag = false;
    public int hitCount = 0;
    public int ballCount = 0;
    public int Hp = 1000;
    public bool isInvincible = false;
    public float invincibleStartTime = 0;
    public Vector3 myPosiotion;
    public int BulletRemain = 3;
    public bool Reloading = false;
    public float ReloadStartTime;
    public bool ReadyFlg = false;//Ready To Starでボタンを押したかどうかのフラグ
    public bool StartButtonFlg = false;//Ready To Starでボタンを押したかどうかのフラグの連続押し防止用フラグ

    float ReloadTime = 3.0f;        //玉がなくなった状態で発射ボタンを押すと、一定時間リロード動作を行う。その時間。
    float ReloadSpeedRatio = 0.3f;  //リロード中に移動速度が遅くなる比率。
    float MutekiTime = 1.0f;        //攻撃を受けた済に一定時間無敵となる、その時間。

    //コントローラー入力の名前設定。
    //初期値をプレイヤー１用に設定して、void start において、プレイヤー２の場合は、"_P2" を付加するネーミングルールとした。
    string inputNameVertical = "Vertical";
    string inputNameVerticalCamera = "VerticalCamera";
    string inputNameHorizontal = "Horizontal";
    string inputNameHorizontalCamera = "HorizontalCamera";
    string inputNameJump = "Jump";
    string inputNameL1 = "L1";
    string inputNameFire1 = "Fire1";
    string inputNameStart = "Start";

    Animator animator;
    float stunAnimeTime;

    /*
    //public void:謎の変数
    public void Hit()        // ヒット時のアニメーションイベント（今のところからっぽ。ないとエラーが出る）
    {
    }
    public void Jump()        // ヒット時のアニメーションイベント（今のところからっぽ。ないとエラーが出る）
    {
    }
    */

    AudioSource SE_JUMP;
    AudioSource SE_BALL;
    AudioSource SE_RELOAD;



    //****************************************************************************************************
    // Start is called before the first frame update
    //****************************************************************************************************
    void Start()
    {
        charaCon = GetComponent<CharacterController>(); // キャラクターコントローラーのコンポーネントを参照する
        animCon = GetComponent<Animator>(); // アニメーターのコンポーネントを参照する

        if ( this.name == "Player1")
        {
            objCamera = GameObject.Find("PlayerCamera1");
            objPlayer_CameraLink = GameObject.Find("Player1/Player Camera Link");
            objPlayer_invincibleSphere = GameObject.Find("Player1/invincibleSphere");
            animator = GameObject.Find("LOVEDUCK").GetComponent<Animator>();
        }
        else
        {
            objCamera = GameObject.Find("PlayerCamera2");
            objPlayer_CameraLink = GameObject.Find("Player2/Player Camera Link");
            objPlayer_invincibleSphere = GameObject.Find("Player2/invincibleSphere");
            inputNameVertical += "_P2";
            inputNameVerticalCamera += "_P2";
            inputNameHorizontal += "_P2";
            inputNameHorizontalCamera += "_P2";
            inputNameJump += "_P2";
            inputNameL1 += "_P2";
            inputNameFire1 += "_P2";
            inputNameStart += "_P2";
            animator = GameObject.Find("penguin").GetComponent<Animator>();
        }

        SE_JUMP = GameObject.Find("SE_JUMP").GetComponent<AudioSource>();
        SE_BALL = GameObject.Find("SE_BALL").GetComponent<AudioSource>();
        SE_RELOAD = GameObject.Find("SE_RELOAD").GetComponent<AudioSource>();


    }

    //****************************************************************************************************
    //カメラ初期位置設定
    //****************************************************************************************************
    public void initCamera()
    {
        objCamera.transform.position = this.transform.position;
        objCamera.transform.rotation = this.transform.rotation;
        //objPlayer_CameraLink.transform.position = this.transform.position;
        //objPlayer_CameraLink.transform.rotation = this.transform.rotation;
        objCamera.transform.position = objCamera.transform.forward * cameraDist;
        objCamera.transform.RotateAround(this.transform.position, this.transform.TransformDirection(Vector3.left), -30);

    }



    //==========================================================================================================================
    // Update is called once per frame
    //==========================================================================================================================
    void Update()
    {


        //==========================================================================================================================
        //現在のGameSceneの取得  ※このSceneは独自に決めた通番であり、Unityの機能とは別物。
        //==========================================================================================================================
        int GameScene = GameObject.Find("GameControl").gameObject.GetComponent<GameControl>().GameScene;


        //==========================================================================================================================
        // タイトル画面等で、Ready To Startの入力待ちで、Startボタン（OPTIONSボタン）を押したら、フラグを立てる。
        //==========================================================================================================================
        if (Input.GetAxis(inputNameStart) != 0)
        {
            if (StartButtonFlg == false)
            {
                StartButtonFlg = true;
                if (GameScene == 0 || GameScene == 3 || GameScene == 4)
                {
                    ReadyFlg = true;
                }
            }
        }
        else
        {
            StartButtonFlg = false;
        }

        //==========================================================================================================================
        // 上記に加え、タイトル画面等で、Ready To Startの入力待ちで、EnterKeyを押しても、フラグを立てる。
        //==========================================================================================================================
        if (Input.GetKeyDown(KeyCode.Return)) { ReadyFlg = true; }


        //==========================================================================================================================
        // stunアニメーションがwalkアニメーションに上書きされないようにするためのタイムカウンタ
        //==========================================================================================================================
        stunAnimeTime -= Time.deltaTime;
        if (stunAnimeTime < 0)
        {
            stunAnimeTime = 0;
        }



        //==========================================================================================================================
        // 無敵時間処理
        //==========================================================================================================================
        if (isInvincible == true)
        {
            if(Time.time - invincibleStartTime > MutekiTime)
            {
                isInvincible = false;
            }

        }


        //==========================================================================================================================
        // 無敵オーラ表示処理
        //==========================================================================================================================
        if (isInvincible == true)
        {
            if (Mathf.RoundToInt((Time.time - invincibleStartTime)*15) % 2 == 0 )
            {
                objPlayer_invincibleSphere.transform.position = this.transform.position + new Vector3(0, 0.5f, 0);
            }
            else
            {
                objPlayer_invincibleSphere.transform.position = this.transform.position + new Vector3(0, -10000f, 0);

            }
        }
        else
        {
            objPlayer_invincibleSphere.transform.position = this.transform.position + new Vector3(0, -10000f, 0);
        }


        //==========================================================================================================================
        //カメラ移動処理 左右
        //==========================================================================================================================
        distFlg = Input.GetAxis(inputNameHorizontalCamera);
        if (distFlg != 0)
        {
            distFlg *= -1; //俺セッティング
            //カメラリンクオブジェクトを中心にカメラを回す
            objCamera.transform.RotateAround(objPlayer_CameraLink.transform.position, Vector3.up, cameraRotateSpeed * Time.deltaTime * distFlg);
            //カメラリンクオブジェクトも同じだけその場で回す
            objPlayer_CameraLink.transform.Rotate(0, cameraRotateSpeed * Time.deltaTime * distFlg, 0);
        }



        //==========================================================================================================================
        //カメラ移動処理 上下　（L1を押しながらだと、寄せ・引き
        //==========================================================================================================================
        if (Input.GetAxis(inputNameL1) == 0)//L1が押されているかどうか
        {
            //L1ボタンが押されていない状態で右スティックを上下左右に動かす・・・カメラの向き 上下左右 
            distFlg = Input.GetAxis(inputNameVerticalCamera);
            if (distFlg != 0)
            {
                distFlg *= -1; //俺セッティング
                //カメラリンクオブジェクトを中心にカメラを回す
                objCamera.transform.RotateAround(objPlayer_CameraLink.transform.position, objPlayer_CameraLink.transform.TransformDirection(Vector3.left), cameraRotateSpeed * Time.deltaTime * distFlg);
                if (objCamera.transform.rotation.eulerAngles.x > 80)
                {
                    objCamera.transform.RotateAround(objPlayer_CameraLink.transform.position, objPlayer_CameraLink.transform.TransformDirection(Vector3.left), -cameraRotateSpeed * Time.deltaTime * distFlg);
                }
            }
        }
        else
        {
            //L1ボタンが押されている状態で右スティックを上下に動かす・・・カメラの寄せ／引き 
            //カメラ前後
            distFlg = Input.GetAxis(inputNameVerticalCamera);
            if (distFlg != 0)
            {
                cameraDist -= distFlg * cameraRotateSpeed * Time.deltaTime / 10;
                if (cameraDist < 2) { cameraDist = 2; }
                if (cameraDist > 15) { cameraDist = 15; }
            }
        }


        //==========================================================================================================================
        // ▼▼▼カメラの難しい処理▼▼▼
        //==========================================================================================================================
        var cameraForward = Vector3.Scale(objCamera.transform.forward, new Vector3(1, 0, 1)).normalized;  //  カメラが追従するための動作
        Vector3 direction = cameraForward * Input.GetAxis(inputNameVertical) + objCamera.transform.right * Input.GetAxis(inputNameHorizontal);  //  テンキーや3Dスティックの入力（GetAxis）があるとdirectionに値を返す
                                                                                                                                                //objCamera.transform.LookAt(this.transform.position);


        //==========================================================================================================================
        // ▼▼▼移動処理▼▼▼
        //==========================================================================================================================
        if (GameScene == 0 || GameScene == 2)
        {
            if (Reloading == false)
            {　　
                charaCon.Move(moveDirection * Time.deltaTime);  //CharacterControllerの付いているこのオブジェクトを移動させる処理

            }
            else
            {
                charaCon.Move(moveDirection * Time.deltaTime * ReloadSpeedRatio);  //CharacterControllerの付いているこのオブジェクトを移動させる処理

            }


            if (Input.GetAxis(inputNameVertical) == 0 && Input.GetAxis(inputNameHorizontal) == 0)  //  テンキーや3Dスティックの入力（GetAxis）がゼロの時の動作
            {                         
                if (stunAnimeTime == 0)
                {
                    animator.Play("idle");
                }
            }
            else //  テンキーや3Dスティックの入力（GetAxis）がゼロではない時の動作
            {
                MukiWoKaeru(direction);  //  向きを変える動作の処理を実行する（後述）
                                         //animCon.SetBool("Run", true);  //  Runモーションする
                if (stunAnimeTime == 0)
                {
                    animator.Play("walk");
                }
            }

        }
        else{
            //移動スティックを入力したままGameSceneが遷移すると、ずっと歩き続けるので、この処理を入れて停止する。
            charaCon.Move(moveDirection * 0);  
        }




        //==========================================================================================================================
        // ▼▼▼ジャンプと落下の処理▼▼▼
        //==========================================================================================================================
        if (GameScene == 0 || GameScene == 2)
        {
            if (charaCon.isGrounded)    //CharacterControllerの付いているこのオブジェクトが接地している場合の処理
            {
                //animCon.SetBool("Jump", Input.GetKeyDown("space") || Input.GetButtonDown("Jump"));  //  キーorボタンを押したらジャンプアニメを実行
                moveDirection.y = 0f;  //Y方向への速度をゼロにする
                moveDirection = direction * idoSpeed;  //移動スピードを向いている方向に与える

                if (Input.GetAxis(inputNameJump) != 0.0f) //ジャンプボタンが押されている場合
                                                          //if (Input.GetButtonDown(inputNameJump)) //ジャンプボタンが押されている場合
                {
                    moveDirection.y = jumpPower; //Y方向への速度に「ジャンプパワー」の変数を代入する
                    SE_JUMP.Play();
                }
                else //Spaceキーorジャンプボタンが押されていない場合
                {
                    moveDirection.y -= gravity * Time.deltaTime; //マイナスのY方向（下向き）に重力を与える（これを入れるとなぜかジャンプが安定する…）
                }
            }
            else  //CharacterControllerの付いているこのオブジェクトが接地していない場合の処理
            {

                //animator.Play("jump");

                moveDirection.y -= gravity * Time.deltaTime;  //マイナスのY方向（下向き）に重力を与える
            }
        }


        // ▼▼▼アクション処理▼▼▼
        //animCon.SetBool("Action", Input.GetKey("x") || Input.GetButtonDown("Action1"));  //  キーorボタンを押したらアクションを実行
        //animCon.SetBool("Action2", Input.GetKey("z") || Input.GetButtonDown("Action2"));  //  キーorボタンを押したらアクション2を実行
        //animCon.SetBool("Action3", Input.GetKey("c") || Input.GetButtonDown("Action3"));  //  キーorボタンを押したらアクション3を実行
        //animCon.SetBool("Jump", Input.GetKey("space") || Input.GetButtonDown("Jump"));  //  キーorボタンを押したらジャンプを実行（仮）


        //==========================================================================================================================
        //カメラリンクオブジェクトを、カメラから向かって向こうに向ける　　（これをやらないと、カメラを上下に動かすときの回転軸がずれるため）
        //==========================================================================================================================
        Quaternion tmpQuaternion;
        //Rigitbodyを指定しないので、スクリプトでY軸固定の処理を入れている
        tmpQuaternion = Quaternion.LookRotation(objPlayer_CameraLink.transform.position - objCamera.transform.position, Vector3.up);
        tmpQuaternion.z = 0;
        tmpQuaternion.x = 0;
        objPlayer_CameraLink.transform.rotation = Quaternion.Lerp(objPlayer_CameraLink.transform.rotation, tmpQuaternion, 1f);

        //==========================================================================================================================
        //カメラの追随 --- カメラの現在の向きをtmpQuaternionにバックアップし、カメラリンクオブジェクトの位置に移動させ（向きがカメラリンクオブジェクトと同じになる）、向きを戻して、カメラ距離まで引く
        //==========================================================================================================================
        tmpQuaternion = objCamera.transform.rotation;
        objCamera.transform.position = this.transform.position;
        objCamera.transform.rotation = tmpQuaternion;
        objCamera.transform.position -= objCamera.transform.forward * cameraDist;


        //==========================================================================================================================
        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        //==========================================================================================================================
        RaycastHit hit;
        if (Physics.Linecast(this.transform.position, objCamera.transform.position, out hit))
        {
            objCamera.transform.position =  hit.point;
//            objCamera.transform.position = Vector3.MoveTowards( objCamera.transform.position, hit.point, 10*Time.deltaTime);    
        }



        //==========================================================================================================================
        //チョコボール発射（リロード方式）
        //==========================================================================================================================
        if (GameScene == 0 || GameScene == 2)
        {
            if (Reloading == false)
            {
                if (Input.GetAxis(inputNameFire1) != 0)
                {
                    if (fireFlag != true)
                    {
                        if (BulletRemain > 0)
                        {
                            objBullet = Instantiate(BulletPreFab);
                            BulletScript Script = objBullet.GetComponent<BulletScript>();
                            Script.createrName = this.name;

                            objBullet.transform.position = this.transform.position + this.transform.forward;// + Vector3.forward;
                            Rigidbody rg = objBullet.GetComponent<Rigidbody>();
                            rg.AddForce(this.transform.forward * 10, ForceMode.Impulse);
                            fireFlag = true;
                            ballCount++;
                            BulletRemain--;
                            SE_BALL.Play();

                        }
                        else
                        {
                            Reloading = true;
                            ReloadStartTime = Time.time;
                            SE_RELOAD.Play();
                        }
                    }
                }
                else
                {
                    fireFlag = false;
                }
            }
            else
            {
                if (Time.time - ReloadStartTime > ReloadTime)
                {
                    Reloading = false;
                    BulletRemain = 3;

                }
            }

        }





        /*
        //チョコボール発射（同時発射可能数制限方式）
        if (Input.GetAxis(inputNameFire1) != 0)
        {
            //ballcunt < 3 により、同時に発射できる玉は３発までとし、３発を超えたら、消えるのを待たないといけないデザインとしている
            if (fireFlag != true && ballCount < 3)
            {
                objBullet = Instantiate(BulletPreFab);
                BulletScript Script = objBullet.GetComponent<BulletScript>();
                Script.createrName = this.name;

                objBullet.transform.position = this.transform.position + this.transform.forward;// + Vector3.forward;
                Rigidbody rg = objBullet.GetComponent<Rigidbody>();
                rg.AddForce(this.transform.forward * 10, ForceMode.Impulse);
                fireFlag = true;
                ballCount++;
            }

        }
        else
        {
            fireFlag = false;
        }
        */


        //==========================================================================================================================
        //ここにデバック出力の処理を書いていたが、GameControlに移行済 2020/01/02
        //==========================================================================================================================




        //==========================================================================================================================
        //GameObject.Find("Player1").transform.positionを使用すると画面がチカチカする謎の現象が起きたので、
        //PlayerのスクリプトのUpdate処理でPublic変数にPositionをセットし、そこから取得するようにしたら、なんとなく動いた。
        //==========================================================================================================================
        myPosiotion = this.transform.position;






    }

    

    //**********************************************************************************************************************:
    // ■向きを変える動作の処理
    //**********************************************************************************************************************:
    void MukiWoKaeru(Vector3 mukitaiHoukou)
    {
        Quaternion q = Quaternion.LookRotation(mukitaiHoukou);          // 向きたい方角をQuaternion型に直す
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, kaitenSpeed * Time.deltaTime);   // 向きを q に向けてじわ～っと変化させる.
    }


    //**********************************************************************************************************************:
    //
    //**********************************************************************************************************************:
    public void incHitCount()
    {
        hitCount++;
        animator.Play("stun");
        stunAnimeTime = 2;
    }

    //**********************************************************************************************************************:
    //
    //**********************************************************************************************************************:
    public int getHitCount()
    {
        return hitCount;
    }

    //**********************************************************************************************************************:
    //
    //**********************************************************************************************************************:
    public int getBallCount()
    {
        return ballCount;
    }

    //**********************************************************************************************************************:
    //
    //**********************************************************************************************************************:
    public void DecBallCount()
    {
        ballCount --;

        //念のための処理。バグの温床になるかも。
        if (ballCount < 0)
        {
            ballCount = 0;
        }
    }

    //**********************************************************************************************************************:
    //
    //**********************************************************************************************************************:
    public int getHp()
    {
        return Hp;
    }

    //**********************************************************************************************************************:
    //
    //**********************************************************************************************************************:
    public void incHp(int pValue)
    {
        Hp += pValue;
        if (Hp > 1000) { Hp = 1000; }
        if (Hp < 0) { Hp = 0; }
    }



}
