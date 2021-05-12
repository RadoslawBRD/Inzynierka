using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    public static int dataBufferSize = 4096;

    public int id;
    public Player player;
    public TCP tcp;
    public UDP udp;

    public Client(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }


    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private Packet recivedData;
        private byte[] reciverBuffer;


        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();

            recivedData = new Packet();
            reciverBuffer = new byte[dataBufferSize];

            stream.BeginRead(reciverBuffer, 0, dataBufferSize, ReciveCallback, null);

            ServerSend.Welcome(id, "Witaj na serwerze!");
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to player {id} via TCP: {_ex}");
            }

        }


        private void ReciveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(reciverBuffer, _data, _byteLength);

                recivedData.Reset(HandleData(_data));
                stream.BeginRead(reciverBuffer, 0, dataBufferSize, ReciveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error reciving TCP data: {_ex}");
                Server.clients[id].Disconnect();
            }
        }

        private bool HandleData(byte[] _data) //pilnuje ¿eby nie uciê³o danych jeœli s¹ roz³o¿one na dwa pakiety(tak dzia³a TCP, wysy³a pakiety w paczkach)
        {
            int _packetLenght = 0;

            recivedData.SetBytes(_data);

            if (recivedData.UnreadLength() >= 4) //bo int ma 4 bity
            {
                _packetLenght = recivedData.ReadInt();
                if (_packetLenght <= 0)
                {
                    return true;
                }
            }

            while (_packetLenght > 0 && _packetLenght <= recivedData.UnreadLength()) // jeœli to dzia³a to znaczy, ¿e mamy jeszcze pakiet, który mo¿emy obs³uzyæ
            {
                byte[] _packetBytes = recivedData.ReadBytes(_packetLenght);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });

                _packetLenght = 0;
                if (recivedData.UnreadLength() >= 4) //bo int ma 4 bity
                {
                    _packetLenght = recivedData.ReadInt();
                    if (_packetLenght <= 0)
                    {
                        return true;
                    }
                }

                if (_packetLenght <= 1)
                {
                    return true;
                }
            }
            return false;

        }


        public void Disconnect()
        {
            socket.Close();
            stream = null;
            recivedData = null;
            reciverBuffer = null;
            socket = null;
        }
    }
    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int _id)
        {
            id = _id;
        }


        public void Connect(IPEndPoint _endpoint)
        {
            endPoint = _endpoint;
        }
        public void SendData(Packet _packet)
        {
            Server.SendUDPData(endPoint, _packet);
        }

        public void HandleData(Packet _packetData)
        {

            int _packetLenght = _packetData.ReadInt(); // zapisuje d³ugoœæ pakietu z odczytu pakietu
            byte[] _packetBytes = _packetData.ReadBytes(_packetLenght); // odczytuje bity wg d³ugoœci pakiet

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packetHandlers[_packetId](id, _packet);//wywo³uje odpowiednie obs³u¿enie ze s³ownika
                }
            });
        }
        public void Disconnect()
        {
            endPoint = null;
        }
    }

    public void SendIntoGame(string _playerName)
    {
        player = NetworkManager.instance.InstantiatePlayer();
        player.Initialize(id, _playerName);
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                if (_client.id != id)
                {
                    ServerSend.SpawnPlayer(id, _client.player);
                }
            }
        }
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                ServerSend.SpawnPlayer(_client.id, player);
            }
        }
        foreach(ItemSpawner _itemSpawner in ItemSpawner.spawners.Values)
        {
            ServerSend.CreateItemSpawner(id, _itemSpawner.spawnerId, _itemSpawner.transform.position, _itemSpawner.hasItem);
        }

        foreach(Enemy _enemy in Enemy.enemies.Values)
        {
            ServerSend.SpawnEnemy(id, _enemy);
        }
    }

    private void Disconnect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} player disconnected");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;


        });

        tcp.Disconnect();
        udp.Disconnect();
        ServerSend.PlayerDisconnected(id);
    }
}

