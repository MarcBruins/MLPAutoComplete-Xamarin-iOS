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

			List<Dictionary<int,Object>> editDistances = new List<Dictionary<int,Object>> ();
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

				Dictionary<int,Object> stringsWithEditDistances = new Dictionary<int,Object>  ();
				stringsWithEditDistances.Add(1,currentString);
				stringsWithEditDistances.Add(2, completion);
				stringsWithEditDistances.Add(3, editDistanceOfCurrentString);

				editDistances.Add (stringsWithEditDistances);
			}
				
			editDistances.Sort ((Dictionary<int, object> x, Dictionary<int, object> y) => {
				var first = (int) x[3];
				var second = (int) y[3];
				return first.CompareTo(second);
			});

			List<Object> otherSuggestions = new List<Object> ();
			List<Object> prioritySuggestions = new List<Object> ();

			foreach(var stringWithEditDistances in editDistances)
			{
				Object autoCompleteObject = stringWithEditDistances [1];
				string suggestedString = (string)stringWithEditDistances [2];

				var suggestedStringComponents = suggestedString.Split (null);
				bool suggestedStringDeservesPriority = false;

				foreach (var component in suggestedStringComponents) {

					if(inputString.Length != 0 && (component.ToLower ().IndexOf(inputString.ToLower ()) == 0)){
						suggestedStringDeservesPriority = true;
						prioritySuggestions.Add (autoCompleteObject);
						break;
					}

					if (inputString.Length <= 1)
						break;
				}

				if (!suggestedStringDeservesPriority)
					otherSuggestions.Add (autoCompleteObject);

			}

			var result = new List<Object>();
			result.AddRange (prioritySuggestions);
			result.AddRange (otherSuggestions);
			return result;

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
			
			return null;
		}
	}
}
