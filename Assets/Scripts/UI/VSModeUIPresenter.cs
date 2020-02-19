using UnityEngine;
using UnityEngine.UI;

public class VSModeUIPresenter : MonoBehaviour
{
    VSGameController vsController;
	public Image hpBar1;
    public Image hpBar2;
    public Text timerText;
    public RectTransform star1;
    public RectTransform star2;
    public RectTransform choco1;
    public RectTransform choco2;
    PlayerStatics player1;
    PlayerStatics player2; 
    
    const float CHOCO_TILE_SIZE = 40F;
    const float STAR_TILE_SIZE = 40F;
    float choco_posX;

    // Start is called before the first frame update
    void Start()
    {
        GameObject _controller = GameObject.FindWithTag("GameController");
        vsController = _controller.GetComponent<VSGameController>();

        player1 = vsController.player1;
        player2 = vsController.player2;

        choco_posX = choco1.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = vsController.nowPlayTime.ToString("00");

        hpBar1.fillAmount = player1.GetHPPercent();
        hpBar2.fillAmount = player2.GetHPPercent();

        PutChoco( player1, choco1);
        PutChoco( player2, choco2);

        PutStar( player1, star1);
        PutStar( player2, star2);
    }

    // チョコの表示処理
    void PutChoco( PlayerStatics _player, RectTransform _choco)
    {
        Vector2 _size = _choco.sizeDelta;
        _size.x = _player.bulletRemain * CHOCO_TILE_SIZE;  
        _choco.sizeDelta = _size;
    }

    // 星の表示処理
    void PutStar( PlayerStatics _player, RectTransform _star)
    {
        Vector2 _size = _star.sizeDelta;
        _size.x = _player.star * STAR_TILE_SIZE;  
        _star.sizeDelta = _size;
    }
}
