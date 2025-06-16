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
            {
                return value;
            }
            return null;
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
    }
}
