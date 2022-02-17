
using System.ComponentModel;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace GPS_NotePad.iOS.Effects
{
    class EntryUnderlineColor_Effect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.Layer.BorderWidth = 0;           
            UITextField textField = (UITextField)this.Control;
            textField.BorderStyle = UITextBorderStyle.None;
           // this.Control.Layer.BorderColor = UIColor.FromName("#FF4081").CGColor;
        }

        protected override void OnDetached() { }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
        }
    }
}