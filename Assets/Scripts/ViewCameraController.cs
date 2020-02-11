using UnityEngine;

public class ViewCameraController : MonoBehaviour
{
    //0:全体（起動時のカメラ位置）
    //1:プレイヤー１と同一
    //2:プレイヤー２と同一
    //3:プレイヤー１の俯瞰
    //4:プレイヤー２の俯瞰 
    //5:プレイヤー１・２の俯瞰
    enum CAMERAMODE
    {
        FIX = 0,
        SAME_PLAYER1,
        SAME_PLAYER2,
        VIEW_PLAYER1,
        VIEW_PLAYER2,
        VIEW
    }

    CAMERAMODE cameraMode = CAMERAMODE.VIEW;

    Vector3 mainCameraStartPosition;
    Quaternion mainCameraStartRotation;

    Transform playerCam1;
    Transform playerCam2;
    PlayerScript player1;
    PlayerScript player2;

    // Start is called before the first frame update
    void Start()
    {
        mainCameraStartPosition = this.transform.position;
        mainCameraStartRotation = this.transform.rotation;

        // キャッシュ
        playerCam1 = GameObject.Find("PlayerCamera1").transform;
        playerCam2 = GameObject.Find("PlayerCamera2").transform;
        player1 = GameObject.Find("Player1").gameObject.GetComponent<PlayerScript>();
        player2 = GameObject.Find("Player2").gameObject.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        GetCameraKey();

        switch(cameraMode)
        {
            case CAMERAMODE.FIX:
                FixCameraMode( mainCameraStartPosition, mainCameraStartRotation);
                break;
            case CAMERAMODE.SAME_PLAYER1:
                FixCameraMode( playerCam1.position, playerCam1.rotation);
                break;
            case CAMERAMODE.SAME_PLAYER2:
                FixCameraMode( playerCam2.position, playerCam2.rotation);
                break;
            case CAMERAMODE.VIEW_PLAYER1:
                ViewCameraMode( player1);
                break;
            case CAMERAMODE.VIEW_PLAYER2:
                ViewCameraMode( player2);
                break;
            case CAMERAMODE.VIEW:
                ViewCameraBetweenPlayerMode();
                break;
        }
    }

    void FixCameraMode( Vector3 _pos, Quaternion _q)
    {
        this.transform.position = _pos;
        this.transform.rotation = _q;
    }

    void ViewCameraMode( PlayerScript _player)
    {
        Vector3 _pos =_player.myPosiotion + new Vector3(0, 5f, -5f);
        Quaternion _q = Quaternion.Euler(45, 0, 0);
        
        FixCameraMode( _pos, _q);
    }

    void ViewCameraBetweenPlayerMode()
    {
        float dist_sqrt = Mathf.Sqrt(Vector3.Distance(player1.myPosiotion, player2.myPosiotion));
        Vector3 _pos = (player1.myPosiotion + player2.myPosiotion) / 2 + new Vector3(0, dist_sqrt * 3.0f, -dist_sqrt * 3.0f);
        Quaternion _q = Quaternion.Euler(45, 0, 0);
        
        FixCameraMode( _pos, _q);
    }

    void GetCameraKey()
    {
        if (Input.GetKeyDown("0")) { cameraMode = CAMERAMODE.FIX; }
        if (Input.GetKeyDown("1")) { cameraMode = CAMERAMODE.SAME_PLAYER1; }
        if (Input.GetKeyDown("2")) { cameraMode = CAMERAMODE.SAME_PLAYER2; }
        if (Input.GetKeyDown("3")) { cameraMode = CAMERAMODE.VIEW_PLAYER1; }
        if (Input.GetKeyDown("4")) { cameraMode = CAMERAMODE.VIEW_PLAYER2; }
        if (Input.GetKeyDown("5")) { cameraMode = CAMERAMODE.VIEW; }
    }
}
