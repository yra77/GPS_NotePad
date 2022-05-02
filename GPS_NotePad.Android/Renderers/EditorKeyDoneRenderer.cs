
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using GPS_NotePad.Controls;
using GPS_NotePad.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Widget.TextView;


[assembly: ExportRenderer(typeof(Editor_KeyDoneRenderer), typeof(EditorKeyDoneRenderer))]
namespace GPS_NotePad.Droid.Renderers
{
    class EditorKeyDoneRenderer : EditorRenderer, IOnEditorActionListener
    {
        public EditorKeyDoneRenderer(Context context) : base(context) { }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            InputMethodManager imm = InputMethodManager.FromContext(MainActivity.Instance.ApplicationContext);

            _ = imm.HideSoftInputFromWindow(MainActivity.Instance.Window.DecorView.WindowToken, HideSoftInputFlags.NotAlways);

            Control.ClearFocus();

            return true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            Control.ImeOptions = Android.Views.InputMethods.ImeAction.Done;
            Control.InputType = Android.Text.InputTypes.ClassText;
            Control.SetOnEditorActionListener(this);
        }

    }
}