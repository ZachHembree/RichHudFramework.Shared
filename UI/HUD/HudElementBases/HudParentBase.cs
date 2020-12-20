﻿using RichHudFramework.Internal;
using System;
using System.Collections.Generic;
using VRage;
using VRageMath;
using ApiMemberAccessor = System.Func<object, int, object>;
using HudSpaceDelegate = System.Func<VRage.MyTuple<bool, float, VRageMath.MatrixD>>;

namespace RichHudFramework
{
    namespace UI
    {
        using HudUpdateAccessors = MyTuple<
            ushort, // ZOffset
            byte, // Depth
            Action, // DepthTest
            Action, // HandleInput
            Action<bool>, // BeforeLayout
            Action // BeforeDraw
        >;

        /// <summary>
        /// Base class for HUD elements to which other elements are parented. Types deriving from this class cannot be
        /// parented to other elements; only types of <see cref="HudNodeBase"/> can be parented.
        /// </summary>
        public abstract class HudParentBase : IReadOnlyHudParent
        {
            /// <summary>
            /// Node defining the coordinate space used to render the UI element
            /// </summary>
            public abstract IReadOnlyHudSpaceNode HudSpace { get; }

            /// <summary>
            /// Determines whether or not an element will be drawn or process input. Visible by default.
            /// </summary>
            public virtual bool Visible { get; set; }

            /// <summary>
            /// Scales the size and offset of an element. Any offset or size set at a given
            /// be increased or decreased with scale. Defaults to 1f. Includes parent scale.
            /// </summary>
            public virtual float Scale { get; set; }

            /// <summary>
            /// Determines whether the UI element will be drawn in the Back, Mid or Foreground
            /// </summary>
            public virtual sbyte ZOffset 
            { 
                get { return _zOffset; } 
                set { _zOffset = value; } 
            }

            /// <summary>
            /// Used internally to indicate when normal parent registration should be bypassed.
            /// Child-side registration unaffected.
            /// </summary>
            protected bool blockChildRegistration;

            protected readonly List<HudNodeBase> children;

            protected readonly Action DepthTestAction;
            protected readonly Action InputAction;
            protected readonly Action<bool> LayoutAction;
            protected readonly Action DrawAction;

            protected sbyte _zOffset;

            /// <summary>
            /// Additional zOffset range used internally; primarily for determining window draw order.
            /// Don't use this unless you have a good reason for it.
            /// </summary>
            protected byte zOffsetInner;
            protected ushort fullZOffset;

            public HudParentBase()
            {
                Visible = true;
                Scale = 1f;
                children = new List<HudNodeBase>();

                DepthTestAction = SafeInputDepth;
                LayoutAction = SafeBeginLayout;
                DrawAction = SafeBeginDraw;
                InputAction = SafeBeginInput;
            }

            /// <summary>
            /// Used to calculate the distance between the screen and HUD space plane and update
            /// the element accordingly.
            /// </summary>
            protected virtual void InputDepth() { }

            /// <summary>
            /// Updates input for the element and its children. Don't override this
            /// unless you know what you're doing. If you need to update input, use 
            /// HandleInput().
            /// </summary>
            protected virtual void BeginInput()
            {
                if (Visible)
                {
                    Vector3 cursorPos = HudSpace.CursorPos;
                    HandleInput(new Vector2(cursorPos.X, cursorPos.Y));
                }
            }

            /// <summary>
            /// Updates the input of this UI element.
            /// </summary>
            protected virtual void HandleInput(Vector2 cursorPos) { }

            /// <summary>
            /// Updates layout for the element and its children. Don't override this
            /// unless you know what you're doing. If you need to update layout, use 
            /// Layout().
            /// </summary>
            protected virtual void BeginLayout(bool refresh)
            {
                if (Visible || refresh)
                    Layout();
            }

            /// <summary>
            /// Updates the layout of this UI element.
            /// </summary>
            protected virtual void Layout() { }

            /// <summary>
            /// Used to immediately draw billboards. Don't override unless that's what you're
            /// doing.
            /// </summary>
            protected virtual void BeginDraw() { }

            /// <summary>
            /// Draws the UI element.
            /// </summary>
            protected virtual void Draw() { }

            /// <summary>
            /// Adds update delegates for members in the order dictated by the UI tree
            /// </summary>
            public virtual void GetUpdateAccessors(List<HudUpdateAccessors> UpdateActions, byte treeDepth)
            {
                fullZOffset = GetFullZOffset(this);

                UpdateActions.EnsureCapacity(UpdateActions.Count + children.Count + 1);
                UpdateActions.Add(new HudUpdateAccessors(fullZOffset, treeDepth, DepthTestAction, InputAction, LayoutAction, DrawAction));

                treeDepth++;

                for (int n = 0; n < children.Count; n++)
                    children[n].GetUpdateAccessors(UpdateActions, treeDepth);
            }

            /// <summary>
            /// Registers a child node to the object.
            /// </summary>
            public virtual void RegisterChild(HudNodeBase child)
            {
                if (!blockChildRegistration)
                {
                    if (child.Parent == this && !child.Registered)
                        children.Add(child);
                    else if (child.Parent == null)
                        child.Register(this);
                }
            }

            /// <summary>
            /// Registers a collection of child nodes to the object.
            /// </summary>
            public virtual void RegisterChildren(IReadOnlyList<HudNodeBase> newChildren)
            {
                blockChildRegistration = true;

                for (int n = 0; n < newChildren.Count; n++)
                {
                    newChildren[n].Register(this);

                    if (newChildren[n].Parent != this)
                        throw new Exception("HUD Element Registration Failed.");
                }

                children.AddRange(newChildren);
                blockChildRegistration = false;
            }

            /// <summary>
            /// Unregisters the specified node from the parent.
            /// </summary>
            public virtual void RemoveChild(HudNodeBase child) 
            { 
                if (!blockChildRegistration)
                {
                    int index = children.FindIndex(x => x == child);

                    if (index != -1)
                    {
                        if (children[index].Parent == this)
                            children[index].Unregister();
                        else if (children[index].Parent == null)
                            children.RemoveAt(index);
                    }
                }
            }

            /// <summary>
            /// Calculates the full z-offset using the public offset and inner offset.
            /// </summary>
            public static ushort GetFullZOffset(HudParentBase element, HudParentBase parent = null)
            {
                byte outerOffset = (byte)(element._zOffset - sbyte.MinValue);
                ushort innerOffset = (ushort)(element.zOffsetInner << 8);

                if (parent != null)
                {
                    outerOffset += (byte)((parent.fullZOffset & 0x00FF) + sbyte.MinValue);
                    innerOffset += (ushort)(parent.fullZOffset & 0xFF00);
                }

                return (ushort)(innerOffset | outerOffset);
            }

            private void SafeInputDepth()
            {
                if (!ExceptionHandler.ClientsPaused)
                {
                    try
                    {
                        InputDepth();
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.ReportException(e);
                    }
                }
            }

            private void SafeBeginLayout(bool refresh)
            {
                if (!ExceptionHandler.ClientsPaused)
                {
                    try
                    {
                        BeginLayout(refresh);
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.ReportException(e);
                    }
                }
            }

            private void SafeBeginDraw()
            {
                if (!ExceptionHandler.ClientsPaused)
                {
                    try
                    {
                        BeginDraw();
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.ReportException(e);
                    }
                }
            }

            private void SafeBeginInput()
            {
                if (!ExceptionHandler.ClientsPaused)
                {
                    try
                    {
                        BeginInput();
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.ReportException(e);
                    }
                }
            }
        }
    }
}