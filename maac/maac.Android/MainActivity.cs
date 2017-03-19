using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace maac.Droid
{
    [Activity(Label = "maac", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Toast startSearchToast;
        EditText searchTerm;
        GridView tweetViewer;
        Button olderTweetsButton;
        Button newerTweetsButton;
        TweetsCollection tweetsToDisplay;
        public static TwitterConnector twitterServices = new TwitterConnector();
        protected override void OnCreate(Bundle bundle)
        {
            ActionBar.SetBackgroundDrawable(new ColorDrawable(new Color(0x08, 0x8d, 0xe1)));
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //tweetViewer.NumColumns = 1;
            // Get our button from the layout resource,
            // and attach an event to it   
            startSearchToast = Toast.MakeText(this, "Now searching", ToastLength.Short);
            olderTweetsButton = FindViewById<Button>(Resource.Id.olderTweetsButton);
            newerTweetsButton = FindViewById<Button>(Resource.Id.newerTweetsButton);
            olderTweetsButton.Click += OlderTweetsButton_Click;
            newerTweetsButton.Click += NewerTweetsButton_Click;
            tweetViewer = FindViewById<GridView>(Resource.Id.tweetView);
            tweetViewer = FindViewById<GridView>(Resource.Id.tweetView);
            ImageButton searchButton = FindViewById<ImageButton>(Resource.Id.searchButton);
            searchTerm = FindViewById<EditText>(Resource.Id.searchTerm);

            searchButton.Click += B_Click;


        }

        private async void NewerTweetsButton_Click(object sender, EventArgs e)
        {
            tweetsToDisplay = await twitterServices.search(searchTerm.Text, tweetsToDisplay.biggestTweetId, false);
            TweetItemAdapter adapter = new TweetItemAdapter(this, Resource.Layout.listitem, tweetsToDisplay.searchResults.ToList<Tweet>());

            tweetViewer.Adapter = adapter;
        }

        private async void OlderTweetsButton_Click(object sender, EventArgs e)
        {
            tweetsToDisplay = await twitterServices.search(searchTerm.Text, tweetsToDisplay.smallestTweetId, true);
            TweetItemAdapter adapter = new TweetItemAdapter(this, Resource.Layout.listitem, tweetsToDisplay.searchResults.ToList<Tweet>());

            tweetViewer.Adapter = adapter;
        }

        private async void B_Click(object sender, EventArgs e)
        {

                startSearchToast.Show();
                tweetsToDisplay = await twitterServices.search(searchTerm.Text, null, null);
                TweetItemAdapter adapter = new TweetItemAdapter(this, Resource.Layout.listitem, tweetsToDisplay.searchResults.ToList<Tweet>());

                tweetViewer.Adapter = adapter;
                olderTweetsButton.Visibility = ViewStates.Visible;
                newerTweetsButton.Visibility = ViewStates.Visible;
            




        }
    }

}


