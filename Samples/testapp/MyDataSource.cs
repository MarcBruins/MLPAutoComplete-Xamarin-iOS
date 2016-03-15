using System;
using Foundation;
using MLPAutoComplete;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace testapp
{
	public class MyDataSource : IMLPAutoCompleteTextFieldDataSource
	{
		IEnumerable<string> countries = new List<string>(){"Abkhazia","Afghanistan","Aland","Albania","Algeria"};

		#region MLPAutoCompleteTextFieldDataSource implementation

		public async Task AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString, Action<IEnumerable> completionHandler)
		{
			completionHandler (countries);
		}

		public async Task<string[]> AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

