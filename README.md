# Welcome to EarthBelow
EarthBelow is an application designed for iOS and Android handheld devices, serving as an educational tool to convey earth-related information to users through augmented reality. The project aims to prototype an augmented reality application that allows the user to explore the earth both geologically and geographically via the physical environment. The application is built from scratch and serves as a starting point for future development based on evaluation and refinement through continuous usability study.

# System Manual
## Development/Deployment Environment:
- Unity 2019.1.5f1
- MacOS Catalina Beta
- Xcode 11 Beta/Android Studio 3.4.2
- iOS 13 Beta/Android 9.0

## Required Packages
- AR Foundation 2.2
- ARKit XR Plugin 2.2
- ARCore XR Plugin 2.1
- Shader Graph 5.6.1
- Lighweight RP 5.6.1
- Core RP 5.6.1

## Documentation
### Scenes
| Scene Name | Description |
| - | - |
| Main | Core Application Scene |

### Scirpts
| Script Name | Type | Description |
| - | :-: | - |
| CORE | Logic | Global Variables: Data, Object <br> I/O Functions: Texture Loader, JSON Loader <br> Algorithms: ECEF Conversion, Haversine  <br> |
| INIT | Logic | Initialisation: Reading Online Database |
| ModeManager | Logic | Mode Control: Switching Mode |
| EarthPreviewer | Mode | AR Globe: Reading GPS, Spawning the Globe |
| PitSpawner | Mode | Pit Inspector: Plane Detection, Placing the Pit |
| EarthMapping | Mode | Earth Mapping: Plane Detection, Mapping the Earth |
| UIManager | UI | UI Control: Fading In/Out |
| ButtonMovement | UI | Button Animations |
| MainMenuControl | UI | Main Menu Control |

### Other Assets
| Folder Name | Description |
| - | - |
| Animation | UI Element Animations |
| Fonts | UI Typefaces |
| LWRPSupport | LWRP Support Files |
| Materials | Shaders, LWRP Materials |
| Models | Custom Meshes |
| Prefabs | Reusable Game Objects |
| StreamingAssets | Local Resources, Databases |
| Textures | Textures, Sprites and Icons |