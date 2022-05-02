

using GPS_NotePad.Services.TranslatorInterfaces;

using Android;
using Android.OS;
using AndroidX.Core.App;
using Android.Content.PM;

using Google.Android.Material.Snackbar;

using System.Threading.Tasks;


namespace GPS_NotePad.Droid.Services
{
    class MicrophoneService : IMicrophoneService
    {

        public const int RECORDAUDIOPERMISHION_CODE = 1;
        private TaskCompletionSource<bool> _tcsPermissions;
        private string[] _permissions = new string[] { Manifest.Permission.RecordAudio };

        public Task<bool> GetPermissionAsync()
        {
            _tcsPermissions = new TaskCompletionSource<bool>();

            if ((int)Build.VERSION.SdkInt < 23)
            {
                _tcsPermissions.TrySetResult(true);
            }
            else
            {
                MainActivity currentActivity = MainActivity.Instance;

                if (AndroidX.Core.Content.ContextCompat.CheckSelfPermission(currentActivity, Manifest.Permission.RecordAudio) 
                    != (int)Permission.Granted)
                {
                    RequestMicPermissions();
                }
                else
                {
                    _tcsPermissions.TrySetResult(true);
                }

            }

            return _tcsPermissions.Task;
        }

        public void OnRequestPermissionResult(bool isGranted)
        {
            _tcsPermissions.TrySetResult(isGranted);
        }

        private void RequestMicPermissions()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(MainActivity.Instance, Manifest.Permission.RecordAudio))
            {
                Snackbar.Make(MainActivity.Instance.FindViewById(Android.Resource.Id.Content),
                        "Microphone permissions are required for speech transcription!",
                        BaseTransientBottomBar.LengthIndefinite)
                        .SetAction("Ok", v =>
                        {
                            MainActivity.Instance.RequestPermissions(_permissions, RECORDAUDIOPERMISHION_CODE);
                        })
                        .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(MainActivity.Instance, _permissions, RECORDAUDIOPERMISHION_CODE);
            }
        }

    }
}