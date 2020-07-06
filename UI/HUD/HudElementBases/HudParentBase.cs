﻿using RichHudFramework.Internal;
using System;
using System.Collections.Generic;
using VRage;
using VRageMath;
using ApiMemberAccessor = System.Func<object, int, object>;

namespace RichHudFramework
{
    using HudElementMembers = MyTuple<
        Func<bool>, // Visible
        object, // ID
        Action<bool>, // BeforeLayout
        Action<int, MatrixD>, // BeforeDraw
        Action<int>, // HandleInput
        ApiMemberAccessor // GetOrSetMembers
    >;

    namespace UI
    {
        /// <summary>
        /// Base class for HUD elements to which other elements are parented. Types deriving from this class cannot be
        /// parented to other elements; only types of <see cref="IHudNode"/> can be parented.
        /// </summary>
        public abstract class HudParentBase : IHudParent
        {
            /// <summary>
            /// Determines whether or not an element will be drawn or process input. Visible by default.
            /// </summary>
            public virtual bool Visible { get; set; }

            /// <summary>
            /// Unique identifer.
            /// </summary>
            public object ID => this;

            protected HudLayers _zOffset;
            protected readonly List<IHudNode> children;

            public HudParentBase()
            {
                Visible = true;
                children = new List<IHudNode>();
            }

            /// <summary>
            /// Updates input for the element and its children. Don't override this
            /// unless you know what you're doing. If you need to update input, use 
            /// HandleInput().
            /// </summary>
            public virtual void BeforeInput(HudLayers layer)
            {
                for (int n = children.Count - 1; n >= 0; n--)
                {
                    if (children[n].Visible)
                        children[n].BeforeInput(layer);
                }

                if (_zOffset == layer)
                    HandleInput();
            }

            /// <summary>
            /// Updates the input of this UI element.
            /// </summary>
            protected virtual void HandleInput() { }

            /// <summary>
            /// Updates layout for the element and its children. Don't override this
            /// unless you know what you're doing. If you need to update layout, use 
            /// Layout().
            /// </summary>
            public virtual void BeforeLayout(bool refresh)
            {
                Layout();

                for (int n = 0; n < children.Count; n++)
                {
                    if (children[n].Visible || refresh)
                        children[n].BeforeLayout(refresh);
                }
            }

            /// <summary>
            /// Updates the layout of this UI element.
            /// </summary>
            protected virtual void Layout() { }

            /// <summary>
            /// Draws the element and its children.Don't override this
            /// unless you know what you're doing. If you need to draw something, use 
            /// Draw().
            /// </summary>
            public virtual void BeforeDraw(HudLayers layer, ref MatrixD matrix)
            {
                if (_zOffset == layer)
                    Draw(ref matrix);

                for (int n = 0; n < children.Count; n++)
                {
                    if (children[n].Visible)
                        children[n].BeforeDraw(layer, ref matrix);
                }
            }

            /// <summary>
            /// Draws the UI element.
            /// </summary>
            protected virtual void Draw(ref MatrixD matrix) { }

            /// <summary>
            /// Moves the specified child element to the end of the update list in
            /// order to ensure that it's drawn on top/updated last.
            /// </summary>
            public void SetFocus(IHudNode child) =>
                SetFocusInternal(child.ID);

            private void SetFocusInternal(object childID)
            {
                int last = children.Count - 1,
                    childIndex = children.FindIndex(x => x.ID == childID);

                children.Swap(last, childIndex);
            }

            /// <summary>
            /// Registers a child node to the object.
            /// </summary>
            public virtual void RegisterChild(IHudNode child)
            {
                if (child.Parent?.ID == ID && !child.Registered)
                    children.Add(child);
                else if (child.Parent?.ID == null)
                    child.Register(this);
            }

            /// <summary>
            /// Registers a collection of child nodes to the object.
            /// </summary>
            public void RegisterChildren(IEnumerable<IHudNode> newChildren)
            {
                foreach (IHudNode child in newChildren)
                    child.Register(this);
            }

            /// <summary>
            /// Unregisters the specified node from the parent.
            /// </summary>
            public virtual void RemoveChild(IHudNode child)
            {
                if (child.Parent?.ID == ID)
                    child.Unregister();
                else if (child.Parent == null)
                    children.Remove(child);
            }

            private void RemoveChildInternal(object childID)
            {
                IHudNode node = children.Find(x => x.ID == childID);
                RemoveChild(node);
            }

            /// <summary>
            /// Retrieves the information necessary to access the <see cref="IHudParent"/> through the API.
            /// </summary>
            public HudElementMembers GetApiData()
            {
                return new HudElementMembers()
                {
                    Item1 = GetApiVisible,
                    Item2 = this,
                    Item3 = BeforeApiLayout,
                    Item4 = BeforeApiDraw,
                    Item5 = BeforeApiInput,
                    Item6 = GetOrSetMember
                };
            }

            private bool GetApiVisible() =>
                !ExceptionHandler.ClientsPaused && Visible;

            private void BeforeApiLayout(bool refresh)
            {
                if (!ExceptionHandler.ClientsPaused)
                    ExceptionHandler.Run(BeforeLayout, refresh);
            }

            private void BeforeApiDraw(int layer, MatrixD matrix)
            {
                if (!ExceptionHandler.ClientsPaused)
                    ExceptionHandler.Run(() => BeforeDraw((HudLayers)layer, ref matrix));
            }

            private void BeforeApiInput(int layer)
            {
                if (!ExceptionHandler.ClientsPaused)
                    ExceptionHandler.Run(BeforeInput, (HudLayers)layer);
            }

            protected virtual object GetOrSetMember(object data, int memberEnum)
            {
                switch ((HudParentAccessors)memberEnum)
                {
                    case HudParentAccessors.Add:
                        RegisterChild(new HudNodeData((HudElementMembers)data));
                        break;
                    case HudParentAccessors.RemoveChild:
                        RemoveChildInternal(data);
                        break;
                    case HudParentAccessors.SetFocus:
                        SetFocusInternal(data);
                        break;
                }

                return null;
            }
        }
    }
}