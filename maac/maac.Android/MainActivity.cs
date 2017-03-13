using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace maac.Droid
{
    [Activity(Label = "maac", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Toast startSearchToast ;
        EditText searchTerm;
        GridView tweetViewer;
        public static SocialService twitterServices = new SocialService();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            startSearchToast = Toast.MakeText(this, "Now searching", ToastLength.Short);
            
            tweetViewer = FindViewById<GridView>(Resource.Id.tweetView);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //tweetViewer.NumColumns = 1;
            // Get our button from the layout resource,
            // and attach an event to it
            tweetViewer = FindViewById<GridView>(Resource.Id.tweetView);
            ImageButton searchButton = FindViewById<ImageButton>(Resource.Id.searchButton);
            searchTerm = FindViewById<EditText>(Resource.Id.searchTerm);
            searchButton.Click += B_Click;


        }

        private async void B_Click(object sender, EventArgs e)
        {
            TweetsCollection tweetsToDisplay = await twitterServices.search(searchTerm.Text);
            TweetItemAdapter adapter =new TweetItemAdapter(this, Resource.Layout.listitem, tweetsToDisplay.searchResults.ToList<Tweet>());
            startSearchToast.Show();
            tweetViewer.Adapter=adapter;

        }
    }

}


