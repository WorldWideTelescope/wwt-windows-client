# WWT Translation Files

This directory contains translation files for localizing AAS WorldWide Telescope into different
languages. Even if you're using WWT in English, a translation file is used, meaning that these
files are also what you should probably be editing to change UI text in the application.

**However:** the Windows app does not actually use the files stored in this directory! It pulls
translations from the cloud. The list of available translations is downloaded from:

http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=Languages

The contents of that file ultimately come from the blob `languages.xml` in the `catalog` container
of the `wwtfiles` storage account. On releases we sync that blob with the file of the same name in
this directory.

That XML file points to other URLs, generally of the form:

http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=lang_en

Those in turn come from blobs named like `lang_en.txt` (note the extension) in the same container.
Those likewise need to be updated on the cloud to make changes appear in the app.

## Other Notes

To test changes locally, use the "Load Local Language Pack" option of the "Settings -> Select Your
Language" menu item.

The `Usage.tdf` file in this directory contains some notes about the semantics of the different
items to be translated, but it's not very developed.
