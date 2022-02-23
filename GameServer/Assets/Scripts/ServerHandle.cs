using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    
    public static void WelcomeRecived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected and is now player {_fromClient}");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient} has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }


    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();
        string _type = _packet.ReadString();
        Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
        //Server.clients[_fromClient].player.SetSelectedEnemy(_type);

    }
    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();
        string _type = _packet.ReadString();

        Server.clients[_fromClient].player.Shoot(_shootDirection, _type);
    }
    public static void PlayerThrowItem(int _fromClient, Packet _packet)
    {
        Vector3 _throwDirection =_packet.ReadVector3();
        String _type = _packet.ReadString();

        Server.clients[_fromClient].player.ThrowItem(_throwDirection, _type);
    }

    public static void PlayerSentCommand(int _fromClient, Packet _packet)
    {
        Debug.Log("ogarniam komende");
        int _numberOfStrings = _packet.ReadInt();
        string[] _command = new string[5];
        for (int i = 0; i < _numberOfStrings-1; i++)
            _command[i] = _packet.ReadString();

        switch (_command[0])
        {
            case "start_game":
                GameManager.instance.startGame();
                break;
            case "restart_game":
                GameManager.instance.restartGame();
                break;
            case "kill_all_enemies":
                GameManager.instance.killAllEnemies();
                break;
            case "add_granade":
                GameManager.instance.addGranade(_fromClient, _command[1]);
                break;
            case "change_map":
                NetworkManager.instance.Set_map(_command[1].ToString());
                break;
            default:
                break;
        }
    }

    public static void InteractWithObject(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.InteractWithObject(_shootDirection);
    }
}

