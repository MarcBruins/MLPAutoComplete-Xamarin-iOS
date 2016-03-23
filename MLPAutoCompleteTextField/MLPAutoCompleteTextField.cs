using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System.Collections;
using System.Threading.Tasks;

namespace MLPAutoComplete
{
	interface IMLPAutoCompleteSortOperation
	{
		void AutoCompleteTermsDidSort(string[] completions);
		int MaximumEditDistanceForAutoCompleteTerms(); //not sure

	}

	interface IMLPAutoCompleteFetchOperation
	{
		void AutoCompleteTermsDidFetch(Dictionary<string, bool> fetchInfo);
	}

	[Register("MLPAutoCompleteTextField")]
	public class MLPAutoCompleteTextField : UITextField, IUITableViewDataSource, IUITableViewDelegate, IMLPAutoCompleteFetchOperation, IMLPAutoCompleteSortOperation
	{
		private IMLPAutoCompleteTextFieldDataSource autoCompleteDataSource;
		private IMLPAutoCompleteTextFieldDelegate autoCompleteDelegate;

		const string kSortInputStringKey = "sortInputString";
		const string kSortEditDistancesKey = "editDistances";
		const string kSortObjectKey = "sortObject";
		const string kKeyboardAccessoryInputKeyPath = "autoCompleteTableAppearsAsKeyboardAccessory";
		const double DefaultAutoCompleteRequestDelay = 0.1;

		private string _kBorderStyleKeyPath = "borderStyle";
		private string kBorderStyleKeyPath
		{
			get{ return _kBorderStyleKeyPath;}
			set{_kBorderStyleKeyPath = value; Observe (_kBorderStyleKeyPath); }
		}


		private string _kAutoCompleteTableViewHiddenKeyPath = "autoCompleteTableView.hidden";
		private string kAutoCompleteTableViewHiddenKeyPath
		{
			get{ return _kAutoCompleteTableViewHiddenKeyPath; }
			set{ _kAutoCompleteTableViewHiddenKeyPath = value; Observe (_kAutoCompleteTableViewHiddenKeyPath);}
		}

		private string _kBackgroundColorKeyPath = "kBackgroundColorKeyPath";
		private string kBackgroundColorKeyPath
		{
			get{ return _kBackgroundColorKeyPath; }
			set{ _kBackgroundColorKeyPath = value; Observe (_kBackgroundColorKeyPath);}
		}

		private string _kDefaultAutoCompleteCellIdentifier = "_DefaultAutoCompleteCellIdentifier";
		private string kDefaultAutoCompleteCellIdentifier
		{
			get{ return _kDefaultAutoCompleteCellIdentifier; }
			set{ _kDefaultAutoCompleteCellIdentifier = value; Observe (_kDefaultAutoCompleteCellIdentifier);}
		}

		public string IncompleteString {get;set;}
		private string[] possibleCompletions{ get; set; }

		public bool SortAutoCompleteSuggestionsByClosestMatch = false;
		public bool ApplyBoldEffectToAutoCompleteSuggestions = false;
		public bool ShowTextFieldDropShadowWhenAutoCompleteTableIsOpen = false;
		public bool ShouldResignFirstResponderFromKeyboardAfterSelectionOfAutoCompleteRows = false;

		public int AutoCompleteRowHeight = 40;
		public int AutoCompleteFontSize = 13;
		public int MaximumNumberOfAutoCompleteRows = 3;
		public float PartOfAutoCompleteRowHeightToCut = 0.5f;
		public int MaximumEditDistance = 100;
		public bool RequireAutoCompleteSuggestionsToMatchInputExactly = false;
		public UIColor AutoCompleteTableCellBackgroundColor;
		public UIFont AutoCompleteRegularFontName,AutoCompleteBoldFontName;
		public NSTimer AutoCompleteFetchRequestDelay;
		public IList AutoCompleteSuggestions = new List<Object>();

		public NSOperation AutoCompleteSortQueue,AutoCompleteFetchQueue;

