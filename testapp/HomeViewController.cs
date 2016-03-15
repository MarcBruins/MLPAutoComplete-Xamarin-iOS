using System;

using UIKit;
using MLPAutoComplete;
using System.Collections.Generic;

namespace testapp
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
		
			field.AutoCompleteDataSource = new MyDataSource ();
			field.AutoCompleteDelegate = new autoCompleteDelegate();

		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}


		class autoCompleteDelegate : IMLPAutoCompleteTextFieldDelegate
		{

			#region MLPAutoCompleteTextFieldDelegate implementation
			public bool ShouldStyleAutoCompleteTableView (MLPAutoCompleteTextField AutoCompleteTextField, UITableView autoCompleteTableView, UITextBorderStyle borderStyle)
			{
				return true;
			}
			public bool ShouldConfigureCell (MLPAutoCompleteTextField AutoCompleteTextField, UITableViewCell cell, string autoCompleteString, MLPAutoCompletionObject autoObject, Foundation.NSIndexPath NSIndexPath)
			{
				return true;
			}
			public void AutoCompleteTextField (MLPAutoCompleteTextField textField, string selectedString, MLPAutoCompletionObject selectedObject, Foundation.NSIndexPath indexPath)
			{
				throw new NotImplementedException ();
			}
			public void WillShowAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
			{
				Console.WriteLine ("Autocomplete table view will be added to the view hierarchy");
			}
			public void DidShowAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
			{
				Console.WriteLine ("Autocomplete table view is showing from the view hierarchy");
			}
			public void WillHideAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
			{
				Console.WriteLine ("Autocomplete table view will be removed from the view hierarchy");
			}
			public void DidHideAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
			{
				Console.WriteLine ("Autocomplete table view is removed from the view hierarchy");
			}
			public void DidChangeNumberOfSuggestions (MLPAutoCompleteTextField textField, int numberOfSuggestions)
			{
				throw new NotImplementedException ();
			}
			#endregion
			
		}
	}





}


