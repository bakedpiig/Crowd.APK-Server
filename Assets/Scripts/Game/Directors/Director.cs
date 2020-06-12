using UnityEngine;

namespace Crowd.Game
{
    public class Director: MonoBehaviour
    {
        public static Director instance;

        private void Awake()
        {
            if (!(instance is null))
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}