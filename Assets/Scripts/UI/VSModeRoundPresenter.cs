using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VSModeRoundPresenter : MonoBehaviour
{
    public GameObject roundUI;
    public GameObject battleUI;
    public GameObject resultUI;
    public GameObject drawUI;
    public Sprite Round1IMG;
    public Sprite Round2IMG;
    public Sprite Round3IMG;
    public Sprite winPlayer1;
    public Sprite winPlayer2;
    VSGameController vsController;

    VSGameController.STATE oldState = VSGameController.STATE.NONE;

    // Start is called before the first frame update
    void Start()
    {
        vsController = this.GetComponent<VSGameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if( oldState == vsController.state)
        {
            return;
        }
                    
        
        // ラウンド開始になったら
        switch( vsController.state )
        {
            case VSGameController.STATE.ROUND:
                RoundUIProc();
                break;
            case VSGameController.STATE.BATTLE:
                BattleUIProc();
                break;
            case VSGameController.STATE.RESULT:
                ResultUIProc();
                break;
        }


        oldState = vsController.state;
    }

    void RoundUIProc()
    {
        GameObject _go = Instantiate(roundUI);
        Image _resultIMG = _go.transform.Find("RoundImage").GetComponent<Image>();
        switch(vsController.roundNum)
        {
            case 1:
                _resultIMG.sprite = Round1IMG;
                break;
            case 2:
                _resultIMG.sprite = Round2IMG;
                break;
            case 3:
                _resultIMG.sprite = Round3IMG;
                break;
        }

        CloneUIForAllDisplay( _go, 2.5F);
    }

    void BattleUIProc()
    {
        GameObject _go = Instantiate(battleUI);
        CloneUIForAllDisplay( _go, vsController.PLAYTIME);
    }

    void ResultUIProc()
    {

        if( vsController.IsTimerOver())
        {
            GameObject _go = Instantiate(drawUI);
            CloneUIForAllDisplay( _go, 2.5F);
        }
        else
        {
            GameObject _go = Instantiate(resultUI);
            Image _playerIMG = _go.transform.Find("Get_Point").GetComponent<Image>();
            if( vsController.IsPlayer1())
            {
                _playerIMG.sprite = winPlayer1;
            }
            else
            {
                _playerIMG.sprite = winPlayer2;
            }

            CloneUIForAllDisplay( _go, 2.5F);
        }



        
    }

    void CloneUIForAllDisplay( GameObject _go, float _destroyTime)
    {
        GameObject _go_canvas2 = Instantiate(_go);
        _go_canvas2.GetComponent<Canvas>().targetDisplay = 1;
        GameObject _go_canvas3 = Instantiate(_go);
        _go_canvas3.GetComponent<Canvas>().targetDisplay = 7;

        // Destroyの処理
        Destroy(_go, _destroyTime);
        Destroy(_go_canvas2, _destroyTime);
        Destroy(_go_canvas3, _destroyTime);
    }
}

