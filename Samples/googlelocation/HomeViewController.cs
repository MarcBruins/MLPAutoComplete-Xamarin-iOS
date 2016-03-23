using System;

using UIKit;
using MLPAutoComplete;
using System.Collections.Generic;

namespace googlelocation
{
	public partial class HomeViewController : UIViewController
	{
		public HomeViewController () : base ("HomeViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			var field = autoTextField;
		
			field.Setup (new MyDataSource (), false);


		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

	}
}


