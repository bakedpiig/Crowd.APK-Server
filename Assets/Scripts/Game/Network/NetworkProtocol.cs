using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Crowd.Game.Network
{
    public class NetworkProtocol
    {
        public static ByteArrayWrapper[] Receive(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            var messages = new List<ByteArrayWrapper>();
            while(stream.DataAvailable)
            {
                byte[] bufferLength = new byte[4];
                stream.Read(bufferLength, 0, bufferLength.Length);
                int msgSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bufferLength, 0));
                byte[] readBuffer = new byte[msgSize];
                stream.Read(readBuffer, 0, readBuffer.Length);
                messages.Add(new ByteArrayWrapper(readBuffer));
            }

            return messages.ToArray();
        }

        public static void Send(TcpClient client, ByteArrayWrapper binaryData)
        {
            if (client == null) return;
            NetworkStream stream = client.GetStream();
            byte[] writeBuffer = binaryData.binaryData;
            int msgSize = writeBuffer.Length;
            byte[] bufferLegnth = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(msgSize));
            stream.Write(bufferLegnth, 0, bufferLegnth.Length);
            stream.Write(writeBuffer, 0, msgSize);
        }
    }
}