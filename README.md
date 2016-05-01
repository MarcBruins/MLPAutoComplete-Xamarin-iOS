# MLPAutoComplete-Xamarin-iOS

A Xamarin iOS port for https://github.com/EddyBorja/MLPAutoCompleteTextField

![Alt text](/autocompleteDemo.png "Screenshot")|![Alt text](/keyboardDemo.png "Screenshot")

### Step 1 Register your UITextField base class as MLPAutoCompleteTextField

You can do this in the interface builder

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
  
  #endregion
}
```	



### Step 3 Call setup method


```sh
autoTextField.Setup (new MyDataSource ());
```	
If you want to use the sorting alghoritm set second parameter to true

```sh
autoTextField.Setup (new MyDataSource (), true);
```	

You can also switch to a Keyboardaccesory autocomplete table
```sh
autoTextField.Setup (new MyDataSource (),true);
autoTextField.AutoCompleteTableAppearsAsKeyboardAccessory = true;
```	