		public CGRect AutoCompleteTableFrame;
		public CGSize AutoCompleteTableOriginOffset;
		public CGSize AutoCompleteTableSizeOffset;
		public float AutoCompleteTableCornerRadius; //only applies for drop down style autocomplete tables.
		public UIEdgeInsets AutoCompleteContentInsets;
		public UIEdgeInsets AutoCompleteScrollIndicatorInsets;
		public UIColor AutoCompleteTableBorderColor;
		public float AutoCompleteTableBorderWidth;
		public UIColor AutoCompleteTableBackgroundColor;
		public UIColor AutoCompleteTableCellTextColor;


		private CGColor originalShadowColor;
		private CGSize originalShadowOffset;
		private float originalShadowOpacity;

		public UITableView AutoCompleteTableView;

		public bool AutoCompleteTableAppearsAsKeyboardAccessory;
		private bool autoCompleteTableViewHidden;

		public string ReuseIdentifier;

		public bool DisableAutoCompleteTableUserInteractionWhileFetching = true, ShouldSort = true;

		MLPAutoCompleteFetchOperation fetchOperation;

		public MLPAutoCompleteTextField(IntPtr ptr) : base(ptr)
		{
			this.initialize ();
		}

		public MLPAutoCompleteTextField (CGRect frame) : base (frame)
		{
			this.initialize ();
		}

		public MLPAutoCompleteTextField (NSCoder coder) : base (coder)
		{
			this.initialize ();
		}

		private void initialize()
		{
			UITableView newTableView =  this.newAutoCompleteTableViewForTextField(this);
			this.AutoCompleteTableView = newTableView;

			this.beginObservingKeyPathsAndNotifications ();
			this.setDefaultValuesForVariables ();


			fetchOperation = new MLPAutoCompleteFetchOperation (this);
		}


		public void Setup(IMLPAutoCompleteTextFieldDataSource datasource)
		{
			this.autoCompleteDataSource = datasource;
			this.autoCompleteDelegate = new DefaultAutoCompleteDelegate();
		}

		public void Setup(IMLPAutoCompleteTextFieldDataSource datasource, bool ShouldSort)
		{
			this.autoCompleteDataSource = datasource;
			this.autoCompleteDelegate = new DefaultAutoCompleteDelegate();
			this.ShouldSort = ShouldSort;
		}

		public void Setup(IMLPAutoCompleteTextFieldDataSource datasource,IMLPAutoCompleteTextFieldDelegate del,bool ShouldSort)
		{
			this.autoCompleteDataSource = datasource;
			this.autoCompleteDelegate = del;
			this.ShouldSort = ShouldSort;
		}

		public override bool BecomeFirstResponder ()
		{
			saveCurrentShadowProperties ();

			if (AutoCompleteTableAppearsAsKeyboardAccessory) 
			{
				this.setAutoCompleteTableBackgroundColor (this.BackgroundColor);
			}

			return base.BecomeFirstResponder();
		}

		void setAutoCompleteTableBackgroundColor(UIColor autoCompleteTableBackgroundColor)
		{
			this.AutoCompleteTableView.BackgroundColor = autoCompleteTableBackgroundColor;
			this.AutoCompleteTableBackgroundColor = autoCompleteTableBackgroundColor;
		}

		void setAutoCompleteTableBorderWidth(float autoCompleteTableBorderWidth)
		{
			this.AutoCompleteTableView.Layer.BorderWidth = autoCompleteTableBorderWidth;
			this.AutoCompleteTableBorderWidth = autoCompleteTableBorderWidth;
		}

		void setAutoCompleteTableBorderColor(UIColor autoCompleteTableBorderColor)
		{
			this.AutoCompleteTableView.Layer.BorderColor = autoCompleteTableBorderColor.CGColor;
			this.AutoCompleteTableBorderColor = autoCompleteTableBorderColor;
		}

