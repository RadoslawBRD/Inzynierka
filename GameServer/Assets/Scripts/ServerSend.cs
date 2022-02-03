using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    #region SendTCPData
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
            Server.clients[i].tcp.SendData(_packet);
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet) // send to all except one player
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
            if (i != _exceptClient)
                Server.clients[i].tcp.SendData(_packet);
    }
    private static void SendTCPDataToOne(int _oneClient, Packet _packet) // send to all except one player
    {
        _packet.WriteLength();
        Server.clients[_oneClient].tcp.SendData(_packet);
    }
    #endregion

    #region SendUDPData
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
            Server.clients[i].udp.SendData(_packet);
    }
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet) // send to all except one player
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
            if (i != _exceptClient)
                Server.clients[i].udp.SendData(_packet);
    }
    private static void SendUDPDataToOne(int _oneClient, Packet _packet) // send to all except one player
    {
        _packet.WriteLength();
        Server.clients[_oneClient].udp.SendData(_packet);
    }
    #endregion

    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            //_packet.Write(_player.isMaster);

            SendTCPData(_toClient, _packet); //tcp, bo to jest istotny pakiet informuj¹cy o spawnie wiêc nie moge go straciæ
        }
    }
    public static void PlayerPosition(Player _player) //wysy³a info do wszystkich o poruszaniu siê
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            SendUDPDataToAll(_packet);
        }
    }
    public static void PlayerRotation(Player _player)//wysy³a info do wszystkich o obrocie
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }
    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);
            SendTCPDataToAll(_packet);
        }
    }
    public static void PlayerHealth(Player player)
    {
        using(Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(player.id);
            _packet.Write(player.health);

            SendTCPDataToAll(_packet);
            
        }
    }
    public static void PlayerRespawned(Player player, bool _isMaster) 
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawn))
        {
            _packet.Write(player.id);
            _packet.Write(_isMaster);
            SendTCPDataToAll(_packet);

        }
    }

    public static void CreateItemSpawner(int _toClient, int _spawnerId, Vector3 _spawnerPosition, bool _hasItem)
    {
        using (Packet _packet = new Packet((int)ServerPackets.createItemSpawner))
        {
            _packet.Write(_spawnerId);
            _packet.Write(_spawnerPosition);
            _packet.Write(_hasItem);
            

            SendTCPData(_toClient, _packet);

        }
    }
    public static void ItemSpawned(int _spawnerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemSpawned))
        {
            _packet.Write(_spawnerId);

            SendTCPDataToAll(_packet);
        }

    }
    public static void ItemPickedUp(int _spawnerId, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            _packet.Write(_spawnerId);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }

    }
    public static void SpawnProjectile(ProjectileBase _projectile, int _thrownByPlayer, string _type)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnProjectile))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_thrownByPlayer);
            _packet.Write(_type);
            SendTCPDataToAll(_packet);
        }
    }

    public static void ProjectilePosition(ProjectileBase _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectilePosition))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendUDPDataToAll(_packet);
        }
    }
    public static void ProjectileExploded(ProjectileBase _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectileExploded))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendTCPDataToAll(_packet);
        }
    }
    public static void SpawnEnemy(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            SendTCPDataToAll(SpawnEnemy_Data(_enemy, _packet));
                 
        }
    }public static void SpawnEnemy(int _toClient, Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            SendTCPDataToOne(_toClient, SpawnEnemy_Data(_enemy, _packet));
        }
    }

    private static Packet SpawnEnemy_Data(Enemy _enemy, Packet _packet)
    {
        _packet.Write(_enemy.id);
        _packet.Write(_enemy.maxHealth);
        _packet.Write(_enemy.transform.position);
        _packet.Write(_enemy.type);
        return _packet;
    }
    public static void EnemyPosition(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyPosition))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.transform.position);
            _packet.Write(_enemy.transform.rotation);
            _packet.Write(_enemy.state.ToString());
            SendUDPDataToAll(_packet);
        }
    }
    public static void EnemyHealth(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyHealth))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.health);
            SendTCPDataToAll(_packet);
        }
    }
    public static void SetPlayerMoney(int _toClient, int _moneyCount)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerMoney))
        {
            _packet.Write(_toClient);
            _packet.Write(_moneyCount);

            SendTCPDataToOne(_toClient, _packet);

        }
    }

    public static void KillTargetUpdate(int _killValue, int _killTargetValue)
    {
        using (Packet _packet = new Packet((int)ServerPackets.killtargetupdate))
        {
            _packet.Write(_killValue);
            _packet.Write(_killTargetValue);

            SendTCPDataToAll(_packet);
        }
    }
    

}
