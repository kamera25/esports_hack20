using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{

	Slider Canvas1_HpBar1;
    Slider Canvas1_HpBar2;
    Slider Canvas2_HpBar1;
    Slider Canvas2_HpBar2;
    Slider Canvas8_HpBar1;
    Slider Canvas8_HpBar2;
    Text Canvas1_TimerText;
    Text Canvas2_TimerText;
    Text Canvas8_TimerText;
    Text Canvas1_DebugText;
    Text Canvas2_DebugText;
    Text Canvas8_DebugText;
    

    public string TimeLeft;
    public float SceneStartTime;//各シーンがスタートする毎に設定して使用する。

    public bool PlayerControlApproval = false;//プレイヤーの操作を許可する、しない
    public int GameScene = 0;//独自仕様
    //0:title&Setting
    //1:Ready...
    //2:Playing
    //3:RoundResult
    //4:TotalResult

    int SetCount1 = 0;
    int SetCount2 = 0;
    int NowRound = 1;
    int SetPointPlayer = 0;
    int WinPlayer = 0;


    float PlayTime = 60;//１ラウンドのプレイ時間


    PlayerScript Script1;
    PlayerScript Script2;


    AudioSource BGM_TITLE;
    AudioSource BGM_BATTLE;
    AudioSource BGM_SETCOUNT;
    AudioSource BGM_RESULT;

    // Start is called before the first frame update
    void Start()
    {
        // スライダーを取得する
        Canvas1_HpBar1 = GameObject.Find("Canvas1/HpBar1").GetComponent<Slider>();
        Canvas1_HpBar2 = GameObject.Find("Canvas1/HpBar2").GetComponent<Slider>();
        Canvas2_HpBar1 = GameObject.Find("Canvas2/HpBar1").GetComponent<Slider>();
        Canvas2_HpBar2 = GameObject.Find("Canvas2/HpBar2").GetComponent<Slider>();
        Canvas8_HpBar1 = GameObject.Find("Canvas8/HpBar1").GetComponent<Slider>();
        Canvas8_HpBar2 = GameObject.Find("Canvas8/HpBar2").GetComponent<Slider>();
        Canvas1_TimerText = GameObject.Find("Canvas1/TimerText").GetComponent<Text>();
        Canvas2_TimerText = GameObject.Find("Canvas2/TimerText").GetComponent<Text>();
        Canvas8_TimerText = GameObject.Find("Canvas8/TimerText").GetComponent<Text>();

        /*
        //HPバーの初期化 
        Canvas1_HpBar1.value = 1000;
        Canvas1_HpBar2.value = 1000;
        Canvas2_HpBar1.value = 1000;
        Canvas2_HpBar2.value = 1000;
        Canvas8_HpBar1.value = 1000;
        Canvas8_HpBar2.value = 1000;
        */

        Script1 = GameObject.Find("Player1").gameObject.GetComponent<PlayerScript>();
        Script2 = GameObject.Find("Player2").gameObject.GetComponent<PlayerScript>();


        //これではダメ。
        //GameObject.Find("Display 1").GetComponent<Display>().SetRenderingResolution(1024, 576);
        //GameObject.Find("Display 2").GetComponent<Display>().SetRenderingResolution(1024, 576);
        //GameObject.Find("Display 8").GetComponent<Display>().SetRenderingResolution(1024, 576);

        //マルチディスプレイの方法に関する情報
        //http://tsubakit1.hateblo.jp/entry/2015/12/18/235514



        //デバッグ表示用のオブジェクトを取得する処理は、Void Startの最後に記述する。そうしないと、Enabledをオフにしたとき、スクリプトがコケて、この後が処理されないので。
        Canvas1_DebugText = GameObject.Find("Canvas1/DebugText").GetComponent<Text>();
        Canvas2_DebugText = GameObject.Find("Canvas2/DebugText").GetComponent<Text>();
        Canvas8_DebugText = GameObject.Find("Canvas8/DebugText").GetComponent<Text>();

        //プレイヤーの初期位置の設定　この処理が、プレイヤーのスクリプトより先に実行されるようにするため、Project Settingsにおいて、実行順を設定している。
        //※プレイヤースクリプトが先に実行されると、カメラの位置がおかしくなるので
        //バグの元なので、直す予定。
        initPlayer();

        GameScene = 0;
        SceneStartTime = Time.time;
        Script1.ReadyFlg = false;
        Script2.ReadyFlg = false;
        TimeLeft = "";
        SetCount1 = 0;
        SetCount2 = 0;

        BGM_TITLE = GameObject.Find("BGM_TITLE").GetComponent<AudioSource>();
        BGM_BATTLE = GameObject.Find("BGM_BATTLE").GetComponent<AudioSource>();
        BGM_SETCOUNT = GameObject.Find("BGM_SETCOUNT").GetComponent<AudioSource>();
        BGM_RESULT = GameObject.Find("BGM_RESULT").GetComponent<AudioSource>();

        //最初の再生
        BGM_TITLE.Play();
        BGM_BATTLE.Stop();
        BGM_RESULT.Stop();
    }

    //=====================
    //プレイヤーの初期値の設定
    //=====================
    void initPlayer()
    {
        GameObject.Find("Player1").transform.position = new Vector3(-2.0f, 1.5f, 0);
        GameObject.Find("Player2").transform.position = new Vector3( 2.0f, 1.5f, 0);
        GameObject.Find("Player1").transform.rotation = Quaternion.Euler(0, 90, 0);
        GameObject.Find("Player2").transform.rotation = Quaternion.Euler(0, -90, 0);
        Script1.initCamera();
        Script2.initCamera();
        Script1.Reloading = false;
        Script2.Reloading = false;
        Script1.BulletRemain = 3;
        Script2.BulletRemain = 3;
        Script1.ballCount = 0;
        Script2.ballCount = 0;
        Script1.incHp(1000);
        Script2.incHp(1000);

    }

    // Update is called once per frame
    void Update()
    {

        //==================================================================================================
        //タイトル画面の制御
        //==================================================================================================
        //両プレイヤーがSTARTボタンを押すまで待つ。
        //Config機能を追加する予定。
        if (GameScene == 0)
        {
            if (Script1.ReadyFlg == true && Script2.ReadyFlg == true)
            {
                //次のシーンの初期値を設定
                initPlayer();
                NowRound = 1;
                GameScene = 1;
                SceneStartTime = Time.time;
                TimeLeft = PlayTime.ToString();

                BGM_TITLE.Stop();

            }
        }

        //==================================================================================================
        //Round開始画面の制御
        //==================================================================================================
        //Ready...Go!のReady部分。１秒間
        if (GameScene == 1)
        {
            if (Time.time - SceneStartTime > 1)
            {
                GameScene = 2;
                SceneStartTime = Time.time;

                BGM_TITLE.Stop();
                BGM_BATTLE.Play();

            }
        }


        //==================================================================================================
        //プレイ中画面の制御
        //==================================================================================================
        //最初の１秒間は、Goを表示するが、これは後述の画面表示部分で。
        if (GameScene == 2)
        {
            TimeLeft = Mathf.Ceil(PlayTime - (Time.time - SceneStartTime)).ToString();

            //ラウンドが終わる条件に該当したら、結果を設定
            if (TimeLeft == "0" || Script1.getHp() == 0 || Script2.getHp() == 0)
            {
                if (Script1.getHp() == Script2.getHp() )
                {
                    SetPointPlayer = 0;
                }
                if (Script1.getHp() > Script2.getHp() )
                {
                    SetPointPlayer = 1;
                    SetCount1++;
                }
                if (Script1.getHp() < Script2.getHp() )
                {
                    SetPointPlayer = 2;
                    SetCount2++;
                }

                if (SetCount1 < 2 && SetCount2 < 2)
                {
                    GameScene = 3;
                    SceneStartTime = Time.time;
                    Script1.ReadyFlg = false;
                    Script2.ReadyFlg = false;

                    BGM_BATTLE.Stop();
                    BGM_SETCOUNT.Play();
                }
                else {
                    if (SetCount1 == 2)
                    {
                        WinPlayer = 1;
                    }
                    if (SetCount2 == 2)
                    {
                        WinPlayer = 2;
                    }
                    GameScene = 4;
                    SceneStartTime = Time.time;
                    Script1.ReadyFlg = false;
                    Script2.ReadyFlg = false;
                    BGM_BATTLE.Stop();
                    BGM_RESULT.Play();
                }

            }
        }

        //==================================ß================================================================
        //各ラウンドの結果画面の制御　（どちらかのプレイヤーがセットカウント２を取得した場合は、ここには来ない
        //==================================================================================================
        //両プレイヤーがSTARTボタンを押すまで待つ。
        //Config機能を追加する予定。
        if (GameScene == 3)
        {
            if (Script1.ReadyFlg == true && Script2.ReadyFlg == true)
            {

                if (SetPointPlayer == 0)
                {
                    //引き分けの場合はラウンドを進めず、もう一度
                    initPlayer();
                    GameScene = 1;
                    SceneStartTime = Time.time;
                    TimeLeft = PlayTime.ToString();
                    BGM_SETCOUNT.Stop();
                    //BGM_BATTLE.Play();

                }
                else
                {
                    //引き分けではない場合は、ラウンドを進める
                    NowRound++;
                    initPlayer();
                    GameScene = 1;
                    SceneStartTime = Time.time;
                    TimeLeft = PlayTime.ToString();
                    BGM_SETCOUNT.Stop();
                    //BGM_BATTLE.Play();
                }


            }
        }


        //==================================================================================================
        //最終結果画面の制御
        //==================================================================================================
        //両プレイヤーがSTARTボタンを押すまで待つ。
        //Config機能を追加する予定。
        if (GameScene == 4)
        {
            if (Script1.ReadyFlg == true && Script2.ReadyFlg == true)
            {
                //タイトルに戻る
                GameScene = 0;
                initPlayer();
                SetCount1 = 0;
                SetCount2 = 0;
                TimeLeft = "";
                Script1.ReadyFlg = false;
                Script2.ReadyFlg = false;

                BGM_RESULT.Stop();
                BGM_TITLE.Play();

            }


        }

        //UI表示＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊＊
        //このへんの記述は冗長的だけど、とりあえずこれで。
        //GetComponentを失敗して、ハンドリングしない場合は、それ以降が処理されないので、表示関係は必ず最後に記述したほうがよさそう。

        //==================================================================================================
        //残り時間表示
        //==================================================================================================
        Canvas1_TimerText.text = TimeLeft.ToString();
        Canvas2_TimerText.text = TimeLeft.ToString();
        Canvas8_TimerText.text = TimeLeft.ToString();

        //==================================================================================================
        //HP BAR
        //==================================================================================================
        Canvas1_HpBar1.value = Script1.getHp();
        Canvas1_HpBar2.value = Script2.getHp();
        Canvas2_HpBar1.value = Script1.getHp();
        Canvas2_HpBar2.value = Script2.getHp();
        Canvas8_HpBar1.value = Script1.getHp();
        Canvas8_HpBar2.value = Script2.getHp();


        //==================================================================================================
        //Bullet/Reloading Image
        //==================================================================================================
        if (Script1.BulletRemain > 2)
        {
            GameObject.Find("Canvas1/BulletImg1-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/BulletImg1-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/BulletImg1-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/BulletImg1-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/BulletImg1-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/BulletImg1-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }
        
        if (Script1.BulletRemain > 1)
        {
            GameObject.Find("Canvas1/BulletImg1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/BulletImg1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/BulletImg1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/BulletImg1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/BulletImg1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/BulletImg1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }
        if (Script1.BulletRemain > 0)
        {
            GameObject.Find("Canvas1/BulletImg1-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/BulletImg1-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/BulletImg1-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/BulletImg1-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/BulletImg1-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/BulletImg1-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }


        if (Script2.BulletRemain > 2)
        {
            GameObject.Find("Canvas1/BulletImg2-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/BulletImg2-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/BulletImg2-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/BulletImg2-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/BulletImg2-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/BulletImg2-1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }
        if (Script2.BulletRemain > 1)
        {
            GameObject.Find("Canvas1/BulletImg2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/BulletImg2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/BulletImg2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/BulletImg2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/BulletImg2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/BulletImg2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }
        if (Script2.BulletRemain > 0)
        {
            GameObject.Find("Canvas1/BulletImg2-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/BulletImg2-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/BulletImg2-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/BulletImg2-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/BulletImg2-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/BulletImg2-3").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }

        if (Script1.Reloading == true)
        {
            GameObject.Find("Canvas1/BulletText1").GetComponent<Text>().text = "Reloading..";
            GameObject.Find("Canvas2/BulletText1").GetComponent<Text>().text = "Reloading..";
            GameObject.Find("Canvas8/BulletText1").GetComponent<Text>().text = "Reloading..";

        }
        else
        {
            GameObject.Find("Canvas1/BulletText1").GetComponent<Text>().text = "";
            GameObject.Find("Canvas2/BulletText1").GetComponent<Text>().text = "";
            GameObject.Find("Canvas8/BulletText1").GetComponent<Text>().text = "";
        }

        if (Script2.Reloading == true)
        {
            GameObject.Find("Canvas1/BulletText2").GetComponent<Text>().text = "Reloading..";
            GameObject.Find("Canvas2/BulletText2").GetComponent<Text>().text = "Reloading..";
            GameObject.Find("Canvas8/BulletText2").GetComponent<Text>().text = "Reloading..";
        }
        else
        {
            GameObject.Find("Canvas1/BulletText2").GetComponent<Text>().text = "";
            GameObject.Find("Canvas2/BulletText2").GetComponent<Text>().text = "";
            GameObject.Find("Canvas8/BulletText2").GetComponent<Text>().text = "";
        }

        //==================================================================================================
        //Title Image
        //==================================================================================================
        if (GameScene == 0)
        {
            GameObject.Find("Canvas1/TitleImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/TitleImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/TitleImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/TitleImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/TitleImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/TitleImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }

        //==================================================================================================
        //Control Image
        //==================================================================================================
        if (GameScene == 0)
        {
            GameObject.Find("Canvas1/ControlImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/ControlImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/ControlImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            GameObject.Find("Canvas1/ControlImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/ControlImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/ControlImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }

        //==================================================================================================
        //Config Text
        //==================================================================================================
        if (GameScene == 0)
        {
            GameObject.Find("Canvas1/ConfigText").GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
            GameObject.Find("Canvas2/ConfigText").GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
            GameObject.Find("Canvas8/ConfigText").GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            GameObject.Find("Canvas1/ConfigText").GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/ConfigText").GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/ConfigText").GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        }

        //==================================================================================================
        //Ready To Start / Please Wait Image
        //==================================================================================================
        if (GameScene == 0 || GameScene == 3 || GameScene == 4)
        {
            float Blink = Mathf.Sin((Time.time - SceneStartTime) * 7 + Mathf.PI);

            //プレイヤー毎にスタートボタンを押しているかどうかで、表示するイメージを切り替え「
            if (Script1.ReadyFlg == false)
            {
                GameObject.Find("Canvas1/PushStartImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, Blink);
                GameObject.Find("Canvas1/PleaseWaitImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                GameObject.Find("Canvas1/PushStartImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
                GameObject.Find("Canvas1/PleaseWaitImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, Blink);
            }

            if (Script2.ReadyFlg == false)
            {
                GameObject.Find("Canvas2/PushStartImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, Blink);
                GameObject.Find("Canvas2/PleaseWaitImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                GameObject.Find("Canvas2/PushStartImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
                GameObject.Find("Canvas2/PleaseWaitImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, Blink);
            }

            //メイン画面は、常に Please Wait
            GameObject.Find("Canvas8/PushStartImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/PleaseWaitImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, Blink);
        }
        else
        {
            GameObject.Find("Canvas1/PushStartImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/PushStartImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/PushStartImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/PleaseWaitImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/PleaseWaitImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/PleaseWaitImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }


        //==================================================================================================
        //Round... Image
        //==================================================================================================
        if (GameScene == 1 || (GameScene == 2 && (Time.time - SceneStartTime) < 1) )
        {
            if (NowRound == 1)
            {
                GameObject.Find("Canvas1/Round1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/Round1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/Round1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            }
            if (NowRound == 2)
            {
                GameObject.Find("Canvas1/Round2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/Round2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/Round2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            }
            if (NowRound == 3)
            {
                GameObject.Find("Canvas1/Round3Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/Round3Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/Round3Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
        else
        {
            GameObject.Find("Canvas1/Round1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/Round1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/Round1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/Round2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/Round2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/Round2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/Round3Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/Round3Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/Round3Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }

        //==================================================================================================
        //Ready... Image
        //==================================================================================================
        if (GameScene == 1 )
        {
            GameObject.Find("Canvas1/ReadyImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/ReadyImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/ReadyImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/ReadyImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/ReadyImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/ReadyImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }

        //==================================================================================================
        //Go! Image
        //==================================================================================================
        if (GameScene == 2 && (Time.time - SceneStartTime) < 1)
        {
            GameObject.Find("Canvas1/GoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/GoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/GoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/GoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/GoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/GoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }

        //==================================================================================================
        //KO / TimeUp Image
        //==================================================================================================
        if (GameScene == 3 )
        {
            if (TimeLeft != "0")
            {
                GameObject.Find("Canvas1/KoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/KoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/KoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);

            }
            else
            {
                GameObject.Find("Canvas1/TimeUpImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/TimeUpImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/TimeUpImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);

            }
        }
        else
        {
            GameObject.Find("Canvas1/KoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/KoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/KoImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/TimeUpImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/TimeUpImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/TimeUpImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }


        //==================================================================================================
        //GotPoint Image
        //==================================================================================================
        if (GameScene == 3 )
        {
            if (SetPointPlayer == 0)
            {
                GameObject.Find("Canvas1/DrawImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/DrawImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/DrawImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);

            }
            if (SetPointPlayer == 1)
            {
                GameObject.Find("Canvas1/GotPointP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/GotPointP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/GotPointP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);

            }
            if (SetPointPlayer == 2)
            {
                GameObject.Find("Canvas1/GotPointP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/GotPointP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/GotPointP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);

            }
        }
        else
        {
            GameObject.Find("Canvas1/DrawImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/DrawImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/DrawImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/GotPointP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/GotPointP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/GotPointP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/GotPointP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/GotPointP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/GotPointP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }

        //==================================================================================================
        //Winner Image
        //==================================================================================================
        if (GameScene == 4)
        {
            if (WinPlayer == 1)
            {
                GameObject.Find("Canvas1/YouWinImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/YouLoseImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/WinnerP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);

            }
            else
            {
                GameObject.Find("Canvas1/YouLoseImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas2/YouWinImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
                GameObject.Find("Canvas8/WinnerP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);

            }
        }
        else
        {
            GameObject.Find("Canvas1/YouWinImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/YouWinImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/YouWinImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/YouLoseImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/YouLoseImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/YouLoseImg").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/WinnerP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/WinnerP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/WinnerP1Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/WinnerP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/WinnerP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/WinnerP2Img").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }

        //==================================================================================================
        //Setcount Image
        //==================================================================================================
        if (SetCount1 > 0)
        {
            GameObject.Find("Canvas1/PointImgP1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/PointImgP1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/PointImgP1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else if (SetCount1 > 1) {
            GameObject.Find("Canvas1/PointImgP1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/PointImgP1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/PointImgP1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/PointImgP1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/PointImgP1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/PointImgP1").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/PointImgP1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/PointImgP1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/PointImgP1-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }
        if (SetCount2 > 0)
        {
            GameObject.Find("Canvas1/PointImgP2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/PointImgP2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/PointImgP2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else if (SetCount2 > 1)
        {
            GameObject.Find("Canvas1/PointImgP2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas2/PointImgP2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
            GameObject.Find("Canvas8/PointImgP2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            GameObject.Find("Canvas1/PointImgP2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/PointImgP2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/PointImgP2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas1/PointImgP2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas2/PointImgP2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
            GameObject.Find("Canvas8/PointImgP2-2").GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 0f);
        }



        //==================================================================================================
        //debugPrint この処理は、Updateの最後に記述する。
        //==================================================================================================
        string txtDebug = "";
        //txtDebug += "hitCount:" + hitCount.ToString() + ",  ";
        //txtDebug += "x:" + objCamera.transform.rotation.eulerAngles.x.ToString() + ",  ";
        //txtDebug += "y:" + objCamera.transform.rotation.eulerAngles.y.ToString() + ",  ";
        //txtDebug += "z:" + objCamera.transform.rotation.eulerAngles.z.ToString() + ",  ";
        //txtDebug += "VerticalCamera:" + Input.GetAxis("VerticalCamera").ToString() + ",  ";
        //txtDebug += "HorizontalCamera:" + Input.GetAxis("HorizontalCamera").ToString() + ",  ";
        //txtDebug += "inputNameJump:" + inputNameJump + ",  ";
        //txtDebug += "Input.GetAxis(inputNameJump):" + Input.GetAxis(inputNameJump).ToString() + ",  ";
        //txtDebug += "L1:" + Input.GetAxis(inputNameL1).ToString() + ",  ";


        txtDebug += "P1:" + Script1.getHitCount().ToString() + ",  ";
        txtDebug += "P2:" + Script2.getHitCount().ToString() + ",  ";
        txtDebug += "P1Ball:" + Script1.getBallCount().ToString() + ",  ";
        txtDebug += "P2Ball:" + Script2.getBallCount().ToString() + ",  ";

        txtDebug = "P1:" + Script1.BulletRemain.ToString() + ",  ";
        txtDebug += "P2:" + Script2.BulletRemain.ToString() + ",  ";
        txtDebug += "P1:" + Script1.Reloading.ToString() + ",  ";
        txtDebug += "P2:" + Script2.Reloading.ToString() + ",  ";

        Canvas1_DebugText.text = txtDebug;
        Canvas2_DebugText.text = txtDebug;
        Canvas8_DebugText.text = txtDebug;

        //デバック表示を消す処理。デバッグ表示したいときは、この処理をコメントアウトする。
        Canvas1_DebugText.text = "";
        Canvas2_DebugText.text = "";
        Canvas8_DebugText.text = "";
        GameObject.Find("Canvas1/Debug Back Ground").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        GameObject.Find("Canvas2/Debug Back Ground").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        GameObject.Find("Canvas8/Debug Back Ground").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

    }
}
