using System.Collections.Generic;
using RoyTheunissen.AdvancedRoomSetup.Chaperones;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.UI.ChaperoneSpace
{
    /// <summary>
    /// Handles UI interactions with the lighthouses in the chaperone view.
    /// </summary>
    public sealed class LightHouseUi : ChaperoneSpaceUi
    {
        private LineRenderer dragLine;
        
        public void Initialize(ChaperoneEditor chaperoneEditor, LineRenderer dragLine)
        {
            base.Initialize(chaperoneEditor);

            this.dragLine = dragLine;
            this.dragLine.gameObject.SetActive(false);
        }
        
        protected override void OnDragStart()
        {
            Debug.Log($"Started dragging lighthouse {name}");
            
            dragLine.gameObject.SetActive(true);
        }

        protected override void OnDrag(Vector3 position)
        {
            Debug.Log($"Dragging lighthouse {name} to {position}");

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
            
            Debug.Log($"Dragged lighthouse onto lighthouse '{other}'");
        }
    }
}
