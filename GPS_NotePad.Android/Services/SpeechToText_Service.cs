

using GPS_NotePad.Services.TranslatorInterfaces;
using GPS_NotePad.Services.Interfaces;

using Xamarin.Forms;

using System.Collections.Generic;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using AndroidX.AppCompat.App;


namespace GPS_NotePad.Droid.Services
{
    class SpeechToText_Service : AppCompatActivity, IRecognitionListener, ISpeechToText_Service, IMessageSender
    {

        private SpeechRecognizer _speech;
        private Intent _speechIntent;


        #region Private Helper

        private void MessageToPCL(Bundle results)
        {
            IList<string> matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);

            if (matches == null)
            {
                MessagingCenter.Send<IMessageSender, string>(this, "STT", null);
            }
            else
            {
                if (matches.Count != 0)
                {
                    MessagingCenter.Send<IMessageSender, string>(this, "STT", matches[0]);
                }
            }
        }

        #endregion


        #region Intarfaces implementation

        public void SpeechToText()
        {
            _speechIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            _speech = SpeechRecognizer.CreateSpeechRecognizer(MainActivity.Instance);
            _speech.SetRecognitionListener(this);

            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            _speechIntent.PutExtra(RecognizerIntent.ActionRecognizeSpeech, RecognizerIntent.ExtraPreferOffline);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 10000);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 10000);
            _speechIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 30000);
            _speechIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);

            _speech.StartListening(_speechIntent);
        }

        public void StopListening()
        {
            _speech.StopListening();
        }

        #endregion


        #region Interfaces implamentation

        public void OnBeginningOfSpeech()
        {

        }

        public void OnBufferReceived(byte[] buffer)
        {

        }

        public void OnEndOfSpeech()
        {

        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {

        }

        public void OnEvent(int eventType, Bundle @params)
        {

        }

        public void OnPartialResults(Bundle partialResults)
        {
            MessageToPCL(partialResults);
        }

        public void OnReadyForSpeech(Bundle @params)
        {

        }

        public void OnResults(Bundle results)
        {
            MessageToPCL(results);
        }

        public void OnRmsChanged(float rmsdB)
        {

        }

        #endregion

    }
}