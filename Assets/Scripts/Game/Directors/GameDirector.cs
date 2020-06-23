using Crowd.Game.Network;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Crowd.Game
{
    public class GameDirector : SubDirector
    {
        private TcpListener listener = new TcpListener(IPAddress.Any, 1935);
        private TcpClient[] clients = new TcpClient[10];
        private bool[] ready = new bool[10];
        private int clientCount = 0;

        private void Awake()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 60;

            listener.Start();

        }

        private void Update()
        {
            if(listener.Pending())
            {
                var client = listener.AcceptTcpClient();
                if(clientCount>=10)
                {
                    var oms = new OutputMemoryStream();
                    oms.Write((int)RequireType.EnterRoom);
                    oms.Write(false);
                    NetworkProtocol.Send(client, new ByteArrayWrapper(oms.buffer));
                }

                int clientIdx = 0;
                for (int i = 0; i < 10; i++)
                {
                    if(clients[i] is null)
                    {
                        clients[i] = client;
                        clientCount++;
                        clientIdx = i;
                        break;
                    }
                }

                for(int i=0;i<10;i++)
                {
                    if (clients[i] is null) continue;
                    var messages = NetworkProtocol.Receive(clients[i]);
                    foreach(var msg in messages)
                    {
                        
                    }
                }
            }
        }
    }
}