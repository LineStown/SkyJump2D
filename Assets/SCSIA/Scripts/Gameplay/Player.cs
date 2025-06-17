using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SCSIA
{
    public class Player : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Player Data")]
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private Rigidbody2D _playerRigitbody;
        [SerializeField] private Collider2D _playerCollider;

        private Vector3 _playerLeftTurn;
        private Vector3 _playerRightTurn;
        private Vector2 _playerMove;

        private PlayerConfigId _playerConfigId;
        private PlayerConfig _playerConfig;

        private bool _playerJump;

        private Transform _platformTransform;
        private Rigidbody2D _platformRigitbody;

        private InputSystemActions _inputSystemActions;
        private float _screenMinX;
        private float _screenMaxX;

        //############################################################################################
        // PUBLIC METHODS
        //############################################################################################


        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        // awake
        private void Awake()
        {
            // direction
            _playerLeftTurn = new Vector3(-_playerRigitbody.transform.localScale.x, _playerRigitbody.transform.localScale.y, _playerRigitbody.transform.localScale.z);
            _playerRightTurn = new Vector3(_playerRigitbody.transform.localScale.x, _playerRigitbody.transform.localScale.y, _playerRigitbody.transform.localScale.z);
            // jump
            _playerJump = false;
            // set start hero
            SwitchHero(PlayerConfigId.MaskDude);
            // input system
            _inputSystemActions = new InputSystemActions();
            // get screen
            _screenMinX = Sys.Instance.GetScreenMinX();
            _screenMaxX = Sys.Instance.GetScreenMaxX();
        }

        // fixed update
        private void FixedUpdate()
        {
            // run
            _playerRigitbody.linearVelocityX = _playerMove.x * _playerConfig.speed + ((_platformRigitbody) ? _platformRigitbody.linearVelocityX : 0);

            if (_platformRigitbody)
                Debug.Log(_platformRigitbody.linearVelocityX);

            // left <> right
            if (_playerRigitbody.position.x < _screenMinX || _playerRigitbody.position.x > _screenMaxX)
                _playerRigitbody.position = new Vector2(_playerRigitbody.position.x * -0.99f, _playerRigitbody.position.y);
           
            // jump
            if (_playerJump)
            {
                _playerRigitbody.AddForce(Vector3.up * _playerConfig.jumpForce, ForceMode2D.Impulse);
                _playerJump = false;
            }

            _playerAnimator.SetBool("Jump", !_platformTransform);
            _playerAnimator.SetFloat("VelocityY", _playerRigitbody.linearVelocityY);
        }

        // player landed to platform
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!this.gameObject.activeSelf || _platformTransform)
                return;

            if (collision.gameObject.TryGetComponent<BasePlatform>(out BasePlatform platform) && platform.gameObject.activeSelf)
                foreach (ContactPoint2D contact in collision.contacts)
                    if (contact.normal.y > 0.5f)
                    {
                        _platformTransform = platform.transform;
                        _platformRigitbody = platform.gameObject.GetComponent<Rigidbody2D>();
                        //this.gameObject.transform.SetParent(_platformTransform);
                        platform.PlayedLanded();
                        Debug.Log($"{name} landed on {collision.gameObject.name}");
                        break;
                    }
        }

        // player took off from platform 
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!this.gameObject.activeSelf || !_platformTransform)
                return;

            if (collision.gameObject.TryGetComponent<BasePlatform>(out BasePlatform platform) && platform.transform == _platformTransform && platform.gameObject.activeSelf)
            {
                _platformTransform = null;
                _platformRigitbody = null;
                //this.gameObject.transform.SetParent(null);
                platform.PlayerTookOff();
                Debug.Log($"{name} took off {collision.gameObject.name}");
            }
        }

        // control
        private void OnEnable()
        {
            _inputSystemActions.Player.Enable();
            _inputSystemActions.Player.Move.performed += OnMove;
            _inputSystemActions.Player.Move.canceled += OnMove;
            _inputSystemActions.Player.Jump.performed += OnJump;
            _inputSystemActions.Player.PlayerConfig1.performed += ctx => SwitchHero(PlayerConfigId.MaskDude);
            _inputSystemActions.Player.PlayerConfig2.performed += ctx => SwitchHero(PlayerConfigId.NinjaFrog);
            _inputSystemActions.Player.PlayerConfig3.performed += ctx => SwitchHero(PlayerConfigId.PinkMan);
            _inputSystemActions.Player.PlayerConfig4.performed += ctx => SwitchHero(PlayerConfigId.VirtualGuy);
        }

        private void OnDisable()
        {
            _inputSystemActions.Player.Move.performed -= OnMove;
            _inputSystemActions.Player.Move.canceled -= OnMove;
            _inputSystemActions.Player.Jump.performed -= OnJump;
            _inputSystemActions.Player.PlayerConfig1.performed -= ctx => SwitchHero(PlayerConfigId.MaskDude);
            _inputSystemActions.Player.PlayerConfig2.performed -= ctx => SwitchHero(PlayerConfigId.NinjaFrog);
            _inputSystemActions.Player.PlayerConfig3.performed -= ctx => SwitchHero(PlayerConfigId.PinkMan);
            _inputSystemActions.Player.PlayerConfig4.performed -= ctx => SwitchHero(PlayerConfigId.VirtualGuy);
            _inputSystemActions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            // get current move
            _playerMove = context.ReadValue<Vector2>();
            // turn player
            if (_playerMove.x != 0)
                _playerRigitbody.transform.localScale = (_playerMove.x < 0) ? _playerLeftTurn : _playerRightTurn;
            // enable animation run
            _playerAnimator.SetBool("Run", _playerMove.x != 0);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (_platformTransform && !_playerJump)
                 _playerJump = true;
        }

        private void SwitchHero(PlayerConfigId playerConfigId)
        {
            if (_playerConfigId == playerConfigId && _playerConfig)
                return;
            _playerConfigId = playerConfigId;
            _playerConfig = Sys.Instance.GetPlayerConfig(_playerConfigId);
            _playerAnimator.runtimeAnimatorController = _playerConfig.animatorController;
            _playerAnimator.SetTrigger("Switch");
        }
    }
}
