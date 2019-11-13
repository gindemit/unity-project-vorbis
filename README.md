# Unity project Vorbis
This is a Unity project that use the [C wrapper around Vorbis libraries](https://github.com/khindemit/unity-vorbis) to save and load audio data in Vorbis format.

## Test and run
To run this project you need to build the C wrapper library for your target platform.
#### Tested platforms:
- Windows
- Android
- iOS

Detailed instructions for building the C wrapper native libraries you'll find [here](https://github.com/khindemit/unity-vorbis)
After you build the C libraries, you need to copy it to Assets/Plugins folder:
#### Windows:
- Assets/Plugins/Windows/x86_64/VorbisPlugin.dll
#### Android: 
- Assets/Plugins/Android/libs/arm64-v8a/libVorbisPlugin.so 
- Assets/Plugins/Android/libs/armeabi-v7a/libVorbisPlugin.so 
- Assets/Plugins/Android/libs/x86/libVorbisPlugin.so 
- Assets/Plugins/Android/libs/x86_64/libVorbisPlugin.so
#### iOS:
- Assets/Plugins/iOS/libVorbisPlugin.dylib
