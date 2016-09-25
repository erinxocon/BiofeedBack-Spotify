# BiofeedBack-Spotify
A basic biofeedback arduino sketch with a c# program to integrate the data with spotify.  Calm yourself!

##Usage
Compile the c# file, changing the comm port to the one your arduino is using.  Load up the arduino code to your board and then the magic happens!  As long as spotify is open the playlist will change based on the heart rate.  You can set the heart rate threshold and playlist uri's in the c# file.  Depends on https://github.com/JohnnyCrazy/SpotifyAPI-NET for spotify integration, make sure you use nuget to install before compiling.
