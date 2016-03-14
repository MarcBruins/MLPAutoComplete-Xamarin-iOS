using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System.Collections;
using CoreFoundation;
using System.Linq;
using System.Threading.Tasks;

namespace MLPAutoComplete
{
	class MLPAutoCompleteFetchOperation
	{
		private MLPAutoCompleteTextField _textField;
		public bool IsCancelled {
			get;
			set;
		}

		public MLPAutoCompleteFetchOperation(MLPAutoCompleteTextField textField)
		{
			_textField = textField;
		}

		public void Fetch(MLPAutoCompleteTextFieldDataSource _dataSource)
		{
			this.IsCancelled = false;
			var self = this;
			_dataSource.AutoCompleteTextField (_textField, _textField.incompleteString, delegate(IEnumerable suggestions) {
				self.didReceiveSuggestions(suggestions.Cast<object>().ToList());
			});
//
//				[self.dataSource autoCompleteTextField:self.textField
//					possibleCompletionsForString:self.incompleteString
//					completionHandler:^(NSArray *suggestions){
//
//						[operation performSelector:@selector(didReceiveSuggestions:) withObject:suggestions];
//						dispatch_semaphore_signal(sentinelSemaphore);
//					}];
//
//				dispatch_semaphore_wait(sentinelSemaphore, DISPATCH_TIME_FOREVER);
//				if(self.isCancelled){
//					return;
//				}
//			} else if ([self.dataSource respondsToSelector:@selector(autoCompleteTextField:possibleCompletionsForString:)]){
//
//				NSArray *results = [self.dataSource autoCompleteTextField:self.textField
//					possibleCompletionsForString:self.incompleteString];
//
//				if(!self.isCancelled){
//					[self didReceiveSuggestions:results];
//				}
//
//			} else {
//				NSAssert(0, @"An autocomplete datasource must implement either autoCompleteTextField:possibleCompletionsForString: or autoCompleteTextField:possibleCompletionsForString:completionHandler:");
//			}
		}

		public void Cancel()
		{
			this.IsCancelled = true;
		}


		public void didReceiveSuggestions(List<Object> suggestions)
		{
			if(suggestions == null){
				suggestions = new List<Object> ();
			}
			if(!this.IsCancelled){
				if (suggestions.Count > 0) {
					var firstObject = suggestions[0];
					if (!(firstObject is string || firstObject is MLPAutoCompletionObject))
						Console.WriteLine ("MLPAutoCompleteTextField expects an array with objects that are either strings or conform to the MLPAutoCompletionObject protocol for possible completions.");

					autoCompleteTermsDidFetch (_textField.Text,suggestions);
				}
			}
			return;
		}


		void autoCompleteTermsDidFetch(string inputString,List<object> fetchInfo)
		{

			List<Object> completions = fetchInfo;

			//_textField.AutoCompleteSortQueue.IsCancelled = true;

			if (_textField.SortAutoCompleteSuggestionsByClosestMatch) {
				MLPAutoCompleteSortOperation operation = new MLPAutoCompleteSortOperation (inputString, completions);
			} else {
				autoCompleteTermsDidSort (completions);
			}
		}

		void autoCompleteTermsDidSort(List<Object> completions)
		{
			_textField.AutoCompleteSuggestions = completions;

			_textField.autoCompleteTableView.BeginUpdates ();
			_textField.autoCompleteTableView.ReloadData();
			_textField.autoCompleteTableView.EndUpdates ();


			//TODO Sort all the things!

//				[self setAutoCompleteSuggestions:completions];
//				[self.autoCompleteTableView reloadData];
//
//				if ([self.autoCompleteDelegate
//					respondsToSelector:@selector(autoCompleteTextField:didChangeNumberOfSuggestions:)]) {
//					[self.autoCompleteDelegate autoCompleteTextField:self
//						didChangeNumberOfSuggestions:self.autoCompleteSuggestions.count];
					
		}

	}

}

