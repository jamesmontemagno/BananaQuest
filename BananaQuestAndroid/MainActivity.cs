using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using EstimoteSdk;
using Java.Util.Concurrent;
using BananaQuest.Shared;
using BananaQuestAndroid;

namespace BeaconsAndroid
{
	[Activity (Label = "Banana Quest!", MainLauncher = true)]
	public class MainActivity : Activity, BeaconManager.IServiceReadyCallback
	{

		BeaconManager beaconManager;
		Region region;
		PhaseViewModel viewModel = new PhaseViewModel();
		ImageView Banana1, Banana2, Banana3, MainImage;
		TextView MainText;
		ProgressBar progressBar;
		const string beaconId ="com.refractored";
		protected async override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);
			FileCache.SaveLocation = CacheDir.AbsolutePath;


			viewModel.PropertyChanged += HandlePropertyChanged;

			Banana1 = FindViewById<ImageView> (Resource.Id.banana1);
			MainImage = FindViewById<ImageView> (Resource.Id.main_image);
			MainText = FindViewById<TextView> (Resource.Id.main_text);
			progressBar = FindViewById<ProgressBar> (Resource.Id.progressBar);


			beaconManager = new BeaconManager(this);


			// Default values are 5s of scanning and 25s of waiting time to save CPU cycles.
			// In order for this demo to be more responsive and immediate we lower down those values.
			beaconManager.SetBackgroundScanPeriod(TimeUnit.Seconds.ToMillis(3), 0);

			beaconManager.Ranging += (sender, e) => {
				if(e.Beacons == null)
					return;

				foreach(var beacon in e.Beacons)
				{
					var proximity = Utils.ComputeProximity(beacon);
					if(proximity != Utils.Proximity.Immediate)
						continue;

					if(!viewModel.CheckBanana(beacon.Major, beacon.Minor))
						continue;

					UpdateBananas();
				}
			};

			Banana1.Visibility = ViewStates.Invisible;
			Banana2.Visibility = ViewStates.Invisible;
			Banana3.Visibility = ViewStates.Invisible;

			progressBar.Visibility = ViewStates.Visible;

			await viewModel.GetPhaseAsync ();

			progressBar.Visibility = ViewStates.Invisible;

			InitializeBananas();
			UpdateBananas ();
		}

		void InitializeBananas()
		{
			region = new Region(beaconId,
				viewModel.Phase.UUID,
				new Java.Lang.Integer(viewModel.Phase.Major), 
				null);

			Banana1.SetImageResource (Resource.Drawable.ic_no_banana);

			Banana1.Visibility = ViewStates.Visible;	

			RunOnUiThread (() => {
				MainImage.SetImageFromUrlAsync (viewModel.Phase.Clue.Image);
				MainText.Text = viewModel.Phase.Clue.Message;
			});

			beaconManager.Connect (this);
		}

		void UpdateBananas()
		{
			RunOnUiThread (() => {

					if(viewModel.Phase.HiddenBanana.Found)
						Banana1.SetImageResource (Resource.Drawable.ic_banana);

			});
		}

		void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName) {
			case "PhaseComplete":
				if (viewModel.PhaseComplete) {
					DisplayMessage ("Congratulations! You have found all the bananas!", "You Win!");
					RunOnUiThread (() => {
						MainImage.SetImageFromUrlAsync (viewModel.Phase.Prize.Image);
						MainText.Text = viewModel.Phase.Prize.Message;
					});
				}
				break;
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			if(region != null)
				beaconManager.Connect(this);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			if(region != null)
				beaconManager.Disconnect ();
		}

		public void OnServiceReady()
		{
			if (region == null)
				return;
			// This method is called when BeaconManager is up and running.
			beaconManager.StartRanging(region);
		}

		protected override void OnDestroy()
		{
			// Make sure we disconnect from the Estimote.
			beaconManager.Disconnect();
			base.OnDestroy();
		}

		private void DisplayMessage(string message, string title)
		{
			RunOnUiThread (() => {
				var builder = new AlertDialog.Builder (this);
				builder.SetMessage (message)
					.SetTitle (title ?? string.Empty)
					.SetPositiveButton ("OK", delegate {
					});
				var dialog = builder.Create ();
				dialog.Show ();
			});
		}
	}
}


