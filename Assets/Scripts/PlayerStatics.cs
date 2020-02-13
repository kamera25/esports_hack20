using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatics : MonoBehaviour
{
    public enum Player
    {
        one = 1,
        two = 2
    }

    public Player playerTag = Player.one;

    public int Hp = 1000;
    public Vector3 direction = Vector3.zero;
    int hitCount = 0;
    int ballCount = 0;
    public int bulletRemain = 3;
    public float nowReloadTime = 0F;
    public bool isInvincible = false;
    public float nowInvincibleTime = 0F;
    const float FIRST_INVINCIBLE_TIME = 3F;
    const int FIRST_BULLET_NUM = 3;

    void Start()
    {
        ResetBullet();
    }

    public void AddHitCount()
    {
        hitCount++;
    }

    public void AddHP( int _hp)
    {
        Hp += _hp;
    }

    public void ResetBullet()
    {
        bulletRemain = FIRST_BULLET_NUM;
    }

    public void ResetInvincibleTime()
    {
        nowInvincibleTime = FIRST_INVINCIBLE_TIME;
    }

    public bool IsReload()
    {
        return nowReloadTime > 0F;
    }
}
