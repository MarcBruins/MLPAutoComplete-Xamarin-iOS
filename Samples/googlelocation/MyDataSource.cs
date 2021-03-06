﻿using Foundation;
using MLPAutoComplete;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace googlelocation
{
	public class MyDataSource : IMLPAutoCompleteTextFieldDataSource
	{
		private MapsPlaceAutoComplete autoCompleteMaps = new MapsPlaceAutoComplete();

		#region MLPAutoCompleteTextFieldDataSource implementation

		public async Task AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString, Action<IEnumerable> completionHandler)
		{
			var result = await autoCompleteMaps.AutoComplete (textField.Text);
			var strings = result.Select (rs => rs.description).ToList ();
			completionHandler (strings);
		}

		#endregion
	}
}

