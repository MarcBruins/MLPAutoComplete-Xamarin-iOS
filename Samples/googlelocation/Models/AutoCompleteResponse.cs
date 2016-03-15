using System;
using System.Collections.Generic;

namespace googlelocation
{
	public class AutoCompleteResponse
	{
		public List<Prediction> Predictions { get; set;}
		public String ResponseStatus { get; set;}

		public AutoCompleteResponse (){}
	}
}

