﻿using System;
using VRageMath;

namespace RichHudFramework.UI.Server
{
    /// <summary>
    /// LabelBoxButton modified to roughly match the appearance of buttons in the SE terminal.
    /// </summary>
    public class BorderedButton : LabelBoxButton
    {
        /// <summary>
        /// Color of the border surrounding the button
        /// </summary>
        public Color BorderColor { get { return border.Color; } set { border.Color = value; } }

        /// <summary>
        /// Thickness of the border surrounding the button
        /// </summary>
        public float BorderThickness { get { return border.Thickness; } set { border.Thickness = value; } }

        /// <summary>
        /// Background highlight color
        /// </summary>
        public override Color HighlightColor { get; set; }

        /// <summary>
        /// Text formatting used when the control gains focus.
        /// </summary>
        public GlyphFormat FocusFormat { get; set; }

        /// <summary>
        /// Background color used when the control gains focus.
        /// </summary>
        public Color FocusColor { get; set; } 

        /// <summary>
        /// If true, then the button will change formatting when it takes focus.
        /// </summary>
        public bool UseFocusFormatting { get; set; }

        protected readonly BorderBox border;
        protected GlyphFormat lastFormat;
        protected Color lastColor;

        public BorderedButton(HudParentBase parent) : base(parent)
        {
            border = new BorderBox(this)
            {
                Thickness = 1f,
                DimAlignment = DimAlignments.Both | DimAlignments.IgnorePadding,
            };

            AutoResize = false;
            Format = TerminalFormatting.ControlFormat.WithAlignment(TextAlignment.Center);
            FocusFormat = TerminalFormatting.InvControlFormat.WithAlignment(TextAlignment.Center);
            Text = "NewBorderedButton";

            TextPadding = new Vector2(32f, 0f);
            Padding = new Vector2(37f, 0f);
            Size = new Vector2(253f, 50f);
            HighlightEnabled = true;

            Color = TerminalFormatting.OuterSpace;
            HighlightColor = TerminalFormatting.Atomic;
            BorderColor = TerminalFormatting.LimedSpruce;
            FocusColor = TerminalFormatting.Mint;
            UseFocusFormatting = true;

            _mouseInput.GainedFocus += GainFocus;
            _mouseInput.LostFocus += LoseFocus;
        }

        public BorderedButton() : this(null)
        { }

        protected override void CursorEnter(object sender, EventArgs args)
        {
            if (HighlightEnabled)
            {
                if (!(UseFocusFormatting && MouseInput.HasFocus))
                {
                    lastColor = Color;
                    lastFormat = Format;
                }

                TextBoard.SetFormatting(lastFormat);
                Color = HighlightColor;
            }
        }

        protected override void CursorExit(object sender, EventArgs args)
        {
            if (HighlightEnabled)
            {
                if (UseFocusFormatting && MouseInput.HasFocus)
                {
                    Color = FocusColor;
                    TextBoard.SetFormatting(FocusFormat);
                }
                else
                {
                    Color = lastColor;
                    TextBoard.SetFormatting(lastFormat);
                }
            }
        }

        protected virtual void GainFocus(object sender, EventArgs args)
        {
            if (UseFocusFormatting)
            {
                Color = FocusColor;
                TextBoard.SetFormatting(FocusFormat);
            }
        }

        protected virtual void LoseFocus(object sender, EventArgs args)
        {
            if (UseFocusFormatting)
            {
                Color = lastColor;
                TextBoard.SetFormatting(lastFormat);
            }
        }
    }
}