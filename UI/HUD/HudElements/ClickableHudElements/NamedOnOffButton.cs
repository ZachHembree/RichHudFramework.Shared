﻿using System.Text;
using VRage;
using VRageMath;
using GlyphFormatMembers = VRage.MyTuple<byte, float, VRageMath.Vector2I, VRageMath.Color>;

namespace RichHudFramework.UI.Server
{
    /// <summary>
    /// An On/Off button with a label over it
    /// </summary>
    public class NamedOnOffButton : HudElementBase
    {
        public override float Width { get { return layout.Width; } set { layout.Width = value; } }

        public override float Height 
        { 
            get { return layout.Height; } 
            set 
            {
                if (value > Padding.Y)
                    value -= Padding.Y;

                layout.Height = value - name.Height; 
            } 
        }

        public override Vector2 Padding { get { return layout.Padding; } set { layout.Padding = value; } }

        /// <summary>
        /// The name of the control as it appears in the terminal.
        /// </summary>
        public RichText Name { get { return name.Text; } set { name.Text = value; } }

        /// <summary>
        /// Distance between the on and off buttons
        /// </summary>
        public float ButtonSpacing { get { return onOffButton.ButtonSpacing; } set { onOffButton.ButtonSpacing = value; } }

        /// <summary>
        /// Color of the border surrounding the on and off buttons
        /// </summary>
        public Color BorderColor { get { return onOffButton.BorderColor; } set { onOffButton.BorderColor = value; } }

        /// <summary>
        /// Color of the highlight border used to indicate the current selection
        /// </summary>
        public Color HighlightBorderColor { get { return onOffButton.HighlightBorderColor; } set { onOffButton.HighlightBorderColor = value; } }

        /// <summary>
        /// On button text
        /// </summary>
        public RichText OnText { get { return onOffButton.OnText; } set { onOffButton.OnText = value; } }

        /// <summary>
        /// Off button text
        /// </summary>
        public RichText OffText { get { return onOffButton.OnText; } set { onOffButton.OnText = value; } }

        /// <summary>
        /// Default glyph format used by the on and off buttons
        /// </summary>
        public GlyphFormat Format { get { return onOffButton.Format; } set { onOffButton.Format = value; } }

        /// <summary>
        /// Current value of the on/off button
        /// </summary>
        public bool Value { get { return onOffButton.Value; } set { onOffButton.Value = value; } }

        protected readonly Label name;
        protected readonly OnOffButton onOffButton;
        protected readonly HudChain layout;

        public NamedOnOffButton(HudParentBase parent = null) : base(parent)
        {
            name = new Label()
            {
                Format = RichHudTerminal.ControlFormat,
                Text = "NewOnOffButton",
                AutoResize = false,
                Height = 22f,
            };

            onOffButton = new OnOffButton();

            layout = new HudChain(true, this)
            {
                SizingMode = HudChainSizingModes.FitMembersOffAxis | HudChainSizingModes.FitChainBoth,
                Spacing = 8f,
                ChainContainer = { name, onOffButton }
            };

            Padding = new Vector2(20f, 0f);
            Size = new Vector2(250f, 72f);
        }
    }
}