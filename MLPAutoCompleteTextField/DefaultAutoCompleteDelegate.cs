using System;
using UIKit;
using System.Diagnostics;

namespace MLPAutoComplete
{
	class DefaultAutoCompleteDelegate : IMLPAutoCompleteTextFieldDelegate
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
			Debug.WriteLine ("AutoCompleteTextField method called in delegate");
		}
		public void WillShowAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
		{
			Debug.WriteLine ("Autocomplete table view will be added to the view hierarchy");
		}
		public void DidShowAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
		{
			Debug.WriteLine ("Autocomplete table view is showing from the view hierarchy");
		}
		public void WillHideAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
		{
			Debug.WriteLine ("Autocomplete table view will be removed from the view hierarchy");
		}
		public void DidHideAutoCompleteTableView (MLPAutoCompleteTextField textField, UITableView autoCompleteTableView)
		{
			Debug.WriteLine ("Autocomplete table view is removed from the view hierarchy");
		}
		public void DidChangeNumberOfSuggestions (MLPAutoCompleteTextField textField, int numberOfSuggestions)
		{
			Debug.WriteLine ("Did change number of suggestions");
		}
		#endregion
	}
}

