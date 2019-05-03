# Reference Finder
Find all the places you have used a content or media item in the backoffice.

## Introduction
Koben.ReferenceFinder is a plugin for finding all the places a content or media item has been used as a property value.

Users are able to easily search content or media through a Dashboard in the Backoffice.

## Installation

Koben.ReferenceFinder will need to be installed on the Umbraco instance you want to perform a search on.

### Nuget
[![NuGet](https://buildstats.info/nuget/Koben.ReferenceFinder)](https://www.nuget.org/packages/Koben.ReferenceFinder/)

You can run the following command from within Visual Studio:

    PM> Install-Package Koben.ReferenceFinder

### Umbraco Package
TBD

### Manually
Download the code, compile it and copy the Reference Finder binary and the App_Plugins folder into your Umbraco website App_Plugin folder.

## Configuration
This plugin does not require any extra configuration to work.

## Known issues
Search for media by URL does not currently work. Please use the browse button the select media as a work around.

## Planned Features


## Umbraco Versions
ReferenceFinder has been tested with Umbraco versions:
- 7.3.0
- 7.14.0

More testing on earlier versions is planned and contributors are more than welcome here :)
Testing on new releases will be performed as they come out.

## Changelog
### 0.1.1
- Init alpha release version.
- Nuget work.
- README work.
- Upped assembly version to 0.1.1.

### 0.1.2
- Added ability to search by document type
- Improved media and content browsing
- Styling work
- Testing and QA
- Nuspec work
- Umbraco package work
- Added CI scripts 
- README work.

### 0.1.3
- README work.
- Nuget package description fix

Handmade by Samuel Butler - 2019 @KobenDigital
