using System;
using Foundation;
using MLPAutoComplete;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace googlelocation
{
	public class MyDataSource : IMLPAutoCompleteTextFieldDataSource
	{
		//IEnumerable<string> countries = new List<string>(){"Abkhazia","Afghanistan","Aland","Albania","Algeria"};
		private MapsPlaceAutoComplete autoCompleteMaps = new MapsPlaceAutoComplete();

		#region MLPAutoCompleteTextFieldDataSource implementation

		public async Task AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString, Action<IEnumerable> completionHandler)
		{
			var result = await autoCompleteMaps.AutoComplete (textField.Text);
			var strings = result.Select (rs => rs.description).ToList ();
			completionHandler (strings);
		}

		public Task<string[]> AutoCompleteTextField (MLPAutoCompleteTextField textField, string possibleCompletionsForString)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

