# Changelogs
 **1. I made a entire new class that will handle the uploading the tokens to the external site. That way we don't have to worry about hitting the character limit for Discord and getting an error.**
 **2. Instead of using WebClient now I am using HttpClient to send/receive data.**
 **3. Made the project in .NET 5 since I was facing errors.**

# Discord Token Logger
 **This is made for educational purposes only! Don't use it towards anybody!**

# Building the project
 **Download the source, open the solution file and compile as Release**

# How it works - **Token** class
 > We declare our regex pattern which will be used to search for tokens in Discord, PTB Discord, Canary Discord and some browser destinations. Instead of using two separated ones I am using one that does the same job.

 > Scans the **.ldb** files and searches for a token. If found it is added to a [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder?view=net-6.0 "StringBuilder class").

 > Normal tokens and tokens with two-factor authentication are separated in different StringBuilders.

 > We concatenate all tokens in one variable.

 > If uploading to the external site have been successful, we are sending the link to the Discord WebHook.

# How it works - **Paste** class
 > A class where we make a POST and GET request to the external site.
 > We make POST request to the site to upload our tokens.
 > We wait for GET request to receive information and find our link using regex pattern. Afterwards we save that link in a variable.

# How it works - **Program** class
 > We create a class instance also known as Object so we can access our methods.
