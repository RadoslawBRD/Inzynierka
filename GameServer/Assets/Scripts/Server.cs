using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    //public static Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();

    public static Dictionary<int, Client> clients = new Dictionary<int, Client>(); //s³ownik przechowuj¹cy id klientów oraz objektów Client
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

  
    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        Debug.Log("Server starting...");
        InitializeServerData();


        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on {Port}.");
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint); //zwraca wszystkie bity z tablicy i ustawi endpointIP na endpoint Ÿród³a danych
            udpListener.BeginReceive(UDPReceiveCallback, null);//odbiera kolejne dane 

            if (_data.Length < 4) // jeœli poni¿ej 4 bitów, to znaczy, ze pakiet jest uszkodzony i go pomijam
            {
                return;
            }
            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();
                if (_clientId == 0)//sprawdzam czy id nie jest zerem ¿eby nie odbieraæ danych od nieistniej¹cego klienta
                {
                    return;
                }
                if (clients[_clientId].udp.endPoint == null) // jesli endpoint jest null to klient jest nowym klientem i pakiet powinien byæ pusty, otwieraj¹cy po³¹czenie
                {
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return; // wychodze, ¿eby serwer nie próbowa³ dalej przetwarzaæ tego pakieru
                }
                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())//sprawdzamy czy enpointy siê zgadzaj¹, ten z danych serwera z tymi wys³anymi przez klienta
                {
                    clients[_clientId].udp.HandleData(_packet);
                }
            }

        }
        catch (Exception _ex)
        {
            Debug.Log($"Error reciving UDP data(its fine): {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null) //sprawdze czy endpoint nie jest pusty zanim wyœle dane
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }

        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }

    }

    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
    {
        { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeRecived },
        { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement},
        { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot},
        { (int)ClientPackets.playerThrowItem, ServerHandle.PlayerThrowItem},
        { (int)ClientPackets.playerSentCommand, ServerHandle.PlayerSentCommand}

    };
        Debug.Log("Initialized packets");
    }

    
    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }

}

