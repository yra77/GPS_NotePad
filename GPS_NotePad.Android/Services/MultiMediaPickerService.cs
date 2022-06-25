
using GPS_NotePad.Services.MediaService;
using GPS_NotePad.Models;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Widget;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;


namespace GPS_NotePad.Droid.Services
{
    class MultiMediaPickerService : IMultiMediaPicker
    {

        public static MultiMediaPickerService SharedInstance;

        private const string TEMPDIRNAME = "TmpMedia";
        private int _multiPickerResultCode; 
        private TaskCompletionSource<IList<MediaFile>> _mediaPickedTcs;

        public event EventHandler<IList<MediaFile>> OnMediaPickedCompleted;


        public MultiMediaPickerService()
        {
            if (SharedInstance == null)
            {
                SharedInstance = this;
            }

            _multiPickerResultCode = 9793;
        }


        private MediaFile CreateMediaFileFromUri(Android.Net.Uri uri)
        {
            MediaFile mediaFile = null;

            string path = GetRealPathFromURI(uri);

            if (path != null)
            {
                string fileName = System.IO.Path.GetFileName(path);

                mediaFile = new MediaFile()
                {
                    Path = path,
                    FileName = fileName
                };

            }

            return mediaFile;
        }

        private async Task<IList<MediaFile>> PickMediaAsync(string type, string title, int resultCode)
        {

            _mediaPickedTcs = new TaskCompletionSource<IList<MediaFile>>();

            Intent imageIntent = new Intent(Intent.ActionPick);

            imageIntent.SetType(type);
            imageIntent.PutExtra(Intent.ExtraAllowMultiple, true);

            MainActivity.Instance.StartActivityForResult(Intent.CreateChooser(imageIntent, title), resultCode);

            return await _mediaPickedTcs.Task;

        }

        private string GetRealPathFromURI(Android.Net.Uri contentURI)
        {

            ICursor cursor = null;

            try
            {
                string mediaPath = string.Empty;

                cursor = MainActivity.Instance.ContentResolver.Query(contentURI, null, null, null, null);
                cursor.MoveToFirst();

                int idx = cursor.GetColumnIndex(MediaStore.MediaColumns.Data);

                if (idx != -1)
                {
                    string type = MainActivity.Instance.ContentResolver.GetType(contentURI);

                    int pIdx = cursor.GetColumnIndex(MediaStore.MediaColumns.Id);

                    string mData = cursor.GetString(idx);

                    mediaPath = mData;

                }
                else
                {

                    string docID = DocumentsContract.GetDocumentId(contentURI);
                    string[] doc = docID.Split(':');
                    string id = doc[1];
                    string whereSelect = MediaStore.Images.ImageColumns.Id + "=?";
                    string dataConst = MediaStore.Images.ImageColumns.Data;
                    string[] projections = new string[] { dataConst };
                    Android.Net.Uri internalUri = MediaStore.Images.Media.InternalContentUri;
                    Android.Net.Uri externalUri = MediaStore.Images.Media.ExternalContentUri;

                    switch (doc[0])
                    {

                        case "image":
                            whereSelect = MediaStore.Video.VideoColumns.Id + "=?";
                            projections = new string[] { MediaStore.Video.VideoColumns.Data };
                            break;
                    }

                    projections = new string[] { dataConst };
                    cursor = MainActivity.Instance.ContentResolver.Query(internalUri, projections, whereSelect, new string[] { id }, null);

                    if (cursor.Count == 0)
                    {
                        cursor = MainActivity.Instance.ContentResolver.Query(externalUri, projections, whereSelect, new string[] { id }, null);
                    }

                    int colDatax = cursor.GetColumnIndexOrThrow(dataConst);

                    cursor.MoveToFirst();

                    mediaPath = cursor.GetString(colDatax);
                }
                return mediaPath;
            }
            catch (Exception)
            {
                Toast.MakeText(MainActivity.Instance, "Unable to get path", ToastLength.Long).Show();

            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                    cursor.Dispose();
                }
            }

            return null;

        }


        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            ObservableCollection<MediaFile> mediaPicked = null;

            if (requestCode == _multiPickerResultCode)
            {
                if (resultCode == Result.Ok)
                {
                    mediaPicked = new ObservableCollection<MediaFile>();
                    if (data != null)
                    {
                        ClipData clipData = data.ClipData;
                        if (clipData != null)
                        {
                            for (int i = 0; i < clipData.ItemCount; i++)
                            {
                                ClipData.Item item = clipData.GetItemAt(i);
                                Android.Net.Uri uri = item.Uri;
                                MediaFile media = CreateMediaFileFromUri(uri);

                                if (media != null)
                                {
                                    mediaPicked.Add(media);
                                }

                            }
                        }
                        else
                        {
                            Android.Net.Uri uri = data.Data;
                            MediaFile media = CreateMediaFileFromUri(uri);

                            if (media != null)
                            {
                                mediaPicked.Add(media);
                            }
                        }

                        OnMediaPickedCompleted?.Invoke(this, mediaPicked);
                    }
                }

                _mediaPickedTcs?.TrySetResult(mediaPicked);

            }
        }

        public void Clean()
        {

            string documentsDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), 
                                                               TEMPDIRNAME);

            if (Directory.Exists(documentsDirectory))
            {
                Directory.Delete(documentsDirectory);
            }
        }

        public async Task<IList<MediaFile>> PickPhotosAsync()
        {
            return await PickMediaAsync("image/*", "Select Images", _multiPickerResultCode);
        }

    }
}