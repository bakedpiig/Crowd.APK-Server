using Crowd.Game.Network;
using SuperSocket.ClientEngine;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UniRx;
using UnityEngine;

namespace Crowd.Game
{
    public class GameDirector : SubDirector
    {
        private TcpListener listener = new TcpListener(IPAddress.Parse("192.168.137.23"), 1935);
        private TcpClient[] clients = new TcpClient[10];
        private ReactiveProperty<bool[]> Ready = new ReactiveProperty<bool[]>();
        private int clientCount = 0;

        private void Awake()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 60;

            Debug.Log($"Server opened: {IPAddress.Any}");
            Ready.Value = new bool[10];
            Ready.Where(arr => arr.All(_ => _)).Subscribe(_ => ClientReady()).AddTo(gameObject);

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
                            var newArr = (bool[])Ready.Value.Clone();
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
                client.Client.Send(oms.buffer);
            });
    }
}