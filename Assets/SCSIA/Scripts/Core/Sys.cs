using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SCSIA
{
    public class Sys : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [SerializeField] private string _startScene;

        // player configs
        private Dictionary<PlayerConfigId, PlayerConfig> _playerConfigs;
        // platform spawner configs
        private Dictionary<PlatformSpawnerConfigId, PlatformSpawnerConfig> _platformSpawnerConfigs;

        //############################################################################################
        // PROPERTIES
        //############################################################################################
        public static Sys Instance { get; private set; }

        //############################################################################################
        // PUBLIC METHODS
        //############################################################################################
        // get player config
        public PlayerConfig GetPlayerConfig(PlayerConfigId playerConfigId)
        {
            if (_playerConfigs.TryGetValue(playerConfigId, out PlayerConfig value))
                return value;
            return null;
        }

        // get platform spawner config
        public PlatformSpawnerConfig GetPlatformSpawnerConfig(PlatformSpawnerConfigId platformSpawnerConfigId)
        {
            if (_platformSpawnerConfigs.TryGetValue(platformSpawnerConfigId, out PlatformSpawnerConfig value))
                return value;
            return null;
        }

        public float GetScreenMinX()
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        }

        public float GetScreenMaxX()
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        }

        public float GetScreenWidth()
        {
            return GetScreenMaxX() - GetScreenMinX();
        }

        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        // awake
        private void Awake()
        {
            // create singleton
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            // load player configs
            LoadPlayerConfigs();
            // load platform spawner configs
            LoadPlatformSpawnerConfigs();

            // start game
            SceneManager.LoadScene(_startScene);
        }

        // load player configs
        private void LoadPlayerConfigs()
        {
            _playerConfigs = new Dictionary<PlayerConfigId, PlayerConfig>()
            {
                { PlayerConfigId.MaskDude, Resources.Load<PlayerConfig>("Configs/Player/PlayerConfigMaskDude") },
                { PlayerConfigId.NinjaFrog, Resources.Load<PlayerConfig>("Configs/Player/PlayerConfigNinjaFrog") },
                { PlayerConfigId.PinkMan, Resources.Load<PlayerConfig>("Configs/Player/PlayerConfigPinkMan") },
                { PlayerConfigId.VirtualGuy, Resources.Load<PlayerConfig>("Configs/Player/PlayerConfigVirtualGuy") }
            };
        }

        // load platform spawner configs
        private void LoadPlatformSpawnerConfigs()
        {
            _platformSpawnerConfigs = new Dictionary<PlatformSpawnerConfigId, PlatformSpawnerConfig>()
            {
                { PlatformSpawnerConfigId.Level1, Resources.Load<PlatformSpawnerConfig>("Configs/PlatformSpawner/PlatformSpawnerConfigLevel1") }
            };
        }
    }
}
