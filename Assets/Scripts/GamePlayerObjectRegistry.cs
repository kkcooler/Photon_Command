using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerObjectRegistry
{
    public GamePlayerObject ServerPlayer
    {
        get { return _serverPlayer; }
    }

    // 玩家索引
    private Dictionary<BoltConnection, GamePlayerObject> _playerMap
        = new Dictionary<BoltConnection, GamePlayerObject>();

    private GamePlayerObject _serverPlayer
        = new GamePlayerObject();



    // utility function which creates a server player
    public GamePlayerObject CreateServerPlayer()
    {
        return CreatePlayerData(null);
    }

    // utility that creates a client player object.
    public GamePlayerObject CreateClientPeerPlayer(BoltConnection connection)
    {
        return CreatePlayerData(connection);
    }


    public GamePlayerObject GetPlayerData(BoltConnection connection)
    {
        GamePlayerObject data;
        if (connection == null)
            data = ServerPlayer;
        else
            data = _playerMap.GetValue(connection);

        return data;
    }


    
    public GamePlayerObject CreatePlayerData(BoltConnection connection)
    {
        var player = GetPlayerData(connection);
        if (player == null)
            player = new GamePlayerObject() { connection = connection, };

        if (connection == null)
            _serverPlayer = player;
        else
            _playerMap[connection] = player;

        return player;
    }
   
    public void RemoveClientPlayer(BoltConnection connection)
    {
        if (connection == null)
            return;

        try
        {
            BoltNetwork.Destroy(_playerMap[connection].character);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        _playerMap.Remove(connection);
    }




}

public class GamePlayerObject
{
    public BoltEntity character;
    public BoltConnection connection;


    public bool IsServer
    {
        get { return connection == null; }
    }
    public bool IsClient
    {
        get { return connection != null; }
    }


    public void Spawn()
    {
        if (!character)
        {
            character = BoltNetwork.Instantiate(BoltPrefabs.PlayerEntity);
            character.transform.position = RandomPosition();
        }

        if (IsServer)
        {
            character.TakeControl();
            character.name = "Player_Server";
        }
        else
        {
            character.AssignControl(connection);
            character.name = $"Player_{connection.RemoteEndPoint}";
        }
    }





    private Vector3 RandomPosition()
    {
        float x = UnityEngine.Random.Range(-3f, +3f);
        float z = UnityEngine.Random.Range(-3f, +3f);
        return new Vector3(x, 0, z);
    }
}

public static class Extention
{
    public static Tvalue GetValue<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dictionary, Tkey key)
    {
        if (key == null)
            return default(Tvalue);

        if (!dictionary.TryGetValue(key, out Tvalue value))
            return default(Tvalue);
        else
            return value;
    }
}