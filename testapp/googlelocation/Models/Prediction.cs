using System;
using System.Collections.Generic;

namespace googlelocation
{
	public class Prediction
	{
		public String description {
			get;
			set;
		}

		public String id {
			get;
			set;
		}


		public List<MatchedSubStrings> matched_substrings {
			get;
			set;
		}

		public String place_id {
			get;
			set;
		}

		public String reference {
			get;
			set;
		}

		public List<Terms> terms {
			get;
			set;
		}

		public List<String> types {
			get;
			set;
		}


	}


	public class MatchedSubStrings{
		public int length {
			get;
			set;
		}

		public int offset {
			get;
			set;
		}

		public MatchedSubStrings(){}
	}


	public class Terms{
		public int offset {
			get;
			set;
		}

		public String value {
			get;
			set;
		}

		public Terms(){}
	}


}

