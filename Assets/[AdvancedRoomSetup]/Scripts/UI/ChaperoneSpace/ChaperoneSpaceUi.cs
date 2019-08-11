using System;
using System.Collections.Generic;
using RoyTheunissen.AdvancedRoomSetup.Chaperones;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoyTheunissen.AdvancedRoomSetup.UI.ChaperoneSpace
{
    /// <summary>
    /// Responsible for detecting pointer events for chaperone space 3D objects.
    /// </summary>
    public abstract class ChaperoneSpaceUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Chaperone-space UI should be set to the 3D UI layer.
        /// </summary>
        private const int RenderModelLayer = 8;
        
        private const string GlowProperty = "_Glow";
        private const float GlowDuration = 0.1f;

        [NonSerialized] private new Renderer renderer;
        private MaterialPropertyBlock materialPropertyBlock;

        private bool isHovered;
        public bool IsHovered => isHovered;

        private bool isSelected;
        public bool IsSelected => isSelected;
        
        private bool isDragged;
        public bool IsDragged => isDragged;

        private float glowTarget;
        private float glowVelocity;

        private float Glow
        {
            get => materialPropertyBlock.GetFloat(GlowProperty);
            set
            {
                renderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetFloat(GlowProperty, value);
                renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        private List<ChaperoneSpaceUi> dropTargets = new List<ChaperoneSpaceUi>();
        
        private ChaperoneEditor chaperoneEditor;

        protected virtual void Awake()
        {
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                MeshCollider meshCollider = meshFilters[i].gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true;
                meshFilters[i].gameObject.layer = RenderModelLayer;
            }
            gameObject.AddComponent<Selectable>();
            
            renderer = GetComponentInChildren<Renderer>();
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        public void Initialize(ChaperoneEditor chaperoneEditor)
        {
            this.chaperoneEditor = chaperoneEditor;
        }

        protected virtual void Update()
        {
            if (isDragged)
                glowTarget = 1.0f;
            else if (isHovered)
                glowTarget = 0.5f;
            else
                glowTarget = 0.0f;

            Glow = Mathf.SmoothDamp(Glow, glowTarget, ref glowVelocity, GlowDuration);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            isSelected = true;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            isSelected = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragged = true;
            
            OnDragStart();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pointerScreenSpace = eventData.position;

            Vector3 pointerWorldSpace =
                chaperoneEditor.GetWorldSpacePointerPosition(pointerScreenSpace);
            
            dropTargets.Clear();
            for (int i = 0; i < eventData.hovered.Count; i++)
            {
                ChaperoneSpaceUi dropTarget = eventData.hovered[i].GetComponent<ChaperoneSpaceUi>();
                if (dropTarget == null)
                    continue;
                
                dropTargets.Add(dropTarget);
            }
            
            OnDrag(pointerWorldSpace, dropTargets);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragged = false;
            
            dropTargets.Clear();
            for (int i = 0; i < eventData.hovered.Count; i++)
            {
                ChaperoneSpaceUi dropTarget = eventData.hovered[i].GetComponent<ChaperoneSpaceUi>();
                if (dropTarget == null)
                    continue;
                
                dropTargets.Add(dropTarget);
            }

            OnDropped(dropTargets);
        }

        protected virtual void OnDragStart()
        {
        }

        protected virtual void OnDrag(Vector3 position, List<ChaperoneSpaceUi> targets)
        {
        }

        protected virtual void OnDropped(List<ChaperoneSpaceUi> chaperoneSpaceUis)
        {
        }
    }
}