		void setAutoCompleteContentInsets(UIEdgeInsets autoCompleteContentInsets)
		{
			this.AutoCompleteTableView.ContentInset = autoCompleteContentInsets;
			this.AutoCompleteContentInsets = autoCompleteContentInsets;
		}


		void setAutoCompleteTableViewHidden(bool autoCompleteTableViewHidden)
		{
			this.AutoCompleteTableView.Hidden = autoCompleteTableViewHidden;
		}

		UITableView newAutoCompleteTableViewForTextField(MLPAutoCompleteTextField textField)
		{
			CGRect dropDownTableFrame = this.autoCompleteTableViewFrameForTextField (textField);

			var newTableView = new UITableView (dropDownTableFrame, UITableViewStyle.Plain);
			newTableView.WeakDelegate = textField;
			newTableView.DataSource = textField;
			newTableView.ScrollEnabled = true;
			newTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			return newTableView;
		}

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected (UIKit.UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			if (this.AutoCompleteTableAppearsAsKeyboardAccessory) {
				this.closeAutoCompleteTableView ();
			}

			var cell = tableView.CellAt (indexPath);
			string autoCompleteString = cell.TextLabel.Text;
			this.Text = autoCompleteString;

			var autoCompleteObject = this.AutoCompleteSuggestions[indexPath.Row];
			if (autoCompleteObject != null && autoCompleteObject is MLPAutoCompletionObject) {
				this.autoCompleteDelegate.AutoCompleteTextField (this, autoCompleteString,(MLPAutoCompletionObject) autoCompleteObject, indexPath);
			}
			this.finishedSearching ();
		}

		void finishedSearching()
		{
			if(this.ShouldResignFirstResponderFromKeyboardAfterSelectionOfAutoCompleteRows){
				this.ResignFirstResponder();
			}
		}


		public UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell;
			var cellIdentifier = kDefaultAutoCompleteCellIdentifier;

			if (String.IsNullOrEmpty (this.ReuseIdentifier)) {
				cell = tableView.DequeueReusableCell (cellIdentifier);
				if(cell == null){
					cell = autoCompleteTableViewCellWithReuseIdentifier (cellIdentifier);
				}
			}else{
				cell = tableView.DequeueReusableCell(this.ReuseIdentifier);
			}	
			if (cell == null) {
				Console.WriteLine ("Unable to create cell for autocomplete table");
			}

			var autoCompleteObject = this.AutoCompleteSuggestions [indexPath.Row];
			string suggestedString = string.Empty;

			if (autoCompleteObject is string) {
				suggestedString = (string)autoCompleteObject;
			} else if (autoCompleteObject is MLPAutoCompletionObject) {
				suggestedString = ((MLPAutoCompletionObject)autoCompleteObject).AutocompleteString;
			} else {
				Console.WriteLine ("Unable to create cell for autocomplete table");
			}

			this.configureCell (cell, indexPath, suggestedString);

			return cell;
		}

		void configureCell(UITableViewCell cell, NSIndexPath indexPath, string autoCompleteString)
		{
			//NSAttributedString boldedString;

			var attributedTextSupport = cell.TextLabel.RespondsToSelector (new Selector("setAttributedText:"));
			if (attributedTextSupport && this.ApplyBoldEffectToAutoCompleteSuggestions) {
				//TODO
			}

			var autoCompleteObject = this.AutoCompleteSuggestions [indexPath.Row];
			if (autoCompleteObject is MLPAutoCompletionObject) {
				autoCompleteObject = null;

				if (!(autoCompleteDelegate.ShouldConfigureCell (this, cell, autoCompleteString, (MLPAutoCompletionObject)autoCompleteObject, indexPath)))
				{
					return;
				}
			}

			cell.TextLabel.TextColor = this.TextColor;
			cell.TextLabel.Font = this.Font;

			cell.TextLabel.Text = autoCompleteString;

			//TODO
			//
			//			if (AutoCompleteTableCellTextColor) {
			//				cell.TextLabel.TextColor = this.autoCompleteTableCellTextColor;
			//			}
			//			if(boldedString){
			//				if ([cell.textLabel respondsToSelector:@selector(setAttributedText:)]) {
			//					[cell.textLabel setAttributedText:boldedString];
			//				} else{
			//					[cell.textLabel setText:string];
			//					[cell.textLabel setFont:[UIFont fontWithName:self.font.fontName size:self.autoCompleteFontSize]];
			//				}
			//
			//			} else {
			//				[cell.textLabel setText:string];
			//				[cell.textLabel setFont:[UIFont fontWithName:self.font.fontName size:self.autoCompleteFontSize]];
			//			}
			//
			//			cell.accessibilityLabel = [[self class] accessibilityLabelForIndexPath:indexPath];
			//
			//			if(self.autoCompleteTableCellTextColor){
			//				[cell.textLabel setTextColor:self.autoCompleteTableCellTextColor];
			//			}
		}

