using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{

    public int cameraMode = 5;
    //0:全体（起動時のカメラ位置）
    //1:プレイヤー１と同一
    //2:プレイヤー２と同一
    //3:プレイヤー１の俯瞰
    //4:プレイヤー２の俯瞰 
    //5:プレイヤー１・２の俯瞰

    public Vector3 mainCameraStartPosition;
    public Quaternion mainCameraStartRotation;

    GameObject objPlayer1;
    GameObject objPlayer2;

    // Start is called before the first frame update
    void Start()
    {
        mainCameraStartPosition = this.transform.position;
        mainCameraStartRotation = this.transform.rotation;

        objPlayer1 = GameObject.Find("Player1");

    }

    // Update is called once per frame
    void Update()
    {
        //メインカメラの設定位置で固定。（ステージのサイズに合わせて設定するとよい）
        if (cameraMode == 0)
        {
            this.transform.position = mainCameraStartPosition;
            this.transform.rotation = mainCameraStartRotation;
        }

        //プレイヤー１と同じカメラ
        if (cameraMode == 1)
        {
            this.transform.position = GameObject.Find("PlayerCamera1").transform.position;
            this.transform.rotation = GameObject.Find("PlayerCamera1").transform.rotation;
        }

        //プレイヤー２と同じカメラ
        if (cameraMode == 2)
        {
            this.transform.position = GameObject.Find("PlayerCamera2").transform.position;
            this.transform.rotation = GameObject.Find("PlayerCamera2").transform.rotation;
        }

        //プレイヤー１を中心に俯瞰
        //GameObject.Find("Player1").transform.positionを使用すると画面がチカチカする謎の現象が起きたので、
        //PlayerのスクリプトのUpdate処理でPublic変数にPositionをセットし、そこから取得するようにしたら、なんとなく動いた。
        if (cameraMode == 3)
        {
            PlayerScript Script = GameObject.Find("Player1").gameObject.GetComponent<PlayerScript>();
            this.transform.position = Script.myPosiotion + new Vector3(0, 5f, -5f);
            this.transform.rotation = Quaternion.Euler(45, 0, 0);
        }

        //プレイヤー２を中心に俯瞰
        if (cameraMode == 4)
        {
            PlayerScript Script = GameObject.Find("Player2").gameObject.GetComponent<PlayerScript>();
            this.transform.position = Script.myPosiotion + new Vector3(0, 5f, -5f);
            this.transform.rotation = Quaternion.Euler(45, 0, 0);
        }

        //プレイヤー１と２の中心を基準に俯瞰・・・プレイヤー間の距離に応じてカメラを引く
        if (cameraMode == 5)
        {
            PlayerScript Script1 = GameObject.Find("Player1").gameObject.GetComponent<PlayerScript>();
            PlayerScript Script2 = GameObject.Find("Player2").gameObject.GetComponent<PlayerScript>();
            float dist = Vector3.Distance(Script1.myPosiotion, Script2.myPosiotion);
            this.transform.position = (Script1.myPosiotion + Script2.myPosiotion) / 2 + new Vector3(0, Mathf.Sqrt(dist) * 3.0f, -Mathf.Sqrt(dist) * 3.0f);
            this.transform.rotation = Quaternion.Euler(45, 0, 0);
        }


        if (Input.GetKeyDown("0")) { cameraMode = 0; }
        if (Input.GetKeyDown("1")) { cameraMode = 1; }
        if (Input.GetKeyDown("2")) { cameraMode = 2; }
        if (Input.GetKeyDown("3")) { cameraMode = 3; }
        if (Input.GetKeyDown("4")) { cameraMode = 4; }
        if (Input.GetKeyDown("5")) { cameraMode = 5; }


    }


}
