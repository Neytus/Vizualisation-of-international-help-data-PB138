# Vizualisation-of-international-help-data-PB138

## Usage:
The zipped release folder contains 2 folders: app and web.
* app folder - contains the backend of the project. In order to start it, the user can use BackendProject.exe file. All the functionalities are accessible through this GUI application, which has 4 buttons:
     * Download/Update webpages - Accesses the IATI registry and obtains links to all webpages containing required data.
     * Download XML data - Downloads all the XML files located on those webpages.
     * Extract XML data - Parses all downloaded XMLs and merges the important information into a single XML file. This file is then used to create a json file. 
     * Open visualization - Utilizing the json file, it opens a web application that visualizes collected data. Not all browsers do support this function (web server has to be used).
* web folder - contains the frontend of the project. Index.html should be accessible from the domain for the application to work correctly. 