		UITableViewCell autoCompleteTableViewCellWithReuseIdentifier(string identifier)
		{
			var cell = new UITableViewCell (UITableViewCellStyle.Subtitle,identifier);
			cell.BackgroundColor = UIColor.Clear;
			cell.TextLabel.TextColor = this.TextColor;
			cell.TextLabel.Font = this.Font;
			return cell;
		}


		public nint RowsInSection (UITableView tableView, nint section)
		{
			var numberOfRows = this.AutoCompleteSuggestions.Count;
			this.expandAutoCompleteTableViewForNumberOfRows (numberOfRows);
			return numberOfRows;
		}

		void expandAutoCompleteTableViewForNumberOfRows(int numberOfRows)
		{
			if (numberOfRows <= 0)
				Console.WriteLine ("Number of rows given for auto complete table was negative, this is impossible.");

			if (!this.IsFirstResponder)
				return;

			if (this.AutoCompleteTableAppearsAsKeyboardAccessory)
				this.expandKeyboardAutoCompleteTableForNumberOfRows (numberOfRows);
			else
				this.expandDropDownAutoCompleteTableForNumberOfRows (numberOfRows);
		}

		void expandKeyboardAutoCompleteTableForNumberOfRows(int numberOfRows)
		{
			if(numberOfRows > 0 && (this.autoCompleteTableViewHidden == false)){
				this.AutoCompleteTableView.Alpha = 1;
			} else {
				this.AutoCompleteTableView.Alpha = 0;
			}
		}


		void expandDropDownAutoCompleteTableForNumberOfRows(int numberOfRows)
		{
			this.resetDropDownAutoCompleteTableFrameForNumberOfRows(numberOfRows);

			if(numberOfRows > 0 && (this.autoCompleteTableViewHidden == false)){
				this.AutoCompleteTableView.Alpha = 1;
				var tableViewWillBeAddedToViewHierarchy = this.AutoCompleteTableView.Superview  == null ? true : false;
				if (tableViewWillBeAddedToViewHierarchy) {
					autoCompleteDelegate.WillShowAutoCompleteTableView (this, AutoCompleteTableView);

				}

				UIView rootView = Window.Subviews[0];
				rootView.InsertSubviewBelow (this.AutoCompleteTableView,this);

				AutoCompleteTableView.UserInteractionEnabled = true;

				if (ShowTextFieldDropShadowWhenAutoCompleteTableIsOpen) {
					this.Layer.ShadowColor = UIColor.Black.CGColor;
					this.Layer.ShadowOffset = new CGSize (0, 1);
					this.Layer.ShadowOpacity = 0.35f;
				}
				if (tableViewWillBeAddedToViewHierarchy) {
					autoCompleteDelegate.DidShowAutoCompleteTableView (this, AutoCompleteTableView);
				} 

			}else {
				this.closeAutoCompleteTableView ();
				this.restoreOriginalShadowProperties ();
				this.AutoCompleteTableView.Layer.ShadowOpacity = 0;
			}
		}

