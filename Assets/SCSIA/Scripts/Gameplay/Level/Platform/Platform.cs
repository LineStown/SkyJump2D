using UnityEngine;

namespace SCSIA
{
    public class Platform : MonoBehaviour
    {
        //############################################################################################
        // FIELDS
        //############################################################################################
        [Header("Array of sprites")]
        [SerializeField] private Sprite[] _platformSprites;
        [SerializeField] private SpriteRenderer _platformSpriteRenderer;

        [Header("Array of colliders")]
        [SerializeField] private PolygonCollider2D[] _platformColliders;

        private bool _playerLanded;
        
        //############################################################################################
        // PUBLIC  METHODS
        //############################################################################################
        public void SetRandomSkin()
        {
            if (_platformSprites.Length == 0)
                return;
            int id = Random.Range(0, _platformSprites.Length);
            _platformSpriteRenderer.sprite = _platformSprites[id];
            for (int i = 0; i < _platformColliders.Length; i++)
                _platformColliders[i].enabled = (i == id);
        }
        
        //############################################################################################
        // PRIVATE METHODS
        //############################################################################################
        // awake
        private void Awake()
        {
            _playerLanded = false;
        }

        // enter collision
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!this.gameObject.activeSelf || _playerLanded)
                return;

            if (collision.gameObject.TryGetComponent<Player>(out _))
                foreach (ContactPoint2D contact in collision.contacts)
                    if (contact.normal.y < -0.5f)
                    {
                        collision.gameObject.transform.SetParent(this.transform);
                        _playerLanded = true;
                        Debug.Log($"{collision.gameObject.name} landed on {name}");
                        break;
                    }
        }

        // exit collision
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!this.gameObject.activeSelf || !_playerLanded)
                return;

            if (collision.gameObject.TryGetComponent<Player>(out _))
            {
                collision.gameObject.transform.SetParent(null);
                _playerLanded = false;
                Debug.Log($"{collision.gameObject.name} took off {name}");
            }
        }
    }  
}