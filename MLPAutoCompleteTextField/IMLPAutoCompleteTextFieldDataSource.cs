using System;
using ObjCRuntime;
using Foundation;
using System.Collections;
using System.Threading.Tasks;

namespace MLPAutoComplete
{
	public interface IMLPAutoCompleteTextFieldDataSource
	{

		/*
		 When you have the suggestions ready you must call the completionHandler block with 
		 an NSArray of strings or objects implementing the MLPAutoCompletionObject protocol that 
		 could be used as possible completions for the given string in textField.
		 */
		Task AutoCompleteTextField(MLPAutoCompleteTextField textField, string possibleCompletionsForString, Action<IEnumerable> completionHandler);


		/*
		 Like the above, this method should return an NSArray of strings or objects implementing the MLPAutoCompletionObject protocol
		 that could be used as possible completions for the given string in textField.
		This method will be called asynchronously, so an immediate return is not necessary.
		 */
		//Task<string[]> AutoCompleteTextField(MLPAutoCompleteTextField textField, string possibleCompletionsForString);

	}
}

