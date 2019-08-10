using System;

namespace UnityEngine
{
    public static partial class Vector3Extensions
    {
        public static void GetRelationTo(
            this Vector3 vector, Vector3 to, out Vector3 delta, out float distance,
            out Vector3 direction)
        {
            delta = to - vector;
            distance = delta.magnitude;
            direction = delta / distance;
        }

        public static float GetDistance(
            this Vector3 vector, Vector3 to, DistanceType type = DistanceType.Cartesian)
        {
            switch (type)
            {
                case DistanceType.Cartesian:
                    return Vector3.Distance(vector, to);
                case DistanceType.Chebyshev:
                    return Mathf.Abs(to.x - vector.x) + Mathf.Abs(to.y - vector.y) +
                           Mathf.Abs(to.z - vector.z);
                case DistanceType.ShortestAxis:
                    return Mathf.Min(Mathf.Abs(to.x - vector.x), Mathf.Abs(to.y - vector.y),
                           Mathf.Abs(to.z - vector.z));
                case DistanceType.LongestAxis:
                    return Mathf.Max(Mathf.Abs(to.x - vector.x), Mathf.Abs(to.y - vector.y),
                        Mathf.Abs(to.z - vector.z));
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static float GetDistance2d(
            this Vector3 vector, Vector3 to, DistanceType type = DistanceType.Cartesian)
        {
            switch (type)
            {
                case DistanceType.Cartesian:
                    return Vector2.Distance(vector, to);
                case DistanceType.Chebyshev:
                    return Mathf.Abs(to.x - vector.x) + Mathf.Abs(to.y - vector.y);
                case DistanceType.ShortestAxis:
                    return Mathf.Min(Mathf.Abs(to.x - vector.x), Mathf.Abs(to.y - vector.y));
                case DistanceType.LongestAxis:
                    return Mathf.Max(Mathf.Abs(to.x - vector.x), Mathf.Abs(to.y - vector.y),
                        Mathf.Abs(to.z - vector.z));
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static Vector3 FlattenY(this Vector3 vector)
        {
            vector.y = 0.0f;
            return vector.normalized;
        }
        
        public static Vector3 FlattenZ(this Vector3 vector)
        {
            vector.z = 0.0f;
            return vector.normalized;
        }

        public static Vector3 Accelerate(
            this Vector3 vector, Vector3 acceleration, float maxSpeed,
            bool decelerateIfNecessary = false)
        {
            Vector3 accelerationOriginal = acceleration;

            // Calculate what the new velocity would be after the acceleration.
            Vector3 newVelocity = vector + acceleration;

            // Clamp that new velocity to our allowed max speed.
            newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);

            // Now calculate what the acceleration would be to reach that clamped new velocity.
            acceleration = newVelocity - vector;

            // Disallow deceleration if necessary.
            if (!decelerateIfNecessary && Vector3.Dot(acceleration, accelerationOriginal) <= 0)
                return vector;

            // Apply the clamped acceleration.
            return vector + acceleration;
        }

        public static Vector3 Deccelerate(this Vector3 vector, float decceleration)
        {
            if (vector == Vector3.zero)
                return vector;

            return vector.normalized * Mathf.Max(0.0f, vector.magnitude - decceleration);
        }
        
        public static Vector3 DeccelerateHorizontal(this Vector3 vector, float decceleration)
        {
            Vector2 horizontal = new Vector2(vector.x, vector.z);
            horizontal = horizontal.Deccelerate(decceleration);
            return new Vector3(horizontal.x, vector.y, horizontal.y);
        }

        public static Vector3 Scale(this Vector3 vector, float scale)
        {
            if (vector == Vector3.zero)
                return vector;

            // Ensure that it never truly has a scale of 0.
            if (scale == 0.0f)
                vector = Vector3.one * float.Epsilon;

            vector *= scale;
            return vector;
        }

        public static bool IsInAxisAlignedRange(this Vector3 vector3, Vector3 other, float range)
        {
            return Mathf.Abs(other.x - vector3.x).EqualOrSmaller(range) &&
                Mathf.Abs(other.y - vector3.y).EqualOrSmaller(range) &&
                Mathf.Abs(other.z - vector3.z).EqualOrSmaller(range);
        }

        public static float GetChebyshevDistance(this Vector3 vector3, Vector3 other)
        {
            return Mathf.Abs(other.x - vector3.x)
                + Mathf.Abs(other.y - vector3.y)
                + Mathf.Abs(other.z - vector3.z);
        }
        
        public static Vector3 SmoothDamp(
            this Vector3 direction, Vector3 to, ref float angularVelocity, float duration)
        {
            float delta = Vector3.Angle(direction, to);

            // Calculate a smooth damp from the current angle offset to 0.
            float f = Mathf.SmoothDampAngle(delta, 0.0f, ref angularVelocity, duration);

            // Compare the resulting angle to the target angle and invert it, then we know
            // how many degrees we should move towards the target direction according to 
            // smoothdamping.
            f = delta.Equal(0.0f) ? 0.0f : 1.0f - f / delta;

            // Apply that to the direction.
            return Vector3.Slerp(direction, to, f);
        }
    }
}
