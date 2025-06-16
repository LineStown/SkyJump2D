using Unity.VisualScripting;
using UnityEngine;

namespace SCSIA
{
    public class FollowCamera : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Target")]
        [SerializeField] private Transform _followTarget;
        [SerializeField] private float _followSpeed = 4.0f;

        [Space]
        [Header("Follow Axis")]
        [SerializeField] private bool _x;
        [SerializeField] private float _minX, _maxX;
        [SerializeField] private bool _y;
        [SerializeField] private float _minY, _maxY;
        [SerializeField] private bool _z;
        [SerializeField] private float _minZ, _maxZ;

        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        private void Awake()
        {
            // set start place
            transform.position = GetFollowTargetPosition();
        }

        private void Update()
        {
            // follow
            transform.position = Vector3.Lerp(transform.position, GetFollowTargetPosition(), _followSpeed * Time.deltaTime);
        }

        private Vector3 GetFollowTargetPosition()
        {
            var followTargetPosition = new Vector3(
                _x ? Mathf.Clamp(_followTarget.position.x, _minX, _maxX) : transform.position.x,
                _y ? Mathf.Clamp(_followTarget.position.y, _minY, _maxY) : transform.position.y,
                _z ? Mathf.Clamp(_followTarget.position.z, _minZ, _maxZ) : transform.position.z
            );
            return followTargetPosition;
        }
    }
}