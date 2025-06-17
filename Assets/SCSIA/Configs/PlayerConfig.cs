using UnityEngine;

namespace SCSIA
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]

    public class PlayerConfig : ScriptableObject
    {
        [Header("Player Settings:")]
        public float speed = 5f;
        public float jumpForce = 10f;
        public float boost = 2f;
        public AnimatorOverrideController animatorController;
    }
}
