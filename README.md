# WPF.Translations
An API that provides translations to a WPF application without the need to restart. Provides bindable language strings that can be used in XAML but has the robustness to be used in C# code as well. One stop shopping for translations.

## The Missing Link
If you have ever used Qt before with Qt Linguist then you know how awesome and easy translations can be in a Qt development environment. These translations can be updated in real time without the need to restart the application. Why doesn't something like WPF have this type of functionality built into it? Seems like a mature framework would have a modern day solution to this problem. A solution that is capable of providing bindable translated strings as well as strings that can be used in code? It's 2023 (when I designed this) and I would expect something like this to be built into newer versions of .NET. I am not aware of any direct support for such a thing.

### .NET and Providing Translations
There are plenty of articles and even GitHub samples/examples that describe how to use .RESX files to provide translations. Simple enough and easy to achieve if you don't mind restarting your application. Not too big of a deal, right? I don't think so for most applications. However, if you have ever designed software that has quite the load process (enterprise software (I am not saying all enterprise software loads slowly)) then simply "restarting" the application becomes a process. 

There are even cool articles and GitHub samples/examples that show how to use a ResourceDictionary (RD) that holds translated strings. Then consuming that RD in your application and writing code to merge it into your application's resources collection programmatically. Then in your XAML you use the `{DynamicResource MyTranslatedResource}`. This approach though does not allow you to access these strings from code. What if I need to show a message box from my view model to get user input? I have to write other code to make this goal achievable. Seems like a half solution to me.

## Enter WPF.Translations
This API **provides translations** that can be *bound to for the front-end* and can be *called from code in the back-end* as well. Finally about time WPF gets an API capable of providing what I would consider a modern enterprise level translation mechanism. I just had to write it myself, but now I will share it with the world.
