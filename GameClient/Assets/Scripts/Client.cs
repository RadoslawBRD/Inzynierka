using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Client : MonoBehaviour
{

    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 1000;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;
    public OfflinePlayerManager offlineplayer;

    public bool isPlayer = true;
    private bool isConnected = false;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("instance already exists, destroying object!");
            Destroy(this);
        }
            
    }  

    

    private void OnApplicationQuit()
    {
        Disconnect();
    }
    
    #nullable enable
    public void ConnectedToServer(string? _ipAddress)
    {
        try
        {        

            if (_ipAddress is null || _ipAddress.Length < 4) 
            { 
                Debug.Log("Joining default address"); //dla debuga do³¹cza do localhost TODO : Powiadomienie o z³ym IP
            }
            else
            {
                string[] ipCheck = _ipAddress.Split('.');

                foreach (string s in ipCheck)
                {
                    if (int.Parse(s) > 255 || int.Parse(s) < 0)
                    {
                        throw new Exception();
                    }
                }
                ip = _ipAddress.ToString();
                Debug.Log($"Joining address: {ip}");
                            }
        }
        catch
        {
            Debug.Log("Joining default address exception"); // TODO : Powiadomienie o z³ym adresie IP
        }
#nullable restore

        tcp = new TCP();
        udp = new UDP();

        
        InitializeClientData();
        isConnected = true;
        tcp.Connect();
    }
    public class TCP
    {
        public TcpClient socket;
        private Packet recivedData;

        private NetworkStream stream;
        private byte[] reciverBuffer;

      
        public void Connect()
        {
            socket = new TcpClient
            {
            ReceiveBufferSize = dataBufferSize,
            SendBufferSize = dataBufferSize
            };
            
            reciverBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);
            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            recivedData = new Packet();

            stream.BeginRead(reciverBuffer, 0, dataBufferSize, ReciveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);

                }
            }catch(Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ReciveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(reciverBuffer, _data, _byteLength);

                recivedData.Reset(HandleData(_data)); // reset only if true => HandleData comment
                stream.BeginRead(reciverBuffer, 0, dataBufferSize, ReciveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }
        private bool HandleData(byte[] _data) //pilnuje ¿eby nie uciê³o danych jeœli s¹ roz³o¿one na dwa pakiety(tak dzia³a TCP, wysy³a pakiety w paczkach)
        {
            int _packetLenght = 0;

            recivedData.SetBytes(_data);

            if(recivedData.UnreadLength() >= 4) //bo int ma 4 bity
            {
                _packetLenght = recivedData.ReadInt();
                if (_packetLenght <= 0)
                {
                    return true;
                }
            }

            while(_packetLenght > 0 && _packetLenght <= recivedData.UnreadLength()) // jeœli to dzia³a to znaczy, ¿e mamy jeszcze pakiet, który mo¿emy obs³uzyæ
            {
                byte[] _packetBytes = recivedData.ReadBytes(_packetLenght);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
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

                if(_packetLenght <=1)
                {
                    return true;
                }
            }
            return false;

        }

        public void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            recivedData = null;
            reciverBuffer = null;
            socket = null;
        }

    }

    public class UDP //ms docsk
    {
        public UdpClient socket;
        public IPEndPoint endpoint;

        public UDP()
        {
            endpoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }


        public void Connect( int _localPort, string _currentScene)
        {
            SceneManager.LoadScene(_currentScene, LoadSceneMode.Additive);
            GameObject.Find("main Camera").SetActive(false);
            socket = new UdpClient(_localPort);

            socket.Connect(endpoint);
            socket.BeginReceive(ReciveCallback, null);

            using (Packet _packet = new Packet())
            {//utwoprzenie po³¹czenia z serwerem i otworzenie portu lokalnego tak aby klient otrzyma³ informacje z serwera
                SendData(_packet); //klasa przypisuje packetID wiêc nie trzeba tego tutaj robic rêcznie
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);//aby okresliæ, po stronie serwera, kto wysla³ pakiet
                if(socket!=null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch(Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }


        private void ReciveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endpoint);
                socket.BeginReceive(ReciveCallback, null);

                if (_data.Length < 4) // jest szansa, ¿e czêœæ pakietu przejdzie, a reszta nie, wtedy ta peirwsza czêœc jest bezu¿yteczna
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }
        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLenght = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLenght); // usuwa pierwsze 4 bity opisuj¹ce d³ugoœc pakietu
            }
            ThreadManager.ExecuteOnMainThread(() => // 
            { 
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet); //s³ownik packetid ³¹czy z pakietem
                }
            });
        }
        
        public void Disconnect()
        {
            instance.Disconnect();
            
            endpoint = null;
            socket = null;
        }
    }
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)ServerPackets.playerHealth, ClientHandle.PlayerHealth},
            { (int)ServerPackets.playerRespawned, ClientHandle.PlayerRespawned},
            { (int)ServerPackets.createItemSpawner, ClientHandle.CreateItemSpawner},
            { (int)ServerPackets.itemSpawned, ClientHandle.ItemSpawned},
            { (int)ServerPackets.itemPickedUp, ClientHandle.ItemPickedUp},
            { (int)ServerPackets.spawnProjectile, ClientHandle.SpawnProjectile},
            { (int)ServerPackets.projectilePosition, ClientHandle.ProjectilePosition},
            { (int)ServerPackets.projectileExploded, ClientHandle.ProjectileExplode},
            { (int)ServerPackets.spawnEnemy, ClientHandle.SpawnEnemy},
            { (int)ServerPackets.enemyPosition, ClientHandle.EnemyPosition},
            { (int)ServerPackets.enemyHealth, ClientHandle.EnemyHealht},
            { (int)ServerPackets.playerMoney, ClientHandle.SetPlayerMoney},




        };
        Debug.Log("Initialized packets");
    }

    public void OfflineGame()
    {
        ClientHandle.SpawnOfflinePlayer(1,"OfflinePlayer", new Vector3(25f,25f,25f), new Quaternion(1,1,1,1));
    }

    public void Disconnect()
    {
        if(isConnected)
        {

            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();
            GameManager.instance.DisconnectFromServer();
            Debug.Log("Disconnected from the server");        }
    }

}




