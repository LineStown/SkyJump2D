using UnityEngine;

namespace SCSIA
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]

    public class PlayerConfig : ScriptableObject
    {
        // config
        public float speed = 5f;
        public float jumpForce = 10f;
        public float boostMultiply = 1.5f;
        // animator
        public AnimatorOverrideController animatorController;
    }
}
