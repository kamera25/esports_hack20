using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    float elapsedTime;
    public string createrName { get; set; }

    AudioSource SE_DAMAGE;


    // Start is called before the first frame update
    void Start()
    {
        SE_DAMAGE = GameObject.Find("SE_DAMAGE").GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

        //玉は発射されて一定時間経過後、自然消滅する 
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 3)
        {
            Destroy(this.gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        //プレイヤーにぶつけた時の処理
        if (collision.gameObject.tag == "Player")
        {
            PlayerScript Script = collision.gameObject.GetComponent<PlayerScript>();
            //対象のプレイヤーが無敵中でなかった場合は、ダメージを与える処理
            if (Script.isInvincible == false) {

                Script.incHitCount();//当たった回数のカウントアップ
                Script.incHp(-200);//ダメージ値
                Script.isInvincible = true;//無敵フラグを立てるにする
                Script.invincibleStartTime = Time.time;//無敵開始時刻を設定する

                Destroy(this.gameObject);//プレイヤーに当てたら、玉自身は消える

                //Script.DecBallCount();
                SE_DAMAGE.Play();
            }



        }

    }

    private void OnDestroy()
    {
        //玉が消えるとき（時間で消える／プレイヤーに当たって消える）は、各プレイヤーが発している玉のカウントを減らす
        if (createrName == "Player1")
        {
            PlayerScript Script1 = GameObject.Find("Player1").gameObject.GetComponent<PlayerScript>();
            Script1.DecBallCount();
        }
        else
        {
            PlayerScript Script2 = GameObject.Find("Player2").gameObject.GetComponent<PlayerScript>();
            Script2.DecBallCount();
        }


    }


}
