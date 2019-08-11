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
        
        [NonSerialized] private Collider[] colliders;

        private bool isHovered;
        public bool IsHovered => isHovered;

        private bool isSelected;
        public bool IsSelected => isSelected;
        
        private bool isDragged;
        public bool IsDragged => isDragged;

        private float glowTarget;
        private float glowVelocity;

        private bool shouldBeInteractible = true;
        public bool ShouldBeInteractible
        {
            get { return shouldBeInteractible; }
            set
            {
                shouldBeInteractible = value;
                UpdateInteractibility();
            }
        }

        public bool IsInteractable
        {
            get { return selectable.interactable; }
            private set
            {
                selectable.interactable = value;

                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = value;
                }
                
                if (!IsInteractable)
                    isHovered = isSelected = isDragged = false;
            }
        }

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

        protected ChaperoneEditor chaperoneEditor;
        private Selectable selectable;

        protected virtual void Awake()
        {
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                Collider existingCollider = meshFilters[i].gameObject.GetComponent<Collider>();
                
                if (existingCollider == null)
                {
                    MeshCollider meshCollider =
                        meshFilters[i].gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = true;
                }
                
                meshFilters[i].gameObject.layer = RenderModelLayer;
            }
            selectable = gameObject.AddComponent<Selectable>();
            selectable.transition = Selectable.Transition.None;
            
            Navigation selectableNavigation = selectable.navigation;
            selectableNavigation.mode = Navigation.Mode.None;
            selectable.navigation = selectableNavigation;
            
            renderer = GetComponentInChildren<Renderer>();
            colliders = GetComponentsInChildren<Collider>();
            
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        protected virtual void OnDestroy()
        {
            chaperoneEditor.Unregister(this);
        }

        public void Initialize(ChaperoneEditor chaperoneEditor)
        {
            this.chaperoneEditor = chaperoneEditor;

            chaperoneEditor.Register(this);

            UpdateInteractibility();
        }

        public void UpdateInteractibility()
        {
            IsInteractable =
                chaperoneEditor.IsUiTypeInteractible(GetType()) && shouldBeInteractible;
        }

        protected virtual void Update()
        {
            UpdateInteractibility();
            
            if (!IsInteractable)
                glowTarget = 0.0f;
            else if (isDragged)
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
