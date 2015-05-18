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
    class Sprite
    {
        Image image;
        FloatProperty left, top;
        FloatProperty scaleX, scaleY;
        FloatProperty opacity;
        IntProperty zIndex;

        public Image Image { get { return image; } }
        public FloatProperty Left { get { return left; } }
        public FloatProperty Top { get { return top; } }
        public FloatProperty ScaleX { get { return scaleX; } }
        public FloatProperty ScaleY { get { return scaleY; } }
        public FloatProperty Opacity { get { return opacity; } }
        public IntProperty ZIndex { get { return zIndex; } }
        public Sprite(Image image)
        {
            this.image = image;
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
            left.Value = 0;
            top.Value = 0;
            scaleX.Value = 1;
            scaleY.Value = 1;
            opacity.Value = 0;
            zIndex.Value = 0;
        }
        void PositionChanged(object sender, EventArgs args)
        {
            image.SetValue(Canvas.LeftProperty, (double)Left.Value);
            image.SetValue(Canvas.TopProperty, (double)Top.Value);
        }
        void OpacityChanged(object sender, EventArgs args)
        {
            image.SetValue(Image.OpacityProperty, (double)Opacity.Value);
        }
        void ScaleChanged(object sender, EventArgs args)
        {
            var transform = image.RenderTransform as ScaleTransform;
            transform.ScaleX = scaleX.Value;
            transform.ScaleY = scaleY.Value;
        }
        void ZIndexChanged(object sender, EventArgs args)
        {
            image.SetValue(Canvas.ZIndexProperty, zIndex.Value);
        }
    }
}
