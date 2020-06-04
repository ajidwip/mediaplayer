using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;


namespace MediaPlayer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button btn;
        static ISharedPreferences sharedPreferences;
        static string typebrowser = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            sharedPreferences = this.GetSharedPreferences("sharedprefrences", 0);

            ISharedPreferencesEditor param = sharedPreferences.Edit();
            param.PutString("url", "https://live.cnnindonesia.com/livecnn/smil:cnntv.smil/playlist.m3u8");
            param.PutString("typestream", "HLS");
            param.Commit();

            typebrowser = "STREAM";

            btn = FindViewById<Button>(Resource.Id.btn);

            btn.Click += delegate
            {
                if (typebrowser == "STREAM")
                {
                    Intent i = new Intent(this, typeof(Player));
                    StartActivity(i);
                }
                else
                {
                    Intent i = new Intent(this, typeof(PlayerBrowser));
                    StartActivity(i);
                }
            };
        }
    }
}

