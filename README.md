# WPF.Translations
An API that provides translations to a WPF application without the need to restart. Provides bindable language strings that can be used in XAML but has the robustness to be used in C# code as well. One stop shopping for translations.

## The Missing Link
If you have ever used Qt before with Qt Linguist then you know how awesome and easy translations can be in a Qt development environment. These translations can be updated in real time without the need to restart the application. Why doesn't something like WPF have this type of functionality built into it? Seems like a mature framework would have a modern day solution to this problem. A solution that is capable of providing bindable translated strings as well as strings that can be used in code! It's 2023 (when I designed this) and I would expect something like this to be built into newer versions of .NET. I am not aware of any direct support for such a thing.

### .NET and Providing Translations
There are plenty of articles and even GitHub samples/examples that describe how to use .RESX files to provide translations. Simple enough and easy to achieve if you don't mind restarting your application. Not too big of a deal, right? I don't think so for most applications. However, if you have ever designed software that has quite the load process (enterprise software (I am not saying all enterprise software loads slowly)) then simply "restarting" the application becomes a process. 

There are even cool articles and GitHub samples/examples that show how to use a ResourceDictionary (RD) that holds translated strings. Then consuming that RD in your application and writing code to merge it into your application's resources collection programmatically. Then in your XAML you use the `{DynamicResource MyTranslatedResource}`. This approach though does not allow you to access these strings from code. What if I need to show a message box from my view model to get user input? I have to write other code to make this goal achievable. Seems like a half solution to me.

## Enter WPF.Translations
This API *provides translations* that can be **bound to for the front-end** and can be **called from code in the back-end** as well. Finally about time WPF gets an API capable of providing what I would consider a modern enterprise level translation mechanism. I just had to write it myself, but now I will share it with the world.

### At a Glance
Check out the project [wiki](https://github.com/AaronAmberman/WPF.Translations/wiki) for a little more information about each type.

The API itself is rather small and does not have anything to do with directly controlling access to XAML or C#. That is up to you but we'll go over how to very easily achieve this. This API is capable of reading strings from a XAML ResourceDictionary or a custom resource file. If you have a non-standard translation data source you can roll your own translation data interpeter. Why should you be forced to a type a file for translations? You shouldn't!

##### Member Enforcement
The translation object while being a DynamicObject does not allow for additional dynamic properties or methods to be added to it. Member creation is locked down and translation objects are immutable. That being said, the translation strings themselves are mutable...they need to be! This way they can automatically update the UI when bound to. So don't try to add new properties or methods to it.

![image](https://user-images.githubusercontent.com/23512394/219885295-ac427b0d-d77c-4ea5-b0dc-14d3c2003eb8.png)

###### Warning
Make sure you type your key names correctly or this will generate an exception at runtime. 

`
System.Windows.Data Warning: 17 : Cannot get 'Units' value (type 'Object') from 'Translations' (type 'Translation'). BindingExpression:Path=Translations.Units; DataItem='MainWindowViewModel' (HashCode=31664161); target element is 'TabItem' (Name=''); target property is 'Header' (type 'Object') InvalidOperationException:'System.InvalidOperationException: Property path is not valid. 'System.Dynamic.DynamicObject+MetaDynamic' does not have a public property named 'Items'.
   at CallSite.Target(Closure , CallSite , Object )
   at System.Dynamic.UpdateDelegates.UpdateAndExecute1[T0,TRet](CallSite site, T0 arg0)
   at MS.Internal.DynamicPropertyAccessorImpl.GetValue(Object component)
   at MS.Internal.Data.PropertyPathWorker.GetValue(Object item, Int32 level)
   at MS.Internal.Data.PropertyPathWorker.RawValue(Int32 k)'
`

XAML binding error to a property that doesn't exist.

There is no auto-complete for member declarations because it is a dynamic object. Well, that is not entirely true.

If the developer types it out as a Translation then you won't have access to the keys directly as properties (you will have to access them through methods and you don't want to do that).

`Translation translation = new Translation(...);`

Don't do that though. It is a dynamic object so treat it as such.

`dynamic translation = new Translation(...);`

This is how .NET works with dynamic objects. This is not a dynamic object tutorial so if you are unfamiliar please google it and read up on it.

Back to the point, if you write it the top way you will NOT have access to all the properties as properties. You won't be able to type `MainWindow.Title = Translation.MainWindowTitle` (or whatever your property name is). You'd have to type `string value = Translator.GetTranslation("MainWindowTitle")` and again you don't want to have to do that.

## Usage
Using the API to get translated strings is super simple.

### XAML example
`
Title="{Binding RelativeSource={RelativeSource Self}, Path=Translations.WindowTitle, FallbackValue=Testing Application}"
`
Here I am binding to a Translation object and looking for a property called **WindowTitle**. Notice the *FallbackValue* in the binding! ***This is because translations are a runtime thing not a design time thing.*** So if you want to be able to see something in the designer simply enter a *FallbackValue*.

### C#
`
MessageBox.Show(Translations.WindowTitle);
`

### Eager Loading vs. Lazy Loading
You can load all your translations right away or you can have them loaded on first access.

#### IDisposable
The Translator and Translation objects both implement IDisposable so be sure to clean up your resources.

### Test / Demo Application
In the repo you'll see a project that shows how to setup the API for use. This is not the only way the API can be utilized. This API can just as easily be used in an MVVM setup. Just put the Translation object on your view model and bind to the properties. That easy.

Just remember that wherever you decide to make the reference to the translation, make it dynamic not of type Translation.

![image](https://user-images.githubusercontent.com/23512394/219885446-95ded394-c02e-43b1-90e2-e27ab4035851.png)

### In Action
![Translations](https://user-images.githubusercontent.com/23512394/219886361-42a99bee-cd6e-4d3e-bcdd-ef0cee049248.gif)

Simple and effective! Check out the demo app.

Hopefully this makes some other developer's life easier as it has made mine easier. 

Translatable strings that can be bound in XAML or used in C# without the need to restart the application? Yes please! Enjoy!

## Your Way
Pro tip: the *Translator* object is built as a convenience class. You are not required to use it as the secret sauce here is the fact that the *Translation* object is a *DynamicObject*. Just use the *Translation* object directly however you'd like. This way you have complete control over how *Translation* objects are managed.

## One Final Note
In the demo application I am using pack application strings because my ResourceDictionaries are set to "Resource" under properties.

# Example
If you want to see a real application example then check out my [VTOL VR Mission Assistant](https://github.com/AaronAmberman/VTOLVR-MissionAssistant).

My [LTT-WPF-Application](https://github.com/AaronAmberman/LTT-WPF-Application) template uses this API but does not use the Translator. Instead it uses the Translation object directly to manage translations. Check it out for a different strategy for managing a Translation directly instead of using the Translator.
