namespace UnityEngine
{
    public static partial class QuaternionExtensions
    {
        public static Quaternion SmoothDamp(
            this Quaternion rotation, Quaternion to, ref float angularVelocity, float duration)
        {
            float delta = Quaternion.Angle(rotation, to);

            // Calculate a smooth damp from the current angle offset to 0.
            float f = Mathf.SmoothDampAngle(delta, 0.0f, ref angularVelocity, duration);

            // Compare the resulting angle to the target angle and invert it, then we know
            // how many degrees we should move towards the target rotation according to 
            // smoothdamping.
            f = 1.0f - f / delta;

            // Apply that to the rotation.
            return Quaternion.Slerp(rotation, to, f);
        }
    }
}
