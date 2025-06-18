using UnityEngine;

namespace SCSIA
{
    public abstract class BasePlatform : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [SerializeField] protected GameObject[] _platformRendererPrefabs;
        [SerializeField] protected Transform _platformRendererSpawnPoint;
        [SerializeField] protected Transform _platformBonusSpawnPoint;
        [SerializeField] protected Transform _platformEnemySpawnPoint;

        protected int _platformRendererType;
        protected float _platformRendererWidth;
        //############################################################################################
        // PROPERTIES
        //############################################################################################
        public int Stage { set; get; } = 0;
        public int PlatformType { set; get; } = 0;

        //############################################################################################
        // PUBLIC  METHODS
        //############################################################################################
        public virtual bool SetupPlatform(ref PlatformPlace platformPlace)
        {
            platformPlace.minX += _platformRendererWidth / 2f;
            platformPlace.maxX -= _platformRendererWidth / 2f;
            platformPlace.width = platformPlace.maxX - platformPlace.minX;
            return (platformPlace.width > _platformRendererWidth);
        }
        
        // player landed to platform
        public virtual void PlayedLanded() 
        { }
        // player took off from platform 
        public virtual void PlayerTookOff()
        { }

        // platform renderer width
        public virtual float GetRendererWidth()
        {
            return _platformRendererWidth;
        }

        // platform place point
        public virtual PlatformPlacePoint GetPlatformPlacePoint()
        {
            return new PlatformPlacePoint(this.gameObject.transform.position.x, _platformRendererWidth);
        }

        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        // awake
        protected virtual void Awake()
        {
            _platformRendererType = -1;
            _platformRendererWidth = 0;
            SetRandomSkin();
        }

        // set random skin
        protected virtual void SetRandomSkin()
        {
            int platformRendererType = Random.Range(0, _platformRendererPrefabs.Length);
            if (_platformRendererType != platformRendererType)
            {
                // destroy old skin
                foreach (Transform child in _platformRendererSpawnPoint)
                    Destroy(child.gameObject);
                // create new skin
                Instantiate(_platformRendererPrefabs[platformRendererType], _platformRendererSpawnPoint.position, Quaternion.identity, _platformRendererSpawnPoint);
                // set skin width
                _platformRendererWidth = _platformRendererSpawnPoint.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
                _platformRendererType = platformRendererType;
            }
        }
    }  
}