		void beginObservingKeyPathsAndNotifications()
		{
			this.Observe (kBorderStyleKeyPath);
			this.Observe (kBackgroundColorKeyPath);
			this.Observe (kKeyboardAccessoryInputKeyPath);
			this.Observe (kAutoCompleteTableViewHiddenKeyPath);

			NSNotificationCenter.DefaultCenter.AddObserver (UITextField.TextFieldTextDidChangeNotification, textFieldDidChangeWithNotification);
		}

		public void textFieldDidChangeWithNotification(NSNotification aNotification)
		{
			this.reloadData ();
		}

		void stopObservingKeyPathsAndNotifications()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
			this.RemoveObserver (this, kBorderStyleKeyPath);
			this.RemoveObserver (this, kAutoCompleteTableViewHiddenKeyPath);
			this.RemoveObserver (this, kBackgroundColorKeyPath);
			this.RemoveObserver (this, kKeyboardAccessoryInputKeyPath);
		}

		private void Observe (string keyPath)
		{
			if (keyPath.Equals (kBorderStyleKeyPath)) {
				this.styleAutoCompleteTableForBorderStyle (this.BorderStyle);
			} else if (keyPath.Equals (kAutoCompleteTableViewHiddenKeyPath)) {
				if (this.AutoCompleteTableView.Hidden) {
					this.closeAutoCompleteTableView ();
				} else {
					this.AutoCompleteTableView.ReloadData ();
				}
			} else if (keyPath.Equals (kBackgroundColorKeyPath)) {
				styleAutoCompleteTableForBorderStyle (this.BorderStyle);
			} else if (keyPath.Equals (kKeyboardAccessoryInputKeyPath)) {
				if (this.AutoCompleteTableAppearsAsKeyboardAccessory) {
					this.setAutoCompleteTableForKeyboardAppearance ();
				} else {
					this.setAutoCompleteTableForDropDownAppearance ();
				}
			}
		}

		void closeAutoCompleteTableView()
		{

			autoCompleteDelegate.WillHideAutoCompleteTableView (this, AutoCompleteTableView);

			this.AutoCompleteTableView.RemoveFromSuperview ();
			this.restoreOriginalShadowProperties ();

			autoCompleteDelegate.DidHideAutoCompleteTableView (this, AutoCompleteTableView);

		}

		void saveCurrentShadowProperties()
		{
			this.originalShadowColor = this.Layer.ShadowColor;
			this.originalShadowOffset = this.Layer.ShadowOffset;
			this.originalShadowOpacity = this.Layer.ShadowOpacity;
		}

		void restoreOriginalShadowProperties()
		{
			this.Layer.ShadowColor = this.originalShadowColor;
			this.Layer.ShadowOffset = this.originalShadowOffset;
			this.Layer.ShadowOpacity = this.originalShadowOpacity;
		}

		void setAutoCompleteTableForKeyboardAppearance()
		{	
			this.resetKeyboardAutoCompleteTableFrameForNumberOfRows (this.MaximumNumberOfAutoCompleteRows);
			this.AutoCompleteTableView.ContentInset = UIEdgeInsets.Zero;
			this.AutoCompleteTableView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
			this.InputAccessoryView = this.AutoCompleteTableView;
		}

		void setAutoCompleteTableForDropDownAppearance()
		{
			this.resetDropDownAutoCompleteTableFrameForNumberOfRows (this.MaximumNumberOfAutoCompleteRows);
			this.AutoCompleteTableView.ContentInset = AutoCompleteContentInsets;
			this.AutoCompleteTableView.ScrollIndicatorInsets = AutoCompleteScrollIndicatorInsets;
			this.InputAccessoryView = null;
		}


		void resetKeyboardAutoCompleteTableFrameForNumberOfRows(int numberOfRows)
		{
			this.AutoCompleteTableView.Layer.CornerRadius = 0;

			var newAutoCompleteTableViewFrame = autoCompleteTableViewFrameForTextField (this, numberOfRows);
			this.AutoCompleteTableView.Frame = newAutoCompleteTableViewFrame;

			this.AutoCompleteTableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			this.AutoCompleteTableView.ScrollRectToVisible (new CGRect(0,0,1,1), false);
		}

		void resetDropDownAutoCompleteTableFrameForNumberOfRows(int numberOfRows)
		{
			this.AutoCompleteTableView.Layer.CornerRadius = this.AutoCompleteTableCornerRadius;
			var newAutoCompleteTableViewFrame = autoCompleteTableViewFrameForTextField (this, numberOfRows);

			this.AutoCompleteTableView.Frame = newAutoCompleteTableViewFrame;
			this.AutoCompleteTableView.ScrollRectToVisible (new CGRect(0,0,1,1), false);
		}

		CGRect autoCompleteTableViewFrameForTextField(MLPAutoCompleteTextField textField, int numberOfRows)
		{
			//			#if BROKEN
			//			// TODO: Reimplement this code for people using table views. Right now it breaks
			//			//       more normal use cases because of UILayoutContainerView
			//			CGRect newTableViewFrame             = [self autoCompleteTableViewFrameForTextField:textField];
			//
			//			UIView *rootView                     = [textField.window.subviews objectAtIndex:0];
			//			CGRect textFieldFrameInContainerView = [rootView convertRect:textField.bounds
			//			fromView:textField];
			//
			//			CGFloat textfieldTopInset = textField.autoCompleteTableView.contentInset.top;
			//			CGFloat converted_originY = textFieldFrameInContainerView.origin.y + textfieldTopInset;
			//
			//			#else
			//			CGRect newTableViewFrame = [self autoCompleteTableViewFrameForTextField:textField];
			//			CGFloat textfieldTopInset = textField.autoCompleteTableView.contentInset.top;
			//			#endif
			//
			//			CGFloat height = [self autoCompleteTableHeightForTextField:textField withNumberOfRows:numberOfRows];
			//
			//			newTableViewFrame.size.height = height;
			//			#if BROKEN
			//			newTableViewFrame.origin.y    = converted_originY;
			//			#endif
			//
			//			if(!textField.autoCompleteTableAppearsAsKeyboardAccessory){
			//				newTableViewFrame.size.height += textfieldTopInset;
			//			}
			//
			//			return newTableViewFrame;
			CGRect newTableViewFrame = autoCompleteTableViewFrameForTextField(textField);
			nfloat textfieldTopInset = textField.AutoCompleteTableView.ContentInset.Top;
			nfloat height = this.autoCompleteTableHeightForTextField(textField, numberOfRows);

			newTableViewFrame.Height = height;
			if(!textField.AutoCompleteTableAppearsAsKeyboardAccessory){
				newTableViewFrame.Height += textfieldTopInset;
			}
			return newTableViewFrame;
		}

		nfloat autoCompleteTableHeightForTextField(MLPAutoCompleteTextField  textField, int numberOfRows)
		{
			nfloat maximumHeightMultiplier = (textField.MaximumNumberOfAutoCompleteRows - textField.PartOfAutoCompleteRowHeightToCut);
			nfloat heightMultiplier;
			if(numberOfRows >= textField.MaximumNumberOfAutoCompleteRows){
				heightMultiplier = maximumHeightMultiplier;
			} else {
				heightMultiplier = numberOfRows;
			}

			nfloat height = textField.AutoCompleteRowHeight * heightMultiplier;
			return height;
		}


		CGRect autoCompleteTableViewFrameForTextField(MLPAutoCompleteTextField textField)
		{
			CGRect frame = CGRect.Empty;

			if ((this.AutoCompleteTableFrame).Width > 0){
				frame = this.AutoCompleteTableFrame;
			} else {
				frame = textField.Frame;
				frame.Y += textField.Frame.Size.Height;
			}

			frame.X += textField.AutoCompleteTableOriginOffset.Width;
			frame.Y += textField.AutoCompleteTableOriginOffset.Height;
			frame.Height += textField.AutoCompleteTableSizeOffset.Height;
			frame.Width += textField.AutoCompleteTableSizeOffset.Width;
			frame = RectangleFExtensions.Inset(frame, 1, 0);

			return frame;
		}


		void registerAutoCompleteCellNib(UINib nib, string reuseIdentifier)
		{
			if (this.AutoCompleteTableView == null) {
				Console.WriteLine ("Must have an autoCompleteTableView to register cells to.");
				return;
			}

			if (!String.IsNullOrEmpty (reuseIdentifier)) {
				this.unregisterAutoCompleteCellForReuseIdentifier (reuseIdentifier);
			}

			this.AutoCompleteTableView.RegisterNibForCellReuse (nib, reuseIdentifier);
			this.ReuseIdentifier = reuseIdentifier;
		}


		void registerAutoCompleteCellClass(Type cellClass, string reuseIdentifier)
		{
			if (this.AutoCompleteTableView == null) {
				Console.WriteLine ("Must have an autoCompleteTableView to register cells to.");
				return;
			}
			if (!String.IsNullOrEmpty (reuseIdentifier)) {
				this.unregisterAutoCompleteCellForReuseIdentifier (reuseIdentifier);
			}	


			var classSettingSupported = this.AutoCompleteTableView.RespondsToSelector (new Selector ("registerClass:forCellReuseIdentifier:"));
			if (classSettingSupported) {
				Console.WriteLine ("Unable to set class for cell for autocomplete table, in iOS 5.0 you can set a custom NIB for a reuse identifier to get similar functionality.");
			}

			AutoCompleteTableView.RegisterClassForCellReuse (cellClass, reuseIdentifier);
			this.ReuseIdentifier = reuseIdentifier;
		}


		void unregisterAutoCompleteCellForReuseIdentifier(string reuseIdentifier)
		{
			this.AutoCompleteTableView.RegisterNibForCellReuse (null, reuseIdentifier);
		}


		void styleAutoCompleteTableForBorderStyle(UITextBorderStyle borderStyle)
		{

			if(autoCompleteDelegate != null)
			if(!(this.autoCompleteDelegate.ShouldStyleAutoCompleteTableView(this,AutoCompleteTableView,borderStyle)))
				return;


			switch (borderStyle) {
			case UITextBorderStyle.RoundedRect:
				this.setRoundedRectStyleForAutoCompleteTableView();
				break;
			case UITextBorderStyle.Bezel:
			case UITextBorderStyle.Line:
				this.setLineStyleForAutoCompleteTableView();
				break;
			case UITextBorderStyle.None:
				this.setNoneStyleForAutoCompleteTableView();
				break;
			default:
				break;
			}
		}


		void setRoundedRectStyleForAutoCompleteTableView()
		{
			this.AutoCompleteTableCornerRadius = 8;
			this.AutoCompleteTableOriginOffset = new CGSize (0, -18);
			this.AutoCompleteScrollIndicatorInsets = new UIEdgeInsets(18, 0, 0, 0);
			this.AutoCompleteContentInsets =  new UIEdgeInsets (18, 0, 0, 0);

			if (BackgroundColor == UIColor.Clear)
				this.AutoCompleteTableView.BackgroundColor = UIColor.White;
			else
				this.AutoCompleteTableView.BackgroundColor = this.BackgroundColor;
		}

		void setLineStyleForAutoCompleteTableView()
		{
			this.AutoCompleteTableCornerRadius = 0;
			this.AutoCompleteTableOriginOffset = CGSize.Empty;
			this.AutoCompleteScrollIndicatorInsets = UIEdgeInsets.Zero;
			this.AutoCompleteTableBorderWidth = 1;
			this.AutoCompleteTableBorderColor = UIColor.FromWhiteAlpha (0, 0.5f);

			if (BackgroundColor == UIColor.Clear)
				this.AutoCompleteTableView.BackgroundColor = UIColor.White;
			else
				this.AutoCompleteTableView.BackgroundColor = this.BackgroundColor;
		}

		void setNoneStyleForAutoCompleteTableView()
		{
			this.AutoCompleteTableCornerRadius = 8;
			this.AutoCompleteTableOriginOffset = new CGSize (0, 7);
			this.AutoCompleteScrollIndicatorInsets = UIEdgeInsets.Zero;
			this.AutoCompleteContentInsets =  UIEdgeInsets.Zero;
			this.AutoCompleteTableBorderWidth = 1;

			UIColor lightBlueColor = UIColor.FromRGB (188 / 255, 204 / 255, 255 / 255);
			AutoCompleteTableBorderColor = lightBlueColor;

			UIColor blueTextColor = UIColor.FromRGB (23 / 255, 119 / 255, 206 / 255);
			AutoCompleteTableCellBackgroundColor = blueTextColor;


			if (BackgroundColor == UIColor.Clear)
				this.AutoCompleteTableView.BackgroundColor = UIColor.White;
			else
				this.AutoCompleteTableView.BackgroundColor = this.BackgroundColor;
		}


		void reloadData(){
			fetchOperation.Cancel ();
			fetchAutoCompleteSuggestions();
		}

		void fetchAutoCompleteSuggestions()
		{
			if (this.DisableAutoCompleteTableUserInteractionWhileFetching) {
				this.AutoCompleteTableView.UserInteractionEnabled = false;
			}

			//this.AutoCompleteFetchQueue.Cancel ();

			fetchOperation.Fetch (autoCompleteDataSource);

			//			[self.autoCompleteFetchQueue cancelAllOperations];
			//
			//			MLPAutoCompleteFetchOperation *fetchOperation = [[MLPAutoCompleteFetchOperation alloc]
			//				initWithDelegate:self
			//				completionsDataSource:self.autoCompleteDataSource
			//				autoCompleteTextField:self];
			//
			//			[self.autoCompleteFetchQueue addOperation:fetchOperation];
		}



		#region IMLPAutoCompleteFetchOperation implementation

		public void AutoCompleteTermsDidFetch (Dictionary<string, bool> fetchInfo)
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region IMLPAutoCompleteSortOperation implementation

		public void AutoCompleteTermsDidSort (string[] completions)
		{
			throw new NotImplementedException ();
		}

		public int MaximumEditDistanceForAutoCompleteTerms ()
		{
			throw new NotImplementedException ();
		}

		#endregion

		void setDefaultValuesForVariables()
		{
			this.ClipsToBounds = false;
			//this.AutoCompleteFetchRequestDelay = DefaultAutoCompleteRequestDelay;
			this.SortAutoCompleteSuggestionsByClosestMatch = false;
			this.ApplyBoldEffectToAutoCompleteSuggestions = true;
			this.ShowTextFieldDropShadowWhenAutoCompleteTableIsOpen = true;
			this.ShouldResignFirstResponderFromKeyboardAfterSelectionOfAutoCompleteRows = true;

			this.AutoCompleteRowHeight = 40;
			this.AutoCompleteFontSize = 13;
			this.MaximumNumberOfAutoCompleteRows = 3;
			this.PartOfAutoCompleteRowHeightToCut = 0.5f;

			this.MaximumEditDistance = 100;
			this.RequireAutoCompleteSuggestionsToMatchInputExactly = false;
			this.AutoCompleteTableCellBackgroundColor = UIColor.Clear;

			this.AutoCompleteRegularFontName = UIFont.SystemFontOfSize (13);
			this.AutoCompleteBoldFontName = UIFont.BoldSystemFontOfSize (13);

			this.AutoCompleteSuggestions = new string[]{};

			this.AutoCompleteSortQueue = new NSOperation ();
			this.AutoCompleteSortQueue.Name = String.Format ("Autocomplete queue{0}", new Random ().Next ());

			this.AutoCompleteFetchQueue = new NSOperation ();
			this.AutoCompleteSortQueue.Name = String.Format ("Fetch queue{0}", new Random ().Next ());
		}

	}
}

