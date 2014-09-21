using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BananaQuest.Shared;
using MonoTouch.CoreLocation;

namespace BananaQuestiOS
{
	public partial class BananaQuestiOSViewController : UIViewController
	{
		public BananaQuestiOSViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle
		CLBeaconRegion beaconRegion;
		const string beaconId ="com.refractored";
		CLLocationManager manager;
		PhaseViewModel viewModel = new PhaseViewModel();
		UIImage noBanana;
		UIImage banana;
		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			noBanana = UIImage.FromBundle ("ic_no_banana");
			banana = UIImage.FromBundle ("ic_banana");

			viewModel.PropertyChanged += HandlePropertyChanged;

			manager = new CLLocationManager ();

			if(UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
				manager.RequestAlwaysAuthorization ();

			manager.DidRangeBeacons += (sender, e) => {
				if(e.Beacons == null)
					return;

				foreach(var beacon in e.Beacons)
				{
					if(beacon.Proximity != CLProximity.Immediate)
						continue;

					if(!viewModel.CheckBanana(beacon.Major.Int32Value, beacon.Minor.Int32Value))
						continue;

					UpdateBananas();
				}
			};
			
			ProgressBar.StartAnimating ();

			await viewModel.GetPhaseAsync ();
			InitializeBananas ();
			ProgressBar.StopAnimating ();
		}
			
		void InitializeBananas()
		{
			beaconRegion = new CLBeaconRegion (new NSUuid (viewModel.Phase.UUID), 
				(ushort)viewModel.Phase.Major, beaconId); 

			Banana1.Image = noBanana;
			Banana1.Hidden = false;

			InvokeOnMainThread (() => {
				MainImage.LoadUrl (viewModel.Phase.Clue.Image);
				MainText.Text = viewModel.Phase.Clue.Message;
			});

			manager.StartRangingBeacons (beaconRegion);
		}

		void UpdateBananas()
		{
			InvokeOnMainThread (() => {

				if(viewModel.Phase.HiddenBanana.Found)
					Banana1.Image = banana;
			});
		}

		void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName) {
			case "PhaseComplete":
				if (viewModel.PhaseComplete) {
					new UIAlertView ("You Win!", "Congratulations! You have found all the bananas!", null, "OK").Show();
					InvokeOnMainThread (() => {
						MainImage.LoadUrl (viewModel.Phase.Prize.Image);
						MainText.Text = viewModel.Phase.Prize.Message;
					});
				}
				break;
			}
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion
	}
}

