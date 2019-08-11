using System.Collections.Generic;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup
{
    /// <summary>
    /// Helps frame specific transforms in an overhead view.
    /// </summary>
    public sealed class OverheadCameraFraming : MonoBehaviour
    {
        public enum FramingType
        {
            Transforms,
            TransformsAndChildren,
            ChildrenOnly,
        }

        [SerializeField] private FramingType framingType;
        [SerializeField] private float cameraSizeMin = 5.0f;
        [SerializeField] private float cameraPadding = 1.0f;
        
        [SerializeField] private List<Transform> transformsToFrame = new List<Transform>();

        [Header("Dependencies")]
        [SerializeField] private new Camera camera;

        private Bounds boundsLocal;
        private bool isInitialized;

        private void Encapsulate(Vector3 position)
        {
            Vector3 positionLocal = transform.InverseTransformPoint(position);
            
            if (!isInitialized)
            {
                boundsLocal.center = positionLocal;
                boundsLocal.size = Vector3.zero;
                
                isInitialized = true;
            }
            else
            {
                boundsLocal.Encapsulate(positionLocal);
            }
        }

        private void LateUpdate()
        {
            isInitialized = false;
            for (int i = 0; i < transformsToFrame.Count; i++)
            {
                if (framingType != FramingType.ChildrenOnly)
                    Encapsulate(transformsToFrame[i].position);
                
                for (int j = 0; j < transformsToFrame[i].childCount; j++)
                {
                    if (framingType != FramingType.Transforms)
                        Encapsulate(transformsToFrame[i].GetChild(j).position);
                }
            }
            
            transform.position = transform.TransformPoint(boundsLocal.center);

            boundsLocal.Expand(cameraPadding);
            camera.orthographicSize = Mathf.Max(cameraSizeMin, 
                boundsLocal.extents.x, 0.0f, boundsLocal.extents.z);
        }

        public Vector3 GetWorldSpacePointerPosition(Vector2 screenSpacePointerPosition, float y)
        {
            Vector3 viewportPointerPosition =
                camera.ScreenToViewportPoint(screenSpacePointerPosition);
            
            Ray pointerWorldRay = camera.ViewportPointToRay(viewportPointerPosition);
            
            Plane plane = new Plane(Vector3.up, Vector3.up * y);

            plane.Raycast(pointerWorldRay, out float distance);

            return pointerWorldRay.GetPoint(distance);
        }
        
        private void OnDrawGizmos()
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, boundsLocal.extents);

            Gizmos.color = Color.black.Fade(0.25f);
            Gizmos.DrawRay(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            
            Gizmos.color = Color.yellow.Fade(0.25f);
            Gizmos.DrawLine(new Vector3(-1, 0, -1), new Vector3(1, 0, -1));
            Gizmos.DrawLine(new Vector3(-1, 0, 1), new Vector3(1, 0, 1));
            Gizmos.DrawLine(new Vector3(-1, 0, -1), new Vector3(-1, 0, 1));
            Gizmos.DrawLine(new Vector3(1, 0, -1), new Vector3(1, 0, 1));
            Gizmos.matrix = originalMatrix;
        }
    }
}
