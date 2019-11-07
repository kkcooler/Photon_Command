using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "GamePlay")]
public class GlobalCallbacks : Bolt.GlobalEventListener
{
    public GamePlayerObjectRegistry playerRegistry = new GamePlayerObjectRegistry();

    public override void SceneLoadLocalBegin(string scene)
    {
        playerRegistry.CreateServerPlayer().Spawn();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        playerRegistry.CreateClientPeerPlayer(connection).Spawn();
    }
}



[BoltGlobalBehaviour("GamePlay")]
public class PlayerGlobalCallbacks : Bolt.GlobalEventListener
{

}

