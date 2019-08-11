using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace RoyTheunissen.AdvancedRoomSetup.UI.ChaperoneSpace
{
    /// <summary>
    /// Handles UI interactions with the lighthouse references in the chaperone view.
    /// </summary>
    public sealed class LightHouseReferenceUi : ChaperoneSpaceUi
    {
        [SerializeField] private new Renderer renderer;
        public Renderer Renderer => renderer;
        
        [SerializeField] private LineRenderer dragLine;

        protected override void Awake()
        {
            base.Awake();
            
            dragLine.gameObject.SetActive(false);
        }

        protected override void OnDragStart()
        {
            Debug.Log($"Started dragging lighthouse reference {name}");
            
            dragLine.gameObject.SetActive(true);
        }

        protected override void OnDrag(Vector3 position)
        {
            Debug.Log($"Dragging lighthouse reference {name} to {position}");

            Vector3 from = transform.position;
            Vector3 to = position;
            to.y = from.y;
            
            Vector3 delta = to - from;
            float distance = delta.magnitude;
            Vector3 direction = delta / distance;

            dragLine.transform.position = from;
            dragLine.transform.rotation = Quaternion.LookRotation(direction);
            dragLine.SetPosition(1, Vector3.forward * distance);
        }

        protected override void OnDropped(List<ChaperoneSpaceUi> chaperoneSpaceUis)
        {
            dragLine.gameObject.SetActive(false);
            
            LightHouseUi other = null;
            for (int i = 0; i < chaperoneSpaceUis.Count; i++)
            {
                LightHouseUi lightHouseUi = chaperoneSpaceUis[i] as LightHouseUi;
                if (lightHouseUi != null)
                {
                    other = lightHouseUi;
                    break;
                }
            }
            
            Debug.Log($"Dragged lighthouse reference onto lighthouse '{other}'");
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Activate(Matrix4x4 pose)
        {
            gameObject.SetActive(true);
            transform.position = pose.GetPosition();
            transform.rotation = Quaternion.identity;
        }
    }
}
