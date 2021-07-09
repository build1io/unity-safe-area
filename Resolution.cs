using UnityEngine;

namespace Build1.UnitySafeArea
{
    [CreateAssetMenu(fileName = "Resolution", menuName = "Build1/Safe Area/Resolution", order = 1)]
    public sealed class Resolution : ScriptableObject
    {
        [SerializeField] private float width;
        [SerializeField] private float height;

        public float Width  => width;
        public float Height => height;

        public Vector2 ToVector2()
        {
            return new Vector2(width, height);
        }

        public override string ToString()
        {
            return $"{width}x{height}";
        }
        
        /*
         * Static.
         */

        public static Resolution FromVector2(Vector2 vector)
        {
            var res = CreateInstance<Resolution>();
            res.width = vector.x;
            res.height = vector.y;
            return res;
        }
    }
}