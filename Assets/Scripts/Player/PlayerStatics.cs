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

    int maxHP = 1000;
    public int Hp = 1000;
    public Quaternion myCamDirection = Quaternion.identity;
    int hitCount = 0;
    int ballCount = 0;
    public int bulletRemain = 3;
    public int star = 0;
    public float nowReloadTime = 0F;
    public bool isInvincible = false;
    public float nowInvincibleTime = 0F;
    const float FIRST_INVINCIBLE_TIME = 3F;
    const int FIRST_BULLET_NUM = 3;

    void Start()
    {
        ResetHP();
        ResetBullet();
    }

    public void ResetHP()
    {
        Hp = maxHP;
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

    public float GetHPPercent()
    {
        return (float)Hp / (float) maxHP;
    }
}
