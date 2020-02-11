using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraProc();
    }

    void CameraProc()
    {
        //=================================================================
        //カメラ移動処理 左右
        //=================================================================
        distFlg = Input.GetAxis(inputNameHorizontalCamera);
        if (distFlg != 0)
        {
            distFlg *= -1; //俺セッティング
            //カメラリンクオブジェクトを中心にカメラを回す
            objCamera.transform.RotateAround(objPlayer_CameraLink.transform.position, Vector3.up, cameraRotateSpeed * Time.deltaTime * distFlg);
            //カメラリンクオブジェクトも同じだけその場で回す
            objPlayer_CameraLink.transform.Rotate(0, cameraRotateSpeed * Time.deltaTime * distFlg, 0);
        }



        //=================================================================
        //カメラ移動処理 上下　（L1を押しながらだと、寄せ・引き
        //=================================================================
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
                cameraDist = Mathf.Clamp( cameraDist, 2F, 15F);
            }
        }


        //=================================================================
        // ▼▼▼カメラの難しい処理▼▼▼
        //=================================================================
        var cameraForward = Vector3.Scale(objCamera.transform.forward, new Vector3(1, 0, 1)).normalized;  //  カメラが追従するための動作
        Vector3 direction = cameraForward * Input.GetAxis(inputNameVertical) + objCamera.transform.right * Input.GetAxis(inputNameHorizontal);  //  テンキーや3Dスティックの入力（GetAxis）があるとdirectionに値を返す
                                                                                                                                                //objCamera.transform.LookAt(this.transform.position);

        //=================================================================
        //カメラリンクオブジェクトを、カメラから向かって向こうに向ける　　（これをやらないと、カメラを上下に動かすときの回転軸がずれるため）
        //=================================================================
        Quaternion tmpQuaternion;
        //Rigitbodyを指定しないので、スクリプトでY軸固定の処理を入れている
        tmpQuaternion = Quaternion.LookRotation(objPlayer_CameraLink.transform.position - objCamera.transform.position, Vector3.up);
        tmpQuaternion.z = 0;
        tmpQuaternion.x = 0;
        objPlayer_CameraLink.transform.rotation = Quaternion.Lerp(objPlayer_CameraLink.transform.rotation, tmpQuaternion, 1f);

        //=================================================================
        //カメラの追随 --- カメラの現在の向きをtmpQuaternionにバックアップし、カメラリンクオブジェクトの位置に移動させ（向きがカメラリンクオブジェクトと同じになる）、向きを戻して、カメラ距離まで引く
        //=================================================================
        tmpQuaternion = objCamera.transform.rotation;
        objCamera.transform.position = this.transform.position;
        objCamera.transform.rotation = tmpQuaternion;
        objCamera.transform.position -= objCamera.transform.forward * cameraDist;


        //=================================================================
        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        //=================================================================
        RaycastHit hit;
        if (Physics.Linecast(this.transform.position, objCamera.transform.position, out hit))
        {
            objCamera.transform.position =  hit.point;
        }

    }
}
