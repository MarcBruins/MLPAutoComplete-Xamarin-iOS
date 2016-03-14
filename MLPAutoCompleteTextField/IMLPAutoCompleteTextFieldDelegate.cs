using System;
using UIKit;
using Foundation;
using ObjCRuntime;

namespace MLPAutoComplete
{
	public interface IMLPAutoCompleteTextFieldDelegate
	{

		/*IndexPath corresponds to the order of strings within the autocomplete table,
 not the original data source.*/
		 bool ShouldStyleAutoCompleteTableView(MLPAutoCompleteTextField AutoCompleteTextField, UITableView autoCompleteTableView, UITextBorderStyle borderStyle);
	
		 bool ShouldConfigureCell(MLPAutoCompleteTextField AutoCompleteTextField, UITableViewCell cell, string autoCompleteString, MLPAutoCompletionObject autoObject, NSIndexPath NSIndexPath);


		/*IndexPath corresponds to the order of strings within the autocomplete table,
		not the original data source.
		 autoCompleteObject may be nil if the selectedString had no object associated with it.
		 */
		 void AutoCompleteTextField(MLPAutoCompleteTextField textField, String selectedString, MLPAutoCompletionObject selectedObject, NSIndexPath indexPath);

		 void WillShowAutoCompleteTableView(MLPAutoCompleteTextField textField, UITableView autoCompleteTableView);

		 void DidShowAutoCompleteTableView(MLPAutoCompleteTextField textField, UITableView autoCompleteTableView);

		 void WillHideAutoCompleteTableView(MLPAutoCompleteTextField textField, UITableView autoCompleteTableView);

		 void DidHideAutoCompleteTableView(MLPAutoCompleteTextField textField, UITableView autoCompleteTableView);

		 void DidChangeNumberOfSuggestions(MLPAutoCompleteTextField textField, int numberOfSuggestions);

	}
}

