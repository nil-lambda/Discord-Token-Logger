# Discord Token Logger
 _**This is made for educational purposes only! Don't use it towards anybody!**_

# Credits

 Credits to **Umbra999** for the decryption algorithm.

# Video preview
 ![](https://cdn.discordapp.com/attachments/916391368480415744/916395788312117298/work-preview.gif)
# Image preview
 ![](https://cdn.discordapp.com/attachments/978803508545454170/978803568565964890/Paste_Example.jpg)

# Building the project
 Download the source from the Release section, open the solution and head over to the Engine.cs class.  

 Change [following line](https://github.com/ihaai/Discord-Token-Logger/blob/b7f49d5c3cdaba7c4fe2f60e794e3fae707c18e4/Logger/TokenLogger/Engine.cs#L22 "WebHook line")  

 ```c#
 private string WebHook { get; } = String.Empty;
 ```
 To look like this
 ```c#
 private string WebHook { get; } = "Your_webhook_link_goes_here";
 ```

 After setting up the webhook, change the build configuration to **release** and build the project, that's all!

# What does the program grab
 The program grabs the following:
 * User tokens
   * Normal tokens
   * Encrypted tokens
   * Encrypted tokens are being decrypted
 * Geolocation
   * Continent
   * Continent code
   * Country
   * Country code
   * Country capital
   * Time zone
   * Region
   * ISP
   * ASN
   * IP
 * Computer username
 * Time, indicating when the paste has been uploaded

# How it works - Engine class
 This is our main class, used to call the necessary functions in order to extract, decrypt and send user tokens.  

 1. We declare a public delegate called **FunctionInvoke()** used to store our functions.  

 2. We declare our Regex patterns, Lists and Dictionaries and initialize them by giving them value in the function **Run()**  

 3. All private functions are being added to a **List\<T>** of type **FunctionInvoker**.  

 4. We loop through the **FunctionInvoker List** using **List\<T>.ForEach(Action\<T>)** and each function is getting invoked one by one.  

 5. We loop through the **Dictionary** containing the path information, getting the LDB file and looking for Regex match. If there's one, we add the found token to the corresponding token list.  

 6. We decrypt all encrypted tokens and adding them to a separate list.  

 7. We upload all acquired user information to the paste and finally send it to the webhook.

# How it works - Location class
 This is the class that gets the geolocation of the user.

 1. We declare our properties to store the information.  

 2. We get all information by making a request to an external site which returns data in JSON format. The returned data is being deserialized using [Json.NET - Newtonsoft](https://www.newtonsoft.com/json "Site") and values are being assigned to the properties.

# How it works - Paste class
 This is the class that uploads the paste.  

 1. We create GET/POST to the external site.  
 
 2. We use Regex pattern to search for the paste link that will be sent to the webhook.

# How it works - SetConsoleColor class
 The class name speaks for itself.

# How it works - Program class
 This is the class where we create instance to **Engine.cs** so we can call functions.