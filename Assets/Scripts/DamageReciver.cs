using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReciver : MonoBehaviour
{
    PlayerStatics statics;

    void Start()
    {
        statics = this.GetComponent<PlayerStatics>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject _hitObject = collision.gameObject;

        if ( _hitObject.CompareTag("ChocoBall"))
        {
            PlayerStatics.Player _p =  _hitObject.GetComponent<ChocoStatics>().ownPlayer;
            if( _p != statics.playerTag)
            {
                HitProcess( _hitObject);
            }
        }
    }

    //当たった時の処理
    void HitProcess( GameObject _col)
    {
        statics.AddHitCount();
        statics.AddHP(-200);
        statics.ResetInvincibleTime();//無敵開始時刻を設定する

        Destroy( _col);//プレイヤーに当てたら、玉自身は消える
    }
}
