using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using static Android.Views.ViewTreeObserver;

namespace MediaPlayer
{
    [Activity(Label = "Player", Theme = "@style/AppTheme.NoActionBar")]
    public class Player : Activity
    {
        private PlayerView playerView;
        private SimpleExoPlayer player;
        public IMediaSource extractorMediaSource;
        ISharedPreferences sharedPreferences;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.player_activity);

            if (savedInstanceState == null)
            {
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

                // clear FLAG_TRANSLUCENT_STATUS flag:
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);

                RequestedOrientation = ScreenOrientation.Landscape;

                this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                //this.Window.AddFlags(WindowManagerFlags.Fullscreen);
                //this.Window.ClearFlags(WindowManagerFlags.Fullscreen);

                decorview = Window.DecorView;
                CoordinatorLayout root = FindViewById<CoordinatorLayout>(Resource.Id.rootView);
                screenheight = root.Height;
                root.ViewTreeObserver.AddOnGlobalLayoutListener(new mOnGlobalLayoutListener());

                InitializePlayer();

            }
            else
            {
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

                // clear FLAG_TRANSLUCENT_STATUS flag:
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);

                RequestedOrientation = ScreenOrientation.Landscape;

                this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                //this.Window.AddFlags(WindowManagerFlags.Fullscreen);
                //this.Window.ClearFlags(WindowManagerFlags.Fullscreen);

                decorview = Window.DecorView;
                CoordinatorLayout root = FindViewById<CoordinatorLayout>(Resource.Id.rootView);
                screenheight = root.Height;
                root.ViewTreeObserver.AddOnGlobalLayoutListener(new mOnGlobalLayoutListener());
                playerView = (PlayerView)FindViewById(Resource.Id.player_view);
                playerView.RequestFocus();

                sharedPreferences = this.GetSharedPreferences("sharedprefrences", 0);

                var url = sharedPreferences.GetString("url", null);
                var type = sharedPreferences.GetString("typestream", null);

                playVideo(url, type);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            player.Stop();
        }
        public void InitializePlayer()
        {
            var userAgent = Util.GetUserAgent(this, "Player");
            var defaultHttpDataSourceFactory = new DefaultHttpDataSourceFactory(userAgent);
            var defaultBandwidthMeter = new DefaultBandwidthMeter();
            var adaptiveTrackSelectionFactory = new AdaptiveTrackSelection.Factory(defaultBandwidthMeter);
            var defaultTrackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);

            player = ExoPlayerFactory.NewSimpleInstance(this, defaultTrackSelector);
        }
        public void playVideo(string url, string typestream)
        {
            var MyURL = url;
            var mediaUri = Android.Net.Uri.Parse(MyURL);

            var userAgent = Util.GetUserAgent(this, "Player");
            var defaultHttpDataSourceFactory = new DefaultHttpDataSourceFactory(userAgent);
            var defaultDataSourceFactory = new DefaultDataSourceFactory(this, null, defaultHttpDataSourceFactory);
            if (typestream == "HLS")
            {
                playerView.UseController = false;
                extractorMediaSource = new HlsMediaSource.Factory(defaultDataSourceFactory).CreateMediaSource(mediaUri);
            }
            else if (typestream == "MP4")
            {
                extractorMediaSource = new ExtractorMediaSource(mediaUri, defaultDataSourceFactory, new DefaultExtractorsFactory(), null, null);
            }
            ConcatenatingMediaSource concatenatedSource =
            new ConcatenatingMediaSource(extractorMediaSource);
            var defaultBandwidthMeter = new DefaultBandwidthMeter();
            var adaptiveTrackSelectionFactory = new AdaptiveTrackSelection.Factory(defaultBandwidthMeter);
            var defaultTrackSelector = new DefaultTrackSelector(adaptiveTrackSelectionFactory);

            player = ExoPlayerFactory.NewSimpleInstance(this, defaultTrackSelector);

            player.Prepare(concatenatedSource);

            player.AddListener(new CustomListener());

            playerView.Player = player;

            player.PlayWhenReady = true;
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
        public class CustomListener : Java.Lang.Object, IPlayerEventListener
        {
            public CustomListener()
            {
                //Bring in any variables needed and implement the functions for the listener in this class
            }

            public void OnLoadingChanged(bool p0)
            {
                Console.Write(p0);
            }

            public void OnPlayerError(ExoPlaybackException p0)
            {
                Console.Write(p0);
            }

            public void OnPlayerStateChanged(bool p0, int p1)
            {
                Console.Write(p0);
            }

            public void OnPositionDiscontinuity()
            {

            }

            public void OnTimelineChanged(Timeline p0, Java.Lang.Object p1)
            {
                Console.Write(p0);
            }

            public void OnTracksChanged(TrackGroupArray p0, TrackSelectionArray p1)
            {
                Console.Write(p0);
            }
        }
    }
}