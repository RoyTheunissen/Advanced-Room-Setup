using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoyTheunissen.AdvancedRoomSetup.UI
{
    /// <summary>
    /// Helps extend button functionality with things like special hover states.
    /// </summary>
    public sealed class AdvancedCalibrationButton : MonoBehaviour, IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] private Button button;

        private bool isHovered;
        public bool IsHovered => isHovered;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }
    }
}
