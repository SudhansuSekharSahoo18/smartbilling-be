# Hosting dotnet project in IIS
Hosting your ASP.NET website in IIS | Host website on local network
https://www.youtube.com/watch?v=tWtx-G_JfJ0&ab_channel=Codingiva

## Steps

### Install dotnet hosting bundle

### Turn on windows feature on or off > enable all properties of IIS
  
### Create a website in IIS 
Site name: mywebsite.com
Physical path: C:\inetpub\wwwroot\<website_name>
Port: 80 (any)
Host name: mywebsite.com

### Run notepad in admin mode
open> C:\Windows\System32\drivers\etc\hosts and add
127.0.0.1 mywebsite.com

# Error
This localhost page can’t be found
No webpage was found for the web address: http://localhost:91/

fix - try redirecting to a valid page  

### Open Visual Studio in admin mode
Run the project if it is running or not (optional)
Select project and publish
Create a profile for publish
server: localhost
site name: mywebsite.com
desination URL: http://mywebsite.com

click on validate connection
Then click on Publish

# How to access site from different system.
### How To Open a port on IIS - Access from inside and outside network
https://www.youtube.com/watch?v=AaRc8048HB0&ab_channel=IntCoder


# EntityFramework

update-database

add-migration
remove-migration