using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSGameController : MonoBehaviour
{
    float PlayTime = 60;//１ラウンドのプレイ時間
    public float nowPlayTime {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        nowPlayTime = PlayTime;
    }

    // Update is called once per frame
    void Update()
    {
        nowPlayTime = Mathf.Clamp( nowPlayTime - Time.deltaTime, 0F, PlayTime);
    }
}
