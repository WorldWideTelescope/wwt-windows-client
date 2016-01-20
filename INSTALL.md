# Building and Installing the WorldWide Telescope Windows Client

Prerequisites
-------------

If you do not have Visual Studio 2015 installed, you may download the free Community Edition at http://www.visualstiudio.com

To build the installer, you will also need the Visual Studio 2015 Installer Projects add-in found here:
https://visualstudiogallery.msdn.microsoft.com/f1cc3f3e-c300-40a7-8797-c509fb8933b9

If you don't require the installer, just remove it from the solution before building.

Because of GitHub's 100mb file limit you will also have to unzip the datafiles for setup (see [Setup1/ReadMe.txt](Setup1/ReadMe.txt)).

Building
--------

After cloning the repository to your local machine (or downloading as a zip and unzipping to a directory of your choice), open the Solution:  File->Open->Project/Solution-> [local repository path\wwt-windows-client<-master>]\WWTExplorer.sln

Then: Build -> Build Solution
It will then compile the projects, and link appropriately.   

After a successful build, the WorldWide Telescope Windows Client executable will be found here: 
[local repository path\wwt-windows-client<-master>]\WWTExplorer3D\bin\Debug\WWTExplorer.exe

Happy Hacking!
