using RoyTheunissen.AdvancedRoomSetup.Chaperones;
using TMPro;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.UI
{
    /// <summary>
    /// A button for managing a chaperone.
    /// </summary>
    public sealed class ChaperoneButton : MonoBehaviour
    {
        [SerializeField] private AdvancedCalibrationButton loadButton;
        [SerializeField] private AdvancedCalibrationButton deleteButton;
        [SerializeField] private TextMeshProUGUI text;

        private Chaperone chaperone;
        public Chaperone Chaperone => chaperone;

        public delegate void LoadButtonPressedHandler(ChaperoneButton chaperoneButton);
        public event LoadButtonPressedHandler LoadButtonPressedEvent;

        public delegate void HoverStateChangedHandler(
            ChaperoneButton chaperoneButton, bool isHovered);
        public event HoverStateChangedHandler HoverStateChangedEvent;
        
        public delegate void DeleteButtonPressedHandler(ChaperoneButton chaperoneButton);
        public event DeleteButtonPressedHandler DeleteButtonPressedEvent;

        private void Awake()
        {
            loadButton.Button.onClick.AddListener(HandleLoadButtonPressedEvent);
            loadButton.HoverStateChangedEvent += HandleHoverStateChangedEvent;
            
            deleteButton.Button.onClick.AddListener(HandleDeleteButtonPressedEvent);
        }

        private void OnDestroy()
        {
            loadButton.HoverStateChangedEvent -= HandleHoverStateChangedEvent;
        }

        private void HandleLoadButtonPressedEvent()
        {
            LoadButtonPressedEvent?.Invoke(this);
        }

        private void HandleHoverStateChangedEvent(
            AdvancedCalibrationButton advancedCalibrationButton, bool isHovered)
        {
            HoverStateChangedEvent?.Invoke(this, isHovered);
        }

        private void HandleDeleteButtonPressedEvent()
        {
            DeleteButtonPressedEvent?.Invoke(this);
        }

        public void UpdateData(Chaperone chaperone)
        {
            this.chaperone = chaperone;

            if (chaperone == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            text.text = chaperone.Name;
        }
    }
}
