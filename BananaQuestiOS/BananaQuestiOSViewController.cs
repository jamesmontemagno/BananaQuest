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
			Banana2.Image = noBanana;
			Banana3.Image = noBanana;
			Banana1.Hidden = true;
			Banana2.Hidden = true;
			Banana3.Hidden = true;

			for (int i = 0; i < viewModel.Phase.HiddenBananas.Count; i++) {
				switch (i) {
				case 0:
					Banana1.Hidden = false;
					break;
				case 1:
					Banana2.Hidden = false;
					break;
				case 2:
					Banana3.Hidden = false;
					break;
				}
			}

			InvokeOnMainThread (() => {
				MainImage.LoadUrl (viewModel.Phase.Clue.Image);
				MainText.Text = viewModel.Phase.Clue.Message;
			});

			manager.StartRangingBeacons (beaconRegion);
		}

		void UpdateBananas()
		{
			InvokeOnMainThread (() => {
				for (int i = 0; i < viewModel.Phase.HiddenBananas.Count; i++) {
					if(!viewModel.Phase.HiddenBananas[i].Found)
						continue;

					switch (i) {
					case 0:
						Banana1.Image = banana;
						break;
					case 1:
						Banana2.Image = banana;
						break;
					case 2:
						Banana3.Image = banana;
						break;
					}
				}
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

