# Building and Installing the WorldWide Telescope Windows Client

Prerequisites
-------------

If you do not have Visual Studio 2013 installed, you may download the free Community Edition at http://www.visualstiudio.com
Note: there are some minor issues building with Visual Studio 2015 products due to changes in Microsoft's C Runtime and C++ Standard Library.

To build the installer, you will also need the Visual Studio 2013 Installer Projects add-in found here:
https://visualstudiogallery.msdn.microsoft.com/9abe329c-9bba-44a1-be59-0fbf6151054d
Note: There is a similar installer add-in for the Visual Studio 2015 products.
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
