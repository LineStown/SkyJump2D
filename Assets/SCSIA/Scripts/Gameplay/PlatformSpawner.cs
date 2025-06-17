using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCSIA
{
    public class PlatformSpawner : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Parent for paltform pool")]
        [SerializeField] private Transform _platformPoolParent;

        [Header("Platform Prefabs List")]
        [SerializeField] private BasePlatform[] _platformPrefabs;

        [Header("BottomTarget Tramsform")]
        [SerializeField] private Transform _bottomTarget;

        [Header("PlayerTarget Transform")]
        [SerializeField] private Transform _playerTarget;

        private PlatformSpawnerConfig _platformSpawnerConfig;
        private int _playerStage;

        private float _screenMinX;
        private float _screenMaxX;

        private List<Queue<BasePlatform>> _platformPool;
        private List<List<BasePlatform>> _platformOnline;

        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        // awake
        private void Awake()
        {
            // get config for current level (TBA in future)
            _platformSpawnerConfig = Sys.Instance.GetPlatformSpawnerConfig(PlatformSpawnerConfigId.Level1);
            // current player stage (bottom platform)
            _playerStage = 0;
            // get screen
            _screenMinX = Sys.Instance.GetScreenMinX();
            _screenMaxX = Sys.Instance.GetScreenMaxX();
            // create pools for platforms
            _platformPool = new List<Queue<BasePlatform>>();
            for (int i = 0; i < _platformPrefabs.Length; i++)
                _platformPool.Add(new Queue<BasePlatform>());
            _platformOnline = new List<List<BasePlatform>>();
            // update online platform list
            UpdateOnlinePlatformList();
        }

        // fixed update
        private void FixedUpdate()
        {
            UpdateOnlinePlatformList();
        }

        // update online platform list
        private void UpdateOnlinePlatformList()
        {
            // get player position
            int playerStage = 0;
            for (int i = _platformOnline.Count - 1; i >= 0; i--)
                if (_playerTarget.position.y > _platformOnline[i].First().transform.position.y)
                {
                    playerStage = _platformOnline[i].First().Stage;
                    break;
                }
            // update online platform list
            if (_playerStage != playerStage || _platformOnline.Count == 0)
            {
                _playerStage = playerStage;
                Debug.Log($"Player on stage {_playerStage}");
                // calculate
                int countPlatformsUp = (_platformOnline.Count == 0) ? 0 : _platformOnline.Last().First().Stage - _playerStage;
                int countPlatformsDown = (_platformOnline.Count == 0) ? 0 : _playerStage - _platformOnline.First().First().Stage;
                // drop
                if (countPlatformsUp > _platformSpawnerConfig.maxSpawnByDirection)
                    DropPlatform(countPlatformsUp - _platformSpawnerConfig.maxSpawnByDirection, 1);
                if (countPlatformsDown > _platformSpawnerConfig.maxSpawnByDirection)
                    DropPlatform(countPlatformsDown - _platformSpawnerConfig.maxSpawnByDirection, -1);
                // spawn
                if (countPlatformsUp < _platformSpawnerConfig.maxSpawnByDirection)
                    SpawnPlatform(_platformSpawnerConfig.maxSpawnByDirection - countPlatformsUp, 1);
                if (countPlatformsDown < _platformSpawnerConfig.maxSpawnByDirection)
                    SpawnPlatform(_platformSpawnerConfig.maxSpawnByDirection - countPlatformsDown, -1);
            }
        }

        // spawn platforms
        private void SpawnPlatform(int count, int direction)
        {
            for (int i = 0; i < count; i++)
            {
                // platform can be spawed between 0 and maxStage
                int nextStage = (_platformOnline.Count() > 0) ? (((direction == 1) ? _platformOnline.Last().First().Stage : _platformOnline.First().First().Stage) + direction) : 1;
                if (nextStage < 1 || nextStage > _platformSpawnerConfig.maxStage)
                    return;
                // generate count platform by this stage. result count can be lower if stage does not have free space
                int maxPlatformsByCurrentStage = Random.Range(_platformSpawnerConfig.minPlatformsByStage, _platformSpawnerConfig.maxPlatformsByStage + 1);
                List<BasePlatform> platformGroup = new List<BasePlatform>();
                while (true)
                {
                    // stage full
                    if (platformGroup.Count() == maxPlatformsByCurrentStage)
                        break;
                    // prepare new platform
                    BasePlatform platform = GetPlatformFromPool(Random.Range(0, _platformPrefabs.Count()));
                    PlatformPlace maxAvailableFreePlace = GetMaxAvailableFreePlace(platformGroup);
                    // fix place according to place (screen side or another platform side)
                    maxAvailableFreePlace.minX += platform.GetRendererWidth() / 2;
                    maxAvailableFreePlace.maxX -= platform.GetRendererWidth() / 2;
                    // not enough available free space for platform on current stage 
                    if (maxAvailableFreePlace.width < platform.GetRendererWidth())
                    {
                        ReturnPlatformToPool(platform);
                        break;
                    }
                    platform.PrePlaceSetup(maxAvailableFreePlace);
                    // generate X with option part of platform offscreen
                    float platformX = Random.Range(maxAvailableFreePlace.minX, maxAvailableFreePlace.maxX);
                    // generate Y
                    float platformY = ((_platformOnline.Count() == 0) ? _bottomTarget : ((direction == 1) ? _platformOnline.Last().First().transform : _platformOnline.First().First().transform)).position.y;
                    platformY += Random.Range(_platformSpawnerConfig.platformMinYFromPrevious, _platformSpawnerConfig.platformMaxYFromPrevious) * direction;
                    platform.transform.position = new Vector3(platformX, platformY, 0);
                    platform.PostPlaceSetup();
                    platform.Stage = nextStage;
                    platform.gameObject.SetActive(true);
                    platformGroup.Add(platform);
                }
                if (direction == 1)
                    _platformOnline.Add(platformGroup);
                else
                    _platformOnline.Insert(0, platformGroup);
            }
        }

        // drop platforms
        private void DropPlatform(int count, int direction)
        {
            List<BasePlatform> platformGroup;
            for (int i = 0; i < count; i++)
            {
                if (_platformOnline.Count() == 0)
                    return;
                platformGroup = (direction == 1) ? _platformOnline.Last() : _platformOnline.First();
                foreach (BasePlatform p in platformGroup)
                    ReturnPlatformToPool(p);
                platformGroup.Clear();
                _platformOnline.RemoveAt((direction == 1) ? _platformOnline.Count() - 1 : 0);
            }
        }

        // get platform from pool
        private BasePlatform GetPlatformFromPool(int platformType)
        {
            // return first free platform
            if (_platformPool[platformType].Count > 0)
                return _platformPool[platformType].Dequeue();
            // create new platform to pool if queue for this type is empty
            BasePlatform platform = Instantiate(_platformPrefabs[platformType], Vector3.zero, Quaternion.identity, _platformPoolParent);
            platform.PlatformType = platformType;
            platform.gameObject.SetActive(false);
            return platform;
        }

        // return platform to pool
        private void ReturnPlatformToPool(BasePlatform basePlatform)
        {
            basePlatform.gameObject.SetActive(false);
            _platformPool[basePlatform.PlatformType].Enqueue(basePlatform);
        }

        // get max available free place on stage
        private PlatformPlace GetMaxAvailableFreePlace(List<BasePlatform> platformGroup)
        {
            PlatformPlace result = new PlatformPlace(0, 0, 0);
            // case empty stage
            if (platformGroup.Count() == 0)
                result.Set(_screenMinX, _screenMaxX);
            else
            {
                Debug.Log("------------------------------------------------------------------");
                // create list with points
                List<PlatformPlacePoint> testList = new List<PlatformPlacePoint>();
                // first and last point on the screen
                testList.Add(new PlatformPlacePoint(_screenMinX, 0));
                testList.Add(new PlatformPlacePoint(_screenMaxX, 0));
                // point from placed platforms
                foreach (BasePlatform p in platformGroup)
                    testList.Add(p.GetPlatformPlacePoint());
                // sort list
                testList.Sort((a, b) => a.x.CompareTo(b.x));
                // calculate the biggest place
                PlatformPlace tmpPlace = new PlatformPlace();
                for (int i = 1; i < testList.Count(); i++)
                {
                    tmpPlace.Set(testList[i - 1].x + testList[i - 1].width / 2f, testList[i].x - testList[i].width / 2f);
                    Debug.Log($"Result width {result.width}     tmp width {tmpPlace.width}");
                    if (result.width < tmpPlace.width)
                        result = tmpPlace;
                }
            }
            Debug.Log($"Width resul {result.width}");
            return result;
        }
    }
}
