# Spatial Faceted Examine Search

This is a proof of concept project. It looks very basic, there is just a search page.
The purpose of the project is to try and get spatial searching (distance) and also faceted searching.

## Demo Site

There is a demo site to show this code working

[https://sfes.umbhost.dev/](https://sfes.umbhost.dev/)

Thanks to Aaron from [Umbhost](https://umbhost.net/) for hosting it for free for us.

## Setting up the project

When you run the project it should use unattended install to install a SQLite database
Then you can go to settings > uSync and do a full import of everything.

There is just a simple search page. There are no other doc types. The contents of the examine index comes from a file called locations.json in the wwwroot folder.

To get the search working in your project. In the Settings, go to Examine Management, click on LocationsIndex and click on Rebuild Index.

Now when you go to the front end of the site you can search using Lat and Long values to find a location.

## Left to do

It might not be the most efficient but it will work which is the main thing, and if you see improvements to be made please feel free to contribute.

## Credits

Thanks so much to [Lars-Erik Aabech](https://twitter.com/bleedo) for the code to do the spatial searching.
