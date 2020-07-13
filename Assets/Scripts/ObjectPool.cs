using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crowd
{
    public class ObjectPool: MonoBehaviour
    {
        [Serializable]
        struct SerializeInfo
        {
            public GameObject serializeObject;
            public int initCount;
            public int plusCount;
        }

        [SerializeField]
        private List<SerializeInfo> poolSerializeList;
        protected Dictionary<string, List<GameObject>> poolDictionary = new Dictionary<string, List<GameObject>>();

        protected void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach(var info in poolSerializeList)
            {
                var objList = new List<GameObject>();
                for(int i=0;i<info.initCount;i++)
                {
                    var go = Instantiate(info.serializeObject, transform) as GameObject;
                    go.SetActive(false);
                    objList.Add(go);
                }
                poolDictionary.Add(info.serializeObject.name, objList);
            }
        }

        public GameObject Use<T>()
        {
            string typeName = typeof(T).Name;

            int objIndex = poolDictionary[typeName].FindIndex(go => !go.activeSelf);
            if(objIndex==-1)
            {
                var serializeInfo = poolSerializeList.Find(info => typeName.Equals(info.serializeObject.name));

                GameObject result = null;
                foreach(var _ in Enumerable.Range(1,serializeInfo.plusCount))
                {
                    var newGo = Instantiate(serializeInfo.serializeObject, transform) as GameObject;
                    poolDictionary[typeName].Add(newGo);
                    if (result is null) result = newGo;
                }

                return result;
            }
            else
            {
                var result = poolDictionary[typeName][objIndex];
                result.SetActive(true);
                return result;
            }
        }

        public void Destroy(GameObject go) => go.SetActive(false);
    }
}
