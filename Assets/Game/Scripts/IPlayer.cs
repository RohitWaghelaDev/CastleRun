using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    BrickType BrickType { get;}
    bool CanMove { get; set; }
    bool InFallState { get; set; }
    BrickStackHandler BrickStackHandler { get; }
    bool IsInSafeZone();
    void OnGameStateChanged(Gamestate gamestate);
    void ToFallState();
    void ToIdleState();
}
