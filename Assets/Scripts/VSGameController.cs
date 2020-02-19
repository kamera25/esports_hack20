using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VSGameController : MonoBehaviour
{
    public float PLAYTIME {get; private set;} = 60;//１ラウンドのプレイ時間
    public int roundNum {get; private set;} = 1;//ラウンド数
    public float nowPlayTime {get; private set;}
    public PlayerStatics player1;
    public PlayerStatics player2;

    public enum STATE
    {
        NONE = 0,
        ROUND,
        BATTLE,
        RESULT
    }
    public STATE state {get; private set;} = STATE.ROUND;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("StartRound");
    }

    // Update is called once per frame
    void Update()
    {

        if( state != STATE.BATTLE)
        {
            return;
        }

        // バトル中に行う処理
        nowPlayTime = Mathf.Clamp( nowPlayTime - Time.deltaTime, 0F, PLAYTIME);
        if( IsTimerOver() || player1.Hp <= 0 || player2.Hp <= 0)
        {
            StartCoroutine("EndRound");
        }
    }

    IEnumerator StartRound()
    {

        // プレイヤーに行う処理
        player1.ResetAllStatus();
        player2.ResetAllStatus();

        state = STATE.ROUND;
        yield return new WaitForSeconds(2F);
        
        state = STATE.BATTLE;
        roundNum++;
        ResetTimer();
    }

    IEnumerator EndRound()
    {
        // スターによる勝敗確認
        TransitionScene();

        state = STATE.RESULT;
        yield return new WaitForSeconds(2.5F);
        StartCoroutine("StartRound");
    }

    void TransitionScene()
    {
        if(roundNum <= 3)
        {
            return;
        }

        // 3ラウンド目なら以下の処理を行う。
        if(player1.star > player2.star)
        {
            SceneManager.LoadScene("ResultScene_1");
        }
        else
        {
            SceneManager.LoadScene("ResultScene_2");
        }
    }

    public bool IsTimerOver()
    {
        return nowPlayTime <= 0F;
    }

    public bool IsPlayer1()
    {
        return player1.Hp > player2.Hp;
    }

    void ResetTimer()
    {
        nowPlayTime = PLAYTIME;
    }
}
