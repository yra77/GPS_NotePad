
using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;


namespace GPS_NotePad.Controls
{
    public class PinchZoom_Control : ContentView
    {

        private readonly PinchGestureRecognizer _pinchGesture;
        private double _startScale;
        private double _currentScale;
        private double _xOffset;
        private double _yOffset;


        public PinchZoom_Control()
        {
            _pinchGesture = new PinchGestureRecognizer();
            _pinchGesture.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(_pinchGesture);

            IsScaled = true;
            _startScale = 0.0;
            _currentScale = 0.0;
            _xOffset = 0.0;
            _yOffset = 0.0;
        }


        #region Public property

        public static BindableProperty IsScaledProperty =
                BindableProperty.Create(nameof(IsScaled),
                    returnType: typeof(bool),
                    declaringType: typeof(PinchZoom_Control),
                    defaultValue: null,
                    defaultBindingMode: BindingMode.TwoWay);

        public bool IsScaled
        {
            get { return (bool)GetValue(IsScaledProperty); }
            set { SetValue(IsScaledProperty, value); }
        }

        #endregion


        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {

            if (e.Status == GestureStatus.Started)
            {
                // Store the current scale factor applied to the wrapped user interface element,
                // and zero the components for the center point of the translate transform.
                _startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
                IsScaled = false;
            }

            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.
                _currentScale += (e.Scale - 1) * _startScale;
                _currentScale = Math.Max(1, _currentScale);

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the X pixel coordinate.
                double renderedX = Content.X + _xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * _startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the Y pixel coordinate.
                double renderedY = Content.Y + _yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * _startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.
                double targetX = _xOffset - (originX * Content.Width) * (_currentScale - _startScale);
                double targetY = _yOffset - (originY * Content.Height) * (_currentScale - _startScale);

                // Apply translation based on the change in origin.
                Content.TranslationX = targetX.Clamp(-Content.Width * (_currentScale - 1), 0);
                Content.TranslationY = targetY.Clamp(-Content.Height * (_currentScale - 1), 0);

                // Apply scale factor.
                Content.Scale = _currentScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                // Store the translation delta's of the wrapped user interface element.
                _xOffset = Content.TranslationX;
                _yOffset = Content.TranslationY;
                IsScaled = true;
            }
        }
    
    }
}
