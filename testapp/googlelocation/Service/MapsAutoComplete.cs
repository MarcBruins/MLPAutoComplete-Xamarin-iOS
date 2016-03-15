using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace googlelocation
{
	public class MapsPlaceAutoComplete
	{
		private const String BASE_URL = "https://maps.googleapis.com/maps/api/place/autocomplete/";
		private const String API_KEY = Configuration.MapsAPIKey;
		private const String TYPE = "json?";

		HttpClient client;

		public List<Prediction> Items { get; private set; }

		public MapsPlaceAutoComplete ()
		{
			client = new HttpClient ();
		}


		public async Task<List<Prediction>> AutoComplete(string word)
		{
			var uri = BASE_URL + TYPE + "input=" + word + "&key="+ API_KEY;

			try {
				var response = await client.GetAsync (uri);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					var result = JsonConvert.DeserializeObject <AutoCompleteResponse> (content);
					Items = result.Predictions;
				}
			}catch (Exception ex) {
				throw ex;
			}

			return Items;
		}
	}
}

