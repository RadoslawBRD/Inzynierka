using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
   public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server:{_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeRecived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }
    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }
    public static void SpawnOfflinePlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }
    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        if(GameManager.players.TryGetValue(_id, out PlayerManager _player))
            _player.transform.position =  _position;
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();
        if(GameManager.players.TryGetValue(_id, out PlayerManager _player))
            _player.transform.rotation = _rotation;
    }
    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
        GameManager.players[_id].RemoveFloatingNickname();
        Destroy(GameManager.players[_id].gameObject);

        GameManager.players.Remove(_id);
    }

    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.players[_id].SetHealth(_health);
    }

    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }
    public static void CreateItemSpawner(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        Vector3 _spawnerPosition = _packet.ReadVector3();
        bool _hasItem = _packet.ReadBool();

        GameManager.instance.CreateItemSapwner(_spawnerId, _spawnerPosition, _hasItem);
    }

    public static void ItemSpawned(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();


        GameManager.itemSpawners[_spawnerId].ItemSpawned();
    }
    public static void ItemPickedUp(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        int _byPlayer = _packet.ReadInt();

        GranadeCount.instance.setGranadeAmount(1);
        GameManager.itemSpawners[_spawnerId].ItemPickedUp();
        GameManager.players[_byPlayer].itemCount++;
    }
    
    public static void SpawnProjectile(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        int _thrownByPlayer = _packet.ReadInt();
        string _type = _packet.ReadString();

        GameManager.instance.SpawnProjectile(_projectileId, _position, _type);
        if(_thrownByPlayer!=100)
            GameManager.players[_thrownByPlayer].itemCount--;
    }
    public static void ProjectilePosition(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        if(GameManager.projectiles.TryGetValue(_projectileId, out ProjectileManager _projectile))
            _projectile.transform.position = _position;
    }
    public static void ProjectileExplode(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        if(_projectileId is 0)
        {
            return;
        }
        GameManager.projectiles[_projectileId].Explode(_position);
    }

    public static void SpawnEnemy(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        float _maxHealth = _packet.ReadFloat();
        Vector3 _position = _packet.ReadVector3();
        string _type = _packet.ReadString();

        GameManager.instance.SpawnEnemy(_enemyId, _maxHealth, _position, _type);
    }
    public static void EnemyPosition(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        if(GameManager.enemies.TryGetValue(_enemyId, out EnemyManager _enemy))
            _enemy.transform.position = _position;
    }
    public static void EnemyHealht(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.enemies[_enemyId].SetHealth(_health);
    }

    
}

