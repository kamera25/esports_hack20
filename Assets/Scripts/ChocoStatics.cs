using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChocoStatics : MonoBehaviour
{
    public PlayerStatics.Player ownPlayer = PlayerStatics.Player.one;

    public void SetOwnPlayer( PlayerStatics.Player _p)
    {
        ownPlayer = _p;
    }
}
