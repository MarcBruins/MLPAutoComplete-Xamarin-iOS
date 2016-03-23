using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MLPAutoComplete
{
	public class MLPAutoCompleteSortOperation
	{
		private MLPAutoCompleteTextField _textField;

		public MLPAutoCompleteSortOperation (MLPAutoCompleteTextField textField)
		{
			_textField = textField;
		}

		public List<Object> Sort(string inputString, List<Object> completions)
		{
			if (String.IsNullOrEmpty (inputString)) { 
				return completions;
			}

			List<Object> editDistances = new List<Object> ();
			int maximumEditDistance = _textField.MaximumEditDistance;

			foreach (var completion in completions) {

				string currentString = String.Empty;
				if (completion is string) {
					currentString = (string)completion;
				} else if (completion is MLPAutoCompletionObject) {
					currentString = ((MLPAutoCompletionObject)completion).AutocompleteString;
				} else {
					Console.WriteLine ("Autocompletion terms must either be strings or objects conforming to the MLPAutoCompleteObject protocol.");
				}

				int maxRange = (inputString.Length < currentString.Length) ? inputString.Length : currentString.Length;
				int editDistanceOfCurrentString = LevenshteinDistance.Compute (inputString, currentString);

				if (editDistanceOfCurrentString < maxRange) {
					break;
				}

				Dictionary<string,Object> ds = new Dictionary<string,Object>  ();
				ds.Add("key",currentString);
				ds.Add("key2", completion);
				ds.Add("key3", editDistanceOfCurrentString);

				editDistances.Add (ds);

				Func<string, IEnumerable<string>> indexSelector = n => new string[] {
					n,
					n
				};

				//editDistances.OrderBy (indexSelector);



				List<string> otherSuggestions,prioritySuggestions = new List<string> ();

				foreach(var stringWithEditDistances in editDistances)
				{
				//	Object autoCompleteObject = stringWithEditDistances ["kSortObjectKey"];
				//	string suggestedString = stringWithEditDistances ["kSortInputStringKey"];

					//var suggestedStringComponents = suggestedString.

				}

			return editDistances;

//
//				if(self.isCancelled){
//					return [NSArray array];
//				}
//
//				[editDistances sortUsingComparator:^(NSDictionary *string1Dictionary,
//					NSDictionary *string2Dictionary){
//
//					return [string1Dictionary[kSortEditDistancesKey]
//						compare:string2Dictionary[kSortEditDistancesKey]];
//
//				}];
//
//
//
//				NSMutableArray *prioritySuggestions = [NSMutableArray array];
//				NSMutableArray *otherSuggestions = [NSMutableArray array];
//				for(NSDictionary *stringsWithEditDistances in editDistances){
//
//					if(self.isCancelled){
//						return [NSArray array];
//					}
//
//					NSObject *autoCompleteObject = stringsWithEditDistances[kSortObjectKey];
//					NSString *suggestedString = stringsWithEditDistances[kSortInputStringKey];
//
//					NSArray *suggestedStringComponents = [suggestedString componentsSeparatedByString:@" "];
//					BOOL suggestedStringDeservesPriority = NO;
//					for(NSString *component in suggestedStringComponents){
//						NSRange occurrenceOfInputString = [[component lowercaseString]
//							rangeOfString:[inputString lowercaseString]];
//
//						if (occurrenceOfInputString.length != 0 && occurrenceOfInputString.location == 0) {
//							suggestedStringDeservesPriority = YES;
//							[prioritySuggestions addObject:autoCompleteObject];
//							break;
//						}
//
//						if([inputString length] <= 1){
//							//if the input string is very short, don't check anymore components of the input string.
//							break;
//						}
//					}
//
//					if(!suggestedStringDeservesPriority){
//						[otherSuggestions addObject:autoCompleteObject];
//					}
//
//				}
//
//				NSMutableArray *results = [NSMutableArray array];
//				[results addObjectsFromArray:prioritySuggestions];
//				[results addObjectsFromArray:otherSuggestions];
//
//
//				return [NSArray arrayWithArray:results];
			}
			return null;
		}
	}
}
