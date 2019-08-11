using System.Collections.Generic;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.UI.ChaperoneSpace
{
    /// <summary>
    /// Handles UI interactions with the lighthouses in the chaperone view.
    /// </summary>
    public sealed class LightHouseUi : ChaperoneSpaceUi 
    {
        protected override void OnDragStart()
        {
            Debug.Log($"Started dragging lighthouse {name}");
        }

        protected override void OnDrag(Vector3 position)
        {
            Debug.Log($"Dragging lighthouse {name} to {position}");
        }

        protected override void OnDropped(List<ChaperoneSpaceUi> chaperoneSpaceUis)
        {
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
