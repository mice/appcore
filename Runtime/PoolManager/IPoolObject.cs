
using UnityEngine;
namespace Pool
{
    public interface IPoolObject
    {
        void OnDespawn();
        void OnSpawn();
        Transform GetTransform();
        SpawnPool Pool { get; set; }
        bool Cached { get; set; }
    }
}