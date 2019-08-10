namespace UnityEngine
{
    /// <summary>
    /// Helps draw complex gizmos.
    /// </summary>
    public static partial class GizmosExtensions 
    {
        public static void DrawArrow(
            Vector3 from, Vector3 to, float arrowHeadPosition = 1.0f, float arrowHeadSize = 1.0f)
        {
            Gizmos.DrawLine(from, to);

            Vector3 delta = to - from;
            DrawArrowHead(from + delta * arrowHeadPosition, delta.normalized, arrowHeadSize);
        }

        public static void DrawArrowHead(Vector3 from, Vector3 direction, float size = 1.0f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            Quaternion rotLeft = Quaternion.Euler(0.0f, -90.0f - 45.0f, 0.0f);
            Quaternion rotRight = Quaternion.Euler(0.0f, 90.0f + 45.0f, 0.0f);
            Gizmos.DrawLine(
                from, from + rotation * rotLeft * Vector3.forward * size);
            Gizmos.DrawLine(
                from, from + rotation * rotRight * Vector3.forward * size);
        }
        
        public static void DrawArrow2d(
            Vector3 from, Vector3 to, float arrowHeadPosition = 1.0f, float arrowHeadSize = 1.0f)
        {
            Gizmos.DrawLine(from, to);

            Vector3 delta = to - from;
            DrawArrowHead2d(from + delta * arrowHeadPosition, delta.normalized, arrowHeadSize);
        }
        
        public static void DrawArrowHead2d(Vector3 from, Vector3 direction, float size = 1.0f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            Quaternion rotLeft = Quaternion.Euler(-90.0f - 45.0f, 0, 0.0f);
            Quaternion rotRight = Quaternion.Euler(90.0f + 45.0f, 0, 0.0f);
            Gizmos.DrawLine(
                from, from + rotation * rotLeft * Vector3.forward * size);
            Gizmos.DrawLine(
                from, from + rotation * rotRight * Vector3.forward * size);
        }
    }
}
