using UnityEngine;

namespace Crowd.Game
{
    public class Director: MonoBehaviour
    {
        public static Director instance;
        
        private DirectorsPool directorsPool;

        private void Awake()
        {
            if (!(instance is null))
                Destroy(gameObject);

            instance = this;
            DontDestroyOnLoad(gameObject);

            directorsPool = (Resources.Load("Prefabs/DirectorsPool") as GameObject).GetComponent<DirectorsPool>();
        }

        private void Start()
        {
            GetSubDirector<GameDirector>();
        }

        public T GetSubDirector<T>() where T : SubDirector => directorsPool.UseDirector<T>();

    }
}
