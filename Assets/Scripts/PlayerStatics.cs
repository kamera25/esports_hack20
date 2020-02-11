using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatics : MonoBehaviour
{
    public enum Player
    {
        one,
        two
    }

    public Player playerTag = Player.one;

    public int Hp = 1000;
    int hitCount = 0;
    int ballCount = 0;
    public int bulletRemain = 3;
    public bool isInvincible = false;
    public float invincibleStartTime = 0;
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
}
