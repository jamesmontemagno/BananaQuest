// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace BananaQuestiOS
{
	[Register ("BananaQuestiOSViewController")]
	partial class BananaQuestiOSViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView Banana1 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView MainImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel MainText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIActivityIndicatorView ProgressBar { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (Banana1 != null) {
				Banana1.Dispose ();
				Banana1 = null;
			}
			if (MainImage != null) {
				MainImage.Dispose ();
				MainImage = null;
			}
			if (MainText != null) {
				MainText.Dispose ();
				MainText = null;
			}
			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}
		}
	}
}
