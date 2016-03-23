# MLPAutoComplete-Xamarin-iOS

A Xamarin iOS port for https://github.com/EddyBorja/MLPAutoCompleteTextField

Only basic functionality works for now. It is nowhere near completion. The sorting alghorithm is not implemented yet and customcell don't work yet.

Feel free to contact me or to just contribute to make this project production ready.

### Step 1 Register your UITextField base class as MLPAutoCompleteTextField

### Step 2 Create a custom datasource

* this one uses a google location api
```sh
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

public async Task<string[]> AutoCompleteTextField (MLPAutoCompleteTextField textField, string possibleCompletionsForString)
{
throw new NotImplementedException ();
}
#endregion
}
```	

* Or your own dataset
```sh
public class MyDataSource : IMLPAutoCompleteTextFieldDataSource
{
IEnumerable<string> countries = new List<string>(){"Abkhazia","Afghanistan","Aland","Albania","Algeria"};

#region MLPAutoCompleteTextFieldDataSource implementation

public async Task AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString, Action<IEnumerable> completionHandler)
{
completionHandler (countries);
}

public async Task<string[]> AutoCompleteTextField (MLPAutoComplete.MLPAutoCompleteTextField textField, string possibleCompletionsForString)
{
throw new NotImplementedException ();
}
#endregion
}
```	



Step 3 Call setup method
```sh
var field = autoTextField;
field.Setup (new MyDataSource (), false);
```	


