using Crowd.Game.Network;
using UnityEngine;

namespace Crowd.Game
{
    public class Character: ITransmittable
    {
        private static int CharacterIdCount = 0;

        public int Id { get; private set; }
        public Vector2 Direction { get; private set; }
        public Vector2 Position { get; private set; }

        private Character() { }
        public Character(Vector2 direction)
        {
            Id = ++CharacterIdCount;
            Direction = direction;
            Position = default;
        }

        public ByteArrayWrapper Serialize()
        {
            var oms = new OutputMemoryStream();
            oms.Write(Id);
            oms.Write(Direction);
            oms.Write(Position);
            return new ByteArrayWrapper(oms.buffer);
        }

        public void Deserialize(ByteArrayWrapper data)
        {
            var ims = new InputMemoryStream(data.binaryData);
            ims.Read(out int id);
            Id = id;
            ims.Read(out Vector2 direction);
            Direction = direction;
            ims.Read(out Vector2 position);
            Position = position;
        }
    }
}
