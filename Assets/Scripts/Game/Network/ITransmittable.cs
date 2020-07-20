using Crowd.Game.Network;

namespace Crowd.Game
{
    public interface ITransmittable
    {
        public ByteArrayWrapper Serialize();
        public void Deserialize(ByteArrayWrapper data);
    }

    public enum TransmitType { Player, Action }
}
