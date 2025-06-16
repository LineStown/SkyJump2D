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

        private InputSystemActions _inputSystemActions;

        private Vector3 _playerLeftTurn;
        private Vector3 _playerRightTurn;
        private Vector2 _playerMove;

        private PlayerConfigId _playerConfigId;
        private PlayerConfig _playerConfig;

        private bool _playerJump;
        

        
        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        // awake
        private void Awake()
        {
            // input system
            _inputSystemActions = new InputSystemActions();
            
            // direction
            _playerLeftTurn = new Vector3(-_playerRigitbody.transform.localScale.x, _playerRigitbody.transform.localScale.y, _playerRigitbody.transform.localScale.z);
            _playerRightTurn = new Vector3(_playerRigitbody.transform.localScale.x, _playerRigitbody.transform.localScale.y, _playerRigitbody.transform.localScale.z);

            // set start hero
            _playerConfigId = PlayerConfigId.Default;
            SwitchHero(PlayerConfigId.MaskDude);

            _playerJump = false;
        }

        private void FixedUpdate()
        {
            // run
            _playerRigitbody.linearVelocityX = _playerMove.x * _playerConfig.speed;

            // jump
            if (_playerJump)
            {
                _playerRigitbody.AddForce(Vector3.up * _playerConfig.jumpForce, ForceMode2D.Impulse);
                _playerJump = false;
            }

            _playerAnimator.SetBool("Jump", !this.gameObject.transform.parent);
            _playerAnimator.SetFloat("VelocityY", _playerRigitbody.linearVelocityY);
        }

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
            if (this.gameObject.transform.parent && !_playerJump)
                 _playerJump = true;
        }

        private void SwitchHero(PlayerConfigId playerConfigId)
        {
            if (_playerConfigId == playerConfigId)
                return;
            _playerConfigId = playerConfigId;
            _playerConfig = Sys.Instance.GetPlayerConfig(_playerConfigId);
            _playerAnimator.runtimeAnimatorController = _playerConfig.animatorController;
            _playerAnimator.SetTrigger("Switch");

        }
    }
}
