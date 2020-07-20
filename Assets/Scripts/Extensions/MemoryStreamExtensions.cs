using Crowd.Game;
using UnityEngine;

namespace Crowd
{
    public static class MemoryStreamExtensions
    {
        public static void Read(this InputMemoryStream ims,out Vector2 data)
        {
            ims.Read(out float x);
            ims.Read(out float y);
            data = new Vector2(x, y);
        }

        public static void Write(this OutputMemoryStream oms,Vector2 data)
        {
            oms.Write(data.x);
            oms.Write(data.y);
        }

        public static void Read(this InputMemoryStream ims,out byte[] data)
        {
            ims.Read(out int length);
            data = new byte[length];

            for(int i=0;i<length;i++)
            {
                ims.Read(out byte temp);
                data[i] = temp;
            }
        }

        public static void Write(this OutputMemoryStream oms,byte[] data)
        {
            oms.Write(data.Length);
            foreach (var b in data)
                oms.Write(b);
        }
    }
}