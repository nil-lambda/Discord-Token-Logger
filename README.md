# Discord Token Logger
 **This is made for educational purposes only! Don't use it towards anybody!**

# Building the project
 **Since this was made in .NET 6.0, to build the project you will need to have .NET 6.0 installed. Alternatively if you don't have it installed, you can copy and paste all of the code in older version of .NET.**

# A little bit of information
 **To use a little characters as possible I am using the following abbreviations:**
 > D - Discord

 > P - PTB Discord

 > C - Canary Discord

 > B(B) - Brave Browser

 > B(G) - Chrome

 > B(I) - Iridium Browser

 > Two-Factor Authentication tokens are being sent with different Webhook username.

# How it works - **Token** class
 > We declare our regex pattern which will be used to search for tokens in Discord, PTB Discord, Canary Discord and some browser destinations. Instead of using two separated ones I am using one that does the same job.

 > Scans the **.ldb** files and searches for a token. If found it is added to a [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder?view=net-6.0 "StringBuilder class").

 > Normal tokens and tokens with two-factor authentication are separated in different StringBuilders.

 > When all found tokens are added we're moving to the part where we're sending the tokens to a Discord Webhook.

 > There is a **limitation**. If there are over 2000 characters the tokens will not be sent.

# How it works - **Program** class
 > We create a class instance also known as Object so we can access our methods.

# How it works - **Using** class
 > A class for **Global Using Statements**. Read more about it [here](https://dotnetcoretutorials.com/2021/08/19/global-using-statements-in-c10/ "Global Using Statements").
