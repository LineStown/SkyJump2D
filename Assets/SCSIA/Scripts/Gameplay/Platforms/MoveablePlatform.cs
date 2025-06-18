using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SCSIA
{
    public class MoveablePlatform : StaticPlatform
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Platform Rigitbody")]
        [SerializeField] private Rigidbody2D _platformRigitbody;

        [Header("Speed")]
        [SerializeField] private float _minSpeed = 2;
        [SerializeField] private float _maxSpeed = 5;

        private PlatformPlace _platformPlace;
        private float _speed;
        private int _direction;

        //############################################################################################
        // PUBLIC  METHODS
        //############################################################################################
        public override bool SetupPlatform(ref PlatformPlace platformPlace)
        {
            platformPlace.minX += _platformRendererWidth / 2f;
            platformPlace.maxX -= _platformRendererWidth / 2f;
            platformPlace.width = platformPlace.maxX - platformPlace.minX;
            _platformPlace = platformPlace;
            _speed = Random.Range(_minSpeed, _maxSpeed);
            _direction = Random.Range(0, 2) * 2 - 1;
            return (platformPlace.width > _platformRendererWidth);
        }

        // platform place point
        public override PlatformPlacePoint GetPlatformPlacePoint()
        {
            return new PlatformPlacePoint(_platformPlace.minX + ((_platformPlace.maxX - _platformPlace.minX) / 2f), _platformPlace.width);
        }
        //############################################################################################
        // PRIVATE  METHODS
        //############################################################################################
        // fixed update
        private void FixedUpdate()
        {
            _platformRigitbody.linearVelocityX = _direction * _speed;
            if (_platformRigitbody.position.x <= _platformPlace.minX)
                _direction = 1;
            if(_platformRigitbody.position.x >= _platformPlace.maxX)
                _direction = -1;
        }
    }
}
