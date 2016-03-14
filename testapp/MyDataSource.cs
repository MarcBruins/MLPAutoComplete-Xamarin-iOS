using System;
using Foundation;
using MLPAutoComplete;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace testapp
{
	public class MyDataSource : NSObject, MLPAutoCompleteTextFieldDataSource
	{
		IEnumerable<string> countries = new List<string>(){"Abkhazia","Afghanistan","Aland","Albania","Algeria"};

		#region MLPAutoCompleteTextFieldDataSource implementation

		public bool RespondsToSelector (ObjCRuntime.Selector sel)
		{
			return base.RespondsToSelector (sel);
		}

		public void AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString, Action<IEnumerable> completionHandler)
		{
			completionHandler (countries);
		}

		public void AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString, Delegate completionHandler)
		{
			//completionHandler
		}
		public string[] AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

