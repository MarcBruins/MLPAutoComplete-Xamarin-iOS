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
			field.autoCompleteDataSource = new MyDataSource ();
			field.autoCompleteDelegate = new autoCompleteDelegate();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}


		class autoCompleteDelegate : MLPAutoCompleteTextFieldDelegate
		{
			public bool RespondsToSelector (ObjCRuntime.Selector sel)
			{
				return this.RespondsToSelector(sel);
			}

			#region MLPAutoCompleteTextFieldDelegate implementation
			public bool ShouldStyleAutoCompleteTableView (MLPAutoCompleteTextField AutoCompleteTextField, UITableView autoCompleteTableView, UITextBorderStyle borderStyle)
			{
				throw new NotImplementedException ();
			}
			public bool ShouldConfigureCell (MLPAutoCompleteTextField AutoCompleteTextField, UITableViewCell cell, string autoCompleteString, MLPAutoCompletionObject autoObject, Foundation.NSIndexPath NSIndexPath)
			{
				throw new NotImplementedException ();
			}
			public void AutoCompleteTextField (MLPAutoCompleteTextField textField, string selectedString, MLPAutoCompletionObject selectedObject, Foundation.NSIndexPath indexPath)
			{
				throw new NotImplementedException ();
			}
			public void WillShowAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
			{
				throw new NotImplementedException ();
			}
			public void DidShowAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
			{
				throw new NotImplementedException ();
			}
			public void WillHideAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
			{
				throw new NotImplementedException ();
			}
			public void DidHideAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
			{
				throw new NotImplementedException ();
			}
			public void DidChangeNumberOfSuggestions (MLPAutoCompleteTextField textField, int numberOfSuggestions)
			{
				throw new NotImplementedException ();
			}
			#endregion
			
		}
	}





}


