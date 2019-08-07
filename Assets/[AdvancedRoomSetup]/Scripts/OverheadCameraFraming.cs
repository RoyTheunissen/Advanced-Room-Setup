using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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

        private Bounds bounds;
        private bool isInitialized;

        private void Encapsulate(Vector3 position)
        {
            if (!isInitialized)
            {
                bounds.center = position;
                bounds.size = Vector3.zero;
                isInitialized = true;
            }
            else
                bounds.Encapsulate(position);
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
            
            bounds.Expand(cameraPadding);

            camera.transform.position = new Vector3(
                bounds.center.x, camera.transform.position.y, bounds.center.z);

            camera.orthographicSize = Mathf.Max(cameraSizeMin, 
                bounds.extents.x, bounds.extents.y, bounds.extents.z);
        }
    }
}
