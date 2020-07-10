namespace Crowd.Game
{
    public class Player
    {
        public string Nickname { get; private set; }
        public int Score { get; private set; }

        public Player() { }
        public Player(string nickname)
        {
            Nickname = nickname;
            Score = 0;
        }
    }
}
