using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
			if (String.IsNullOrEmpty (inputString)) 
				return completions;

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

				if (editDistanceOfCurrentString < maximumEditDistance) {
					break;
				}

				Dictionary<string,Object> ds = new Dictionary<string,Object>  ();
				editDistances.Add (completion);

				//editDistances.Add(
			}

			return editDistances;
//		
//
//				NSMutableArray *editDistances = [NSMutableArray arrayWithCapacity:possibleTerms.count];
//
//				NSInteger maxEditDistance = self.delegate.maximumEditDistanceForAutoCompleteTerms;
//
//				for(NSObject *originalObject in possibleTerms) {
//
//					if(self.isCancelled){
//						return [NSArray array];
//					}
//
//					NSUInteger maximumRange = (inputString.length < currentString.length) ? inputString.length : currentString.length;
//					float editDistanceOfCurrentString = [inputString asciiLevenshteinDistanceWithString:[currentString substringWithRange:NSMakeRange(0, maximumRange)]];
//
//					if(editDistanceOfCurrentString > maxEditDistance){
//						continue;
//					}
//
//					NSDictionary * stringsWithEditDistances = @{kSortInputStringKey : currentString ,
//						kSortObjectKey : originalObject,
//						kSortEditDistancesKey : [NSNumber numberWithFloat:editDistanceOfCurrentString]};
//					[editDistances addObject:stringsWithEditDistances];
//				}
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
//			}
		}
	}
}

