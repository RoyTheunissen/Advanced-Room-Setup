namespace UnityEngine
{
    public static partial class Vector2Extensions
    {
        public static Vector2 Accelerate(this Vector2 vector, Vector2 acceleration, float maxSpeed)
        {
            vector += acceleration;
            return Vector2.ClampMagnitude(vector, maxSpeed);
        }

        public static Vector2 Deccelerate(this Vector2 vector, float decceleration)
        {
            if (vector == Vector2.zero)
                return vector;

            return vector.normalized * Mathf.Max(0.0f, vector.magnitude - decceleration);
        }
    }
}
