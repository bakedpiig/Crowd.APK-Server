using Crowd.Game.Network;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UniRx;
using UnityEngine;

namespace Crowd.Game
{
    public class GameDirector : SubDirector
    {
        private TcpListener listener = new TcpListener(IPAddress.Any, 1935);
        private TcpClient[] clients = new TcpClient[10];
        private Player[] players = new Player[10];
        private ReactiveProperty<BitArray> Ready = new ReactiveProperty<BitArray>();
        private int clientCount = 0;

        private void Awake()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 60;

            Debug.Log($"Server opened: {IPAddress.Any}");
            Ready.Value = new BitArray(10, false);
            Ready.Where(arr => arr.Cast<bool>().All(_ => _)).Subscribe(_ => ClientReady()).AddTo(gameObject);

            listener.Start();

        }

        private void Update()
        {
            if (listener.Pending())
            {
                var client = listener.AcceptTcpClient();
                if (clientCount >= 10)
                {
                    var oms = new OutputMemoryStream();
                    oms.Write((int)RequireType.Ready);
                    oms.Write(false);
                    NetworkProtocol.Send(client, new ByteArrayWrapper(oms.buffer));
                }

                int clientIdx = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (clients[i] is null)
                    {
                        Debug.Log($"New client accepted/ IP: {(client.Client.RemoteEndPoint as IPEndPoint).Address}, Port: {(client.Client.RemoteEndPoint as IPEndPoint).Port}");
                        clients[i] = client;
                        clientCount++;
                        clientIdx = i;
                        break;
                    }
                }
                return;
            }
            
            for(int i=0;i<10;i++)
            {
                if (clients[i] is null) continue;
                var messages = NetworkProtocol.Receive(clients[i]);
                foreach(var msg in messages)
                {
                    var ims = new InputMemoryStream(msg.binaryData);
                    ims.Read(out int type);

                    switch((RequireType)type)
                    {
                        case RequireType.Ready:
                            ims.Read(out string name);
                            players[i] = new Player(name);
                            var newArr = (BitArray)Ready.Value.Clone();
                            newArr[i] = true;
                            Ready.Value = newArr;
                            break;
                        case RequireType.SendData:
                            break;
                        case RequireType.Disconnect:
                            break;
                    }
                }
            }
        }


        private void ClientReady() =>
            clients.ToList().ForEach(client =>
            {
                var oms = new OutputMemoryStream();
                oms.Write((int)RequireType.Ready);
                foreach(var player in players)
                    oms.Write(player.Nickname);
                client.Client.Send(oms.buffer);
            });
    }
}