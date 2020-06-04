using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using static Android.Views.ViewTreeObserver;

namespace MediaPlayer
{
    [Activity(Label = "PlayerBrowser", Theme = "@style/AppTheme.NoActionBar")]
    public class PlayerBrowser : Activity
    {
        private static ProgressBar spinner;
        static View itemView;
        static WebView webView;
        static string ShowOrHideWebViewInitialUse = "show";
        ISharedPreferences sharedPreferences;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.playerbrowser_activity);

            Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

            // clear FLAG_TRANSLUCENT_STATUS flag:
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

            Window.SetStatusBarColor(Android.Graphics.Color.Transparent);

            RequestedOrientation = ScreenOrientation.Landscape;

            this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            //this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            //this.Window.ClearFlags(WindowManagerFlags.Fullscreen);

            decorview = Window.DecorView;
            RelativeLayout root = FindViewById<RelativeLayout>(Resource.Id.rootView);
            screenheight = root.Height;
            root.ViewTreeObserver.AddOnGlobalLayoutListener(new mOnGlobalLayoutListener());

            spinner = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            webView = FindViewById<WebView>(Resource.Id.webview);
            webView.SetWebViewClient(new CustomWebViewClient());

            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.BuiltInZoomControls = false;
            webView.Settings.SetSupportZoom(false);
            webView.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
            webView.ScrollbarFadingEnabled = false;

            sharedPreferences = this.GetSharedPreferences("sharedprefrences", 0);

            var url = sharedPreferences.GetString("url", null);

            view(url);
        }
        public static void view(string url)
        {
            webView.LoadUrl(url);
        }

        private class CustomWebViewClient : WebViewClient
        {


            public override void OnPageStarted(WebView webview, string url, Bitmap favicon)
            {

                // only make it invisible the FIRST time the app is run
                if (ShowOrHideWebViewInitialUse == "show")
                {
                    webview.Visibility = ViewStates.Invisible;
                }
            }


            public override void OnPageFinished(WebView view, string url)
            {

                ShowOrHideWebViewInitialUse = "hide";
                spinner.Visibility = ViewStates.Gone;

                view.Visibility = ViewStates.Visible;
                base.OnPageFinished(view, url);

            }
        }
        public static int screenheight;
        public static Android.Views.View decorview;
        public class mOnGlobalLayoutListener : Java.Lang.Object, IOnGlobalLayoutListener
        {
            public void OnGlobalLayout()
            {
                Rect r = new Rect();
                var keypadHeight = screenheight - r.Bottom;
                if (keypadHeight <= screenheight * 0.15)
                {
                    var uiOptions =
                        SystemUiFlags.HideNavigation |
                        SystemUiFlags.LayoutHideNavigation |
                        SystemUiFlags.LayoutFullscreen |
                        SystemUiFlags.Fullscreen |
                        SystemUiFlags.LayoutStable |
                        SystemUiFlags.ImmersiveSticky;

                    decorview.SystemUiVisibility = (StatusBarVisibility)uiOptions;
                }
            }
        }
    }
}