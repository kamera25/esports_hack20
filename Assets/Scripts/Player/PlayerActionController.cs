using UnityEngine;
using System;

public class PlayerActionController : MonoBehaviour
{
    private MeshRenderer invincibleSphere;
    public GameObject bulletPrefab;
    private PlayerStatics stat;
    private const float FIX_RELOAD_TIME = 3F;        //玉がなくなった状態で発射ボタンを押すと、一定時間リロード動作を行う。その時間。
    public GameObject padController;
    private IVirtualController iController;


    // Start is called before the first frame update
    void Start()
    {
        stat = this.GetComponent<PlayerStatics>();
        iController = padController.GetComponent<IVirtualController>();
        invincibleSphere = this.transform.Find("invincibleSphere").GetComponent<MeshRenderer>();
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
        stat.nowInvincibleTime -= Time.deltaTime;
        invincibleSphere.enabled = stat.useInvincibleSphere();
    }

    //チョコボール発射（リロード方式）
    void RespawnChoco()
    {
        stat.nowReloadTime -= Time.deltaTime;

        if( stat.IsReload() == true || !iController.GetFireButton() )
        {
            return;
        }

        // チョコが0の時
        if( stat.bulletRemain <= 0)
        {
            stat.nowReloadTime = FIX_RELOAD_TIME;
            stat.ResetBullet();
            return;
        }

        // チョコが1以上の時
        // 発射処理
        Vector3 _pos = this.transform.position + this.transform.forward * 3F + Vector3.up;
        GameObject _objBullet = Instantiate( bulletPrefab, _pos, Quaternion.identity);
        Destroy(_objBullet, 3F);

        // コンポーネントにアタッチ
        ChocoStatics _chocoStat = _objBullet.GetComponent<ChocoStatics>();
        _chocoStat.SetOwnPlayer( stat.playerTag);

        // 力を加える
        Rigidbody rg = _objBullet.GetComponent<Rigidbody>();
        rg.AddForce(this.transform.forward * 20F, ForceMode.Impulse);
        stat.bulletRemain--;
        //SE_BALL.Play();

    }
}
