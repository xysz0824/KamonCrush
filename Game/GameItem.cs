using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KamonCrush
{
    class GameItem
    {
        Panel panel;
        IInputListener inputlListener;
        Image pattern;
        Image shadow;
        FloatProperty typeId;
        FloatProperty left, top;
        FloatProperty scaleX, scaleY;
        FloatProperty opacity;
        IntProperty zIndex;
        bool clicked;

        public Image Pattern { get { return pattern; } }
        public Image Shadow { get { return shadow; } }
        public FloatProperty TypeId { get { return typeId; } }
        public FloatProperty Left { get { return left; } }
        public FloatProperty Top { get { return top; } }
        public FloatProperty ScaleX { get { return scaleX; } }
        public FloatProperty ScaleY { get { return scaleY; } }
        public FloatProperty Opacity { get { return opacity; } }
        public IntProperty ZIndex { get { return zIndex; } }
        public GameItem(Canvas panel, IInputListener listener)
        {
            this.panel = panel;
            inputlListener = listener;
            clicked = false;
            pattern = new Image();
            pattern.RenderTransformOrigin = new Point(0.5f, 0.5f);
            pattern.RenderTransform = new ScaleTransform();
            pattern.MouseUp += Image_MouseUp;
            shadow = new Image();
            shadow.RenderTransform = new ScaleTransform();
            typeId = new FloatProperty();
            left = new FloatProperty();
            left.ValueChanged += PositionChanged;
            top = new FloatProperty();
            top.ValueChanged += PositionChanged;
            scaleX = new FloatProperty();
            scaleX.ValueChanged += ScaleChanged;
            scaleY = new FloatProperty();
            scaleY.ValueChanged += ScaleChanged;
            opacity = new FloatProperty();
            opacity.ValueChanged += OpacityChanged;
            zIndex = new IntProperty();
            zIndex.ValueChanged += ZIndexChanged;
            typeId.Value = 0;
            left.Value = 0;
            top.Value = 0;
            scaleX.Value = 1;
            scaleY.Value = 1;
            opacity.Value = 0;
            zIndex.Value = 0;
        }
        void PositionChanged(object sender, EventArgs args)
        {
            if (panel != null)
            {
                pattern.SetValue(Canvas.LeftProperty, (double)Left.Value);
                pattern.SetValue(Canvas.TopProperty, (double)Top.Value);
                shadow.SetValue(Canvas.LeftProperty, (double)Left.Value);
                shadow.SetValue(Canvas.TopProperty, (double)Top.Value);
            }
        }
        void OpacityChanged(object sender, EventArgs args)
        {
            pattern.SetValue(Image.OpacityProperty, (double)Opacity.Value);
            shadow.SetValue(Image.OpacityProperty, (double)Opacity.Value);
        }
        void ScaleChanged(object sender, EventArgs args)
        {
            var transform = pattern.RenderTransform as ScaleTransform;
            transform.ScaleX = scaleX.Value;
            transform.ScaleY = scaleY.Value;
            transform = shadow.RenderTransform as ScaleTransform;
            transform.ScaleX = scaleX.Value;
            transform.ScaleY = scaleY.Value;
        }
        void ZIndexChanged(object sender, EventArgs args)
        {
            if (panel != null)
            {
                shadow.SetValue(Canvas.ZIndexProperty, zIndex.Value);
                pattern.SetValue(Canvas.ZIndexProperty, zIndex.Value);
            }
        }
        #region Image EventHandlers
        void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (inputlListener != null)
                clicked = inputlListener.ItemClicked(this, clicked);
        }
        #endregion
    }
}
