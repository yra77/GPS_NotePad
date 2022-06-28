
using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;


namespace GPS_NotePad.Controls
{
    public class PinchZoom_Control : ContentView
    {

        private const double MINSCALE = 1;
        private const double MAXSCALE = 10;
        private double _startScale;
        private double _currentScale;
        private double _xOffset;
        private double _yOffset;
        private double _startX;
        private double _startY;

        public PinchZoom_Control()
        {
            TapGestureRecognizer tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            tap.Tapped += OnTapped;
            GestureRecognizers.Add(tap);

            PinchGestureRecognizer pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            PanGestureRecognizer pan = new PanGestureRecognizer();
            pan.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(pan);
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
            get => (bool)GetValue(IsScaledProperty);
            set => SetValue(IsScaledProperty, value);
        }

        #endregion


        protected override void OnSizeAllocated(double width, double height)
        {
            RestoreScaleValues();
            Content.AnchorX = 0.5;
            Content.AnchorY = 0.5;

            base.OnSizeAllocated(width, height);
        }

        private void RestoreScaleValues()
        {
            Content.ScaleTo(MINSCALE, 250, Easing.CubicInOut);
            Content.TranslateTo(0, 0, 250, Easing.CubicInOut);

            _currentScale = MINSCALE;
            _xOffset = Content.TranslationX = 0;
            _yOffset = Content.TranslationY = 0;
        }

        private void OnTapped(object sender, EventArgs e)
        {
            if (Content.Scale > MINSCALE)
            {
                RestoreScaleValues();
            }
            else
            {
                //todo: Add tap position somehow
                StartScaling();
                ExecuteScaling(MAXSCALE, .5, .5);
                EndGesture();
            }
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    {
                        StartScaling();
                    }
                    break;

                case GestureStatus.Running:
                    {
                        ExecuteScaling(e.Scale, e.ScaleOrigin.X, e.ScaleOrigin.Y);
                    }
                    break;

                case GestureStatus.Completed:
                    {
                        EndGesture();
                    }
                    break;
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    {
                        _startX = e.TotalX;
                        _startY = e.TotalY;

                        Content.AnchorX = 0;
                        Content.AnchorY = 0;
                    }
                    break;

                case GestureStatus.Running:
                    {
                        double maxTranslationX = Content.Scale * Content.Width - Content.Width;
                        Content.TranslationX = Math.Min(0, Math.Max(-maxTranslationX, _xOffset + e.TotalX - _startX));

                        double maxTranslationY = Content.Scale * Content.Height - Content.Height;
                        Content.TranslationY = Math.Min(0, Math.Max(-maxTranslationY, _yOffset + e.TotalY - _startY));
                    }
                    break;

                case GestureStatus.Completed:
                    {
                        EndGesture();
                    }
                    break;
            }
        }

        private void StartScaling()
        {
            _startScale = Content.Scale;

            Content.AnchorX = 0;
            Content.AnchorY = 0;
            IsScaled = false;
        }

        private void ExecuteScaling(double scale, double x, double y)
        {
            _currentScale += (scale - 1) * _startScale;
            _currentScale = Math.Max(MINSCALE, _currentScale);
            _currentScale = Math.Min(MAXSCALE, _currentScale);

            double deltaX = (Content.X + _xOffset) / Width;
            double deltaWidth = Width / (Content.Width * _startScale);
            double originX = (x - deltaX) * deltaWidth;

            double deltaY = (Content.Y + _yOffset) / Height;
            double deltaHeight = Height / (Content.Height * _startScale);
            double originY = (y - deltaY) * deltaHeight;

            double targetX = _xOffset - (originX * Content.Width) * (_currentScale - _startScale);
            double targetY = _yOffset - (originY * Content.Height) * (_currentScale - _startScale);

            Content.TranslationX = targetX.Clamp(-Content.Width * (_currentScale - 1), 0);
            Content.TranslationY = targetY.Clamp(-Content.Height * (_currentScale - 1), 0);

            Content.Scale = _currentScale;
        }

        private void EndGesture()
        {
            _xOffset = Content.TranslationX;
            _yOffset = Content.TranslationY;
            IsScaled = true;
        }

    }
}
