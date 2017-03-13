
using Android.Content;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using Android;
using Java.Net;
using Android.Graphics;
using Java.Lang;

namespace maac.Droid
{
    public class TweetItemAdapter : ArrayAdapter<Tweet>
    {
        private List<Tweet> tweets;
        public TweetItemAdapter(Context context, int textViewResourceId, List<Tweet> tweets) :
            base(context, textViewResourceId, tweets)
        {
            this.tweets = tweets;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View returnView = convertView;
            if (returnView == null)
            {
                LayoutInflater vi = (LayoutInflater)base.Context.GetSystemService(Context.LayoutInflaterService);
                returnView = vi.Inflate(Resource.Layout.listitem, null);
            }

            Tweet tweet = tweets[position];
            if (tweet != null)
            {
                TextView message = (TextView)returnView.FindViewById(Resource.Id.message);
                TextView username = (TextView)returnView.FindViewById(Resource.Id.username);

                ImageView image = (ImageView)returnView.FindViewById(Resource.Id.avatar);
                if (message != null)
                {
                    message.Text = tweet.text;
                }
                if (username != null)
                {
                    username.Text = tweet.user.name;
                }

                if (image != null)
                {
                    image.SetImageBitmap(GetBitmap(tweet.user.imageUrl));
                }
            }
            return returnView;


        }
        private Bitmap GetBitmap(string bitmapUrl)
        {
            try
            {
                URL url = new URL(bitmapUrl);
                return BitmapFactory.DecodeStream(url.OpenConnection().InputStream);
            }
            catch (Exception ex) { return null; }
        }
    }
}