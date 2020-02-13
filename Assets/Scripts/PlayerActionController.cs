using UnityEngine;
using System;

public class PlayerActionController : MonoBehaviour
{
    private MeshRenderer invincibleSphere;
    public GameObject bulletPrefab;
    PlayerStatics stat;
    private float nowInvincibleTime = 0F;
    private const float FIX_RELOAD_TIME = 3.0f;        //玉がなくなった状態で発射ボタンを押すと、一定時間リロード動作を行う。その時間。
    string inputNameFire1 = "Fire_";


    // Start is called before the first frame update
    void Start()
    {
        stat = this.GetComponent<PlayerStatics>();
        invincibleSphere = this.transform.Find("invincibleSphere").GetComponent<MeshRenderer>();
        GetKeyName();
    }

    // Update is called once per frame
    void Update()
    {
        InvincibleProc();
        RespawnChoco();
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
        stat.nowReloadTime -= Time.deltaTime;

        if( stat.IsReload() == true || !Input.GetButtonDown(inputNameFire1) )
        {
            return;
        }

        if( stat.bulletRemain >= 0F)
        {
                // 発射処理
                Vector3 _pos = this.transform.position + this.transform.forward;
                GameObject _objBullet = Instantiate( bulletPrefab, _pos, Quaternion.identity);
                Destroy(_objBullet, 3F);

                // コンポーネントにアタッチ
                ChocoStatics _chocoStat = _objBullet.GetComponent<ChocoStatics>();
                _chocoStat.SetOwnPlayer( stat.playerTag);

                // 力を加える
                Rigidbody rg = _objBullet.GetComponent<Rigidbody>();
                rg.AddForce(this.transform.forward * 10F, ForceMode.Impulse);
                stat.bulletRemain--;
                //SE_BALL.Play();
        }
        else
        {
            stat.nowReloadTime = FIX_RELOAD_TIME;
            stat.ResetBullet();
        }
    }

    void GetKeyName()
    {
        string _s = Convert.ToString( (int)stat.playerTag);
        inputNameFire1 += _s;
    }
}
