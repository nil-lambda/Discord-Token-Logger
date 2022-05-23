# Discord Token Logger
 _**This is made for educational purposes only! Don't use it towards anybody!**_

# Credits

 Credits to **Umbra999** for the decryption algorithm.

# Video preview
 ![](https://cdn.discordapp.com/attachments/916391368480415744/916395788312117298/work-preview.gif)
# Image preview
 ![](https://cdn.discordapp.com/attachments/916391368480415744/918147943121448990/image_preview.png)

# Building the project
 Download the source, open the solution file and head over to the Token.cs class  
   
 Find the following line  
 ```c#
 public readonly string webHook = string.Empty;
 ```
 Change it so it looks like this
 ```cs
 public readonly string webHook = "your_webhook_link_hoes_here";
 ```

# What does it grab
 It grabs Discord tokens (normal and mfa), Geolocation and current PC username.

# How it works - **Token** class
 **1.** We declare our regex pattern which will be used to search for tokens in Discord, PTB Discord, Canary Discord and some browser destinations. Instead of using two separated ones I am using one that does the same job.

 **2.** Scans the **.ldb** files and searches for a token. If found it is added to a [StringBuilder](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder?view=net-6.0 "StringBuilder class").

 **3.** Normal tokens and tokens with two-factor authentication are separated in different StringBuilders.

 **4.** We concatenate all tokens in one variable.

 **5.** If uploading to the external site have been successful, we are sending the link to the Discord WebHook.

# How it works - **Paste** class
 **1.** A class where we make a POST and GET request to the external site.

 **2.** We make POST request to the site to upload our tokens.

 **3.** We wait for GET request to receive information and find our link using regex pattern. Afterwards we save that link in a variable.

# How it works - **Location** class
 **1.** We make a special method only to extract the IP address so we can use it later.

 **2.** We deserializate JSON to a Object to get the required information such as continent, country, region, ASN, ISP.


 JSON to Object deserialization example
 ```c#
 public class Account
 {
     public string Email { get; set; }
     public bool Active { get; set; }
     public DateTime CreatedDate { get; set; }
     public IList<string> Roles { get; set; }
 }
 ```

 ```c#
 string json = @"{
  'Email': 'james@example.com',
  'Active': true,
  'CreatedDate': '2013-01-20T00:00:00Z',
  'Roles': [
    'User',
    'Admin'
  ]
}";

Account account = JsonConvert.DeserializeObject<Account>(json);

Console.WriteLine(account.Email);
// Output: james@example.com
 ```

# How it works - **Program** class
 **1.** We create a class instance also known as Object so we can access our methods
