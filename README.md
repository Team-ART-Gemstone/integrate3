# Integrate3!
# Setup instructions (starting from scratch):

git clone https://github.com/Team-ART-Gemstone/Hololens.git

Use HTTPS unless you have an SSH key set up already.

Open folder / project "integrate2" using Unity 2017.4.x (Unity Hub recommended for this)

Open specific scene (abcdefg for now)

# Build instructions

(need to verify w/ Abdul)

## 1st time

Menu to Edit -> Project Settings -> Player. Under Publishing Settings make sure that InternetClient, InternetClientServer, WebCam, Microphone, and SpatialPerception are all enabled. Under XR Settings make sure Virtual Reality Supported is enabled and add Windows Mixed Reality under Virtual Reality SDKs if not there.

Menu to File -> Build Settings. Make sure the main scene is included in the build. Set Platform to Universal Windows Platform and Target Device to HoloLens.

Build to "App" folder.

Then follow Common Build Steps below.

## 2nd time+

Delete all .sln files in integrate2 and integrate2/App as well as other files outside of folders.

Build to "App" folder. Then follow Common Build Steps below.

## Common Build Steps

Open the .sln file created in App using Visual Studio. Menu to Project -> Manage NuGet Packages.

Not really sure what to do here, but some keywords:
newtonsoft json library update to version 10 (or something)
uwp microsoft package
azure.cognitiveservices

Set the build toolbar to Release / x86 / Remote Machine. When prompted enter IPv4 address of HoloLens (obtained on device through Settings -> Network near the bottom of the page). Press green arrow and wait for deployment.

You may need to get the latest version of Newtonsoft and search and replace all instances of Newtonsoft in the project for it to build correctly
