using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Definition
{
    public enum GameStateType
    {
        None,
        Loading,
        GameplayRunning,
        GameplayPausing,
        GameplayChoosingItem,
        GameplayLobby,
        WinGameplay,
        LoseGameplay,
        Lobby,
    }
}