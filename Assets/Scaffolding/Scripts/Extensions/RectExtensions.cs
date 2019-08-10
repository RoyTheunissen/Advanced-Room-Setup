namespace UnityEngine
{
    public static partial class RectExtensions
    {
#if UNITY_EDITOR
        public static Rect GetControlFirstRect(this Rect rect)
        {
            return new Rect(rect.x, rect.y, rect.width, UnityEditor.EditorGUIUtility.singleLineHeight);
        }
        
        public static Rect GetControlNextRect(this Rect rect)
        {
            return new Rect(rect.x, rect.yMax, rect.width, UnityEditor.EditorGUIUtility.singleLineHeight);
        }
        
        public static Rect GetControlRemainderVertical(this Rect rect, Rect occupant)
        {
            return new Rect(rect.x, occupant.yMax, rect.width, rect.height - occupant.height);
        }
        
        public static Rect GetLabelRect(this Rect rect)
        {
            return new Rect(
                rect.x, rect.y, UnityEditor.EditorGUIUtility.labelWidth,
                UnityEditor.EditorGUIUtility.singleLineHeight);
        }
        
        public static Rect GetLabelRectRemainder(this Rect rect)
        {
            return new Rect(
                rect.x + UnityEditor.EditorGUIUtility.labelWidth, rect.y,
                rect.width - UnityEditor.EditorGUIUtility.labelWidth,
                UnityEditor.EditorGUIUtility.singleLineHeight);
        }
        
        public static Rect GetLabelRect(this Rect rect, out Rect remainder)
        {
            remainder = rect.GetLabelRectRemainder();
            return rect.GetLabelRect();
        }
#endif

        public static Rect GetSubRectFromLeft(this Rect rect, float width)
        {
            return new Rect(rect.x, rect.y, width, rect.height);
        }
        
        public static Rect GetSubRectFromRight(this Rect rect, float width)
        {
            return new Rect(rect.x + rect.width - width, rect.y, width, rect.height);
        }
        
        public static Rect SubtractFromLeft(this Rect rect, float width)
        {
            return new Rect(rect.x + width, rect.y, rect.width - width, rect.height);
        }
        
        public static Rect SubtractFromRight(this Rect rect, float width)
        {
            return new Rect(rect.x, rect.y, rect.width - width, rect.height);
        }
    }
}
