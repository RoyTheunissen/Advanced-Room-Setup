using System.Collections.Generic;
using RoyTheunissen.AdvancedRoomSetup.Chaperones;
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
        [SerializeField] private Transform dragTarget;
        
        private Matrix4x4 pose;
        private ChaperoneRenderer chaperoneRenderer;

        protected override void Awake()
        {
            base.Awake();
            
            dragLine.gameObject.SetActive(false);
            dragTarget.gameObject.SetActive(false);
        }

        public void Initialize(ChaperoneEditor chaperoneEditor, ChaperoneRenderer chaperoneRenderer)
        {
            base.Initialize(chaperoneEditor);

            this.chaperoneRenderer = chaperoneRenderer;
        }

        protected override void OnDragStart()
        {
            Debug.Log($"Started dragging lighthouse reference {name}");
            
            chaperoneEditor.FilterUiInteractibilityByType<LightHouseUi>();
            
            dragLine.gameObject.SetActive(true);
        }

        protected override void OnDrag(Vector3 position, List<ChaperoneSpaceUi> targets)
        {
            Vector3 from = transform.position;
            Vector3 to = position;
            to.y = from.y;
            
            Vector3 delta = to - from;
            float distance = delta.magnitude;
            Vector3 direction = delta / distance;

            dragLine.transform.position = from;
            dragLine.transform.rotation = Quaternion.LookRotation(direction);
            dragLine.SetPosition(1, Vector3.forward * distance);
            
            LightHouseUi lightHouse = null;
            for (int i = 0; i < targets.Count; i++)
            {
                LightHouseUi lightHouseUi = targets[i] as LightHouseUi;
                if (lightHouseUi != null)
                {
                    lightHouse = lightHouseUi;
                    break;
                }
            }

            if (lightHouse != null)
            {
                dragTarget.gameObject.SetActive(true);
                dragTarget.position = lightHouse.transform.position;
                dragTarget.rotation = Quaternion.identity;
            }
            else
                dragTarget.gameObject.SetActive(false);

            Debug.Log(
                $"Dragging lighthouse reference {name} to {position} over lighthouse {lightHouse}");
        }

        protected override void OnDropped(List<ChaperoneSpaceUi> chaperoneSpaceUis)
        {
            dragLine.gameObject.SetActive(false);
            dragTarget.gameObject.SetActive(false);
            
            LightHouseUi lightHouse = null;
            for (int i = 0; i < chaperoneSpaceUis.Count; i++)
            {
                LightHouseUi lightHouseUi = chaperoneSpaceUis[i] as LightHouseUi;
                if (lightHouseUi != null)
                {
                    lightHouse = lightHouseUi;
                    break;
                }
            }
            
            chaperoneEditor.FilterUiInteractibilityByType<LightHouseReferenceUi>();

            if (lightHouse != null)
            {
                chaperoneRenderer.Chaperone.Realign(
                    pose, lightHouse.transform.localToWorldMatrix);
            }
            
            Debug.Log($"Dragged lighthouse reference onto lighthouse '{lightHouse}'");
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Activate(Matrix4x4 pose)
        {
            this.pose = pose;
            gameObject.SetActive(true);
            transform.position = pose.GetPosition();
            transform.rotation = Quaternion.identity;
        }
    }
}
