using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Xamarin.Forms.Platform.Android;

namespace GPS_NotePad.Droid.Effects
{
    class EntryUnderlineColor_Effect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Android.Graphics.Color borderColor = Android.Graphics.Color.Transparent;// ParseColor("#FF4081");
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                Control.BackgroundTintList = ColorStateList.ValueOf(borderColor);
            else
                Control.Background.SetColorFilter(borderColor, PorterDuff.Mode.SrcOut);
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
        }

        protected override void OnDetached() { }
    }
}