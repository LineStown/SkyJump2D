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

        private Vector2 _targetPosition;
        private Vector2 _leftPosition;
        private Vector2 _rightPosition;

        //############################################################################################
        // PUBLIC  METHODS
        //############################################################################################
        // preplace setup
        public override void PrePlaceSetup(PlatformPlace platformPlace)
        {
            _platformPlace = platformPlace;
        }

        // postplace setup
        public override void PostPlaceSetup()
        {
            _speed = Random.Range(_minSpeed, _maxSpeed);
            _direction = Random.Range(0, 2) * 2 - 1;
            _leftPosition = new Vector2(_platformPlace.minX, this._platformRigitbody.position.y);
            _rightPosition = new Vector2(_platformPlace.maxX, this._platformRigitbody.position.y);
            _targetPosition = _leftPosition;
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
            Vector2 current_position = _platformRigitbody.position;
            Vector2 direction = (_targetPosition - current_position).normalized;
            _platformRigitbody.linearVelocity = direction * _speed;
            if (Vector2.Distance(current_position, _targetPosition) <= 0.1)
                _targetPosition = _targetPosition == _leftPosition ? _rightPosition : _leftPosition;
          
        }
    }
}
