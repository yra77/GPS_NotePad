

using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Content;
using Android.Views.InputMethods;

using System;
using System.ComponentModel;

using Xamarin.Forms.Platform.Android;


namespace GPS_NotePad.Droid.Effects
{
    class EntryUnderlineColor_Effect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Android.Graphics.Color borderColor = Android.Graphics.Color.Transparent;// ParseColor("#495CDD");

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Control.BackgroundTintList = ColorStateList.ValueOf(borderColor);
            }
            else
            {
                Control.Background.SetColorFilter(borderColor, PorterDuff.Mode.SrcOut);
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
        }

        protected override void OnDetached() { }

    }
}