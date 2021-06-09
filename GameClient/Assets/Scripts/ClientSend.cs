using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
      private static void SendTCPData(Packet _packet)
      {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);

      }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets

    public static void WelcomeRecived()
    {
        using(Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach(bool _input in _inputs)
            {
                _packet.Write(_input);                
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(_packet);//wysy³am przez udp, bo mogê straciæ ewentualne pakiety i zyskam na prêdkoœci transferu
        }
    }

    public static void PlayerShoot(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }
    public static void PlayerThrowItem(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }
    public static void PlayerSentCommand(string _commandId,string? _parameter)
    {
        int _numberOfStrings=1;
        using (Packet _packet = new Packet((int)ClientPackets.playerSentCommand))
        {
            _numberOfStrings++;
            if(!(_parameter is null))
                _numberOfStrings++;

            _packet.Write(_numberOfStrings);
            _packet.Write(_commandId);
            if (!(_parameter is null))
                _packet.Write(_parameter);




            SendTCPData(_packet); //TCP bo nie moge stracic tego pakietu

        }
    }
   #endregion
}
