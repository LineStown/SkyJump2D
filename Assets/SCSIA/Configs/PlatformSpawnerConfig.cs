using UnityEngine;

namespace SCSIA
{
    [CreateAssetMenu(fileName = "PlatformSpawnerConfig", menuName = "Scriptable Objects/PlatformSpawnerConfig")]

    public class PlatformSpawnerConfig : ScriptableObject
    {
        [Header("Spawn Settings")]
        public int maxStage = 20;
        public int maxSpawnByDirection = 5;
        public float platformMinYFromPrevious = 2.5f;
        public float platformMaxYFromPrevious = 3.0f;
        public int minPlatformsByStage = 1;
        public int maxPlatformsByStage = 6;
    }
}
