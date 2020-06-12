using UnityEngine;

namespace Crowd.Game
{
    public class DirectorsPool: ObjectPool
    {
        private new void Use<T>() { }

        public T UseDirector<T>() where T: SubDirector
        {
            var typeName = typeof(T).Name;

            if (poolDictionary[typeName].Exists(obj => obj.activeSelf))
                return poolDictionary[typeName][0].GetComponent<T>();
            else
                return base.Use<T>().GetComponent<T>();
        }
    }
}
