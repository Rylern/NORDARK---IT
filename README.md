# NORDARK WP 5: Digital twin-based simulation and visual analytics to assess outdoor conditions

## Development

* Download [Unity](https://unity.com/) **2022.2.0f1**.
* Open the repository with Unity and the *Assets/Scene/Nordark* scene.
* If you export the project for Windows 64 bit, some computers might not recognize the *sqlite3.dll* file shipped with the executable. To fix this, replace the *NORDARK\NORDARK_Data\Plugins\x86_64\sqlite3.dll* file of the exported project with the *AdditionalResources\sqlite3.dll* file of the repository.

### Assets organization

The *Assets* folder is organized as follow:
* The *HDRPDefaultResources* folder contains HDRP settings for the project (see the *Global volume* and *HDRP* part).
* The *IES* folder contains nothing. It is automatically created by an external asset at each start of the application.
* The *Materials* folder contains a shader and a material derived from it used for luminance computation.
* The *Plugins* folder contains external librairies.
* The *Prefabs* folder contains light poles prefabs used in the scene.
* The *Resources* folder contains files that should be accesible after deployment. It contains the [Mapbox access token](https://docs.mapbox.com/help/getting-started/access-tokens/) and information specific to light poles.
* The *Scenes* folder contains the only scene of the project.
* The *Scripts* folder contains every internal script of the project.
    * The *Scripts/Managers* folder contains the managers of the application (see the *Managers* part).
    * The *Scripts/Models* folder contains several classes describing entities. These classes contain very few methods, their main goal is to provide structures for other scripts.
    * The *Scripts/Objects* folder contains scripts attached to prefabs or game objects.
    * The *Scripts/UI* folder contains scripts attached to UI game objects.
    * The *Scripts/Utils* folder contains scripts that do not fall in any of the previous categories.
* The *Textures* folder contains textures used in the splash screen of the application.
* The *Volumes* folder contains HDRP [Volumes](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/Volumes.html) settings.

### Sky (sun and moon)
In the hierarchy, the *Sky* object controls the sun and the moon of the scene, which are its two children. It has a rotation of 180° along the y-axis, to align the sun/moon position with the Mapbox map (by default, the North/South of the sun/moon are inversed with the North/South of the Mapbox map).

The sun and moon are represented by a directional light whose *Affect Physically Based Sky* option is checked. The *angular diameter*, *distance*, *temperature*, and *intensity* parameters are using real-life values. Only the sun create shadows (*Shadow Map* parameter), because only one directional light can cast shadows at a time.

The *Sky* object has a *SkyManager.cs* script attached to it. This script controls the sun/moon position and the moon light intensity:
* The sun/moon position is determined by the [SAMPA algorithm](https://midcdmz.nrel.gov/sampa/). This is a C program, and was converted to C# in the *Assets/Scripts/Utils/Sampa* folder.
* The moon light intensity changes depending on its phase. The phase is determined by the [MoonPhaseConsole algorithm](https://github.com/khalidabuhakmeh/MoonPhaseConsole), stored in the *Assets/Plugins/MoonPhaseConsole* folder.

### EventSystem
The *EventSystem* object is an internal Unity object that process and handle events in a Unity Scene. It is not directly used but should be present.

### Main camera
The *Main Camera* object is rendering what we see on the screen. The *CameraMovement.cs* script handles the translation/rotation of the camera.

### Global volume and HDRP

This part will explain the *GlobalVolume* object and how to use the High Definition Render Pipeline (HDRP).

I recommend to read [The definitive guide to lighting in the High Definition Render Pipeline (HDRP)](https://blog.unity.com/games/updated-for-2021-lts-the-definitive-guide-to-lighting-in-the-high-definition-render-pipeline) that explains what HDRP is. This pipeline was chose for this project because it uses real physical units (like Lux, Lumen…) for lights.

The *Assets/HDRPDefaultResources* folder contain the HDRP settings for the project. The different settings are explained in the guide.

These default settings can be overridden in a Unity scene with a global volume. The *GlobalVolume* object is doing that, by for example setting the sky and the camera exposure.

### Mapbox

The *Map* object is handling the creation of the terrain and the buildings with the *AbstractMap.cs* script. This script is coming from the [Mapbox Unity SDK](https://www.mapbox.com/unity), stored in the *Assets/Plugins/Mapbox* folder. It has a lot of parameters that can be modified in the Unity inspector, the most important ones being:
* The zoom level of the map.
* The extent of the map (number of tiles generated).
* The features (in the *Map layers* section). A feature is a collection of objects that will be placed on the map. In our case, we use the buildings features.

To interact with the Mapbox map, the *MapManager.cs* script was created. It is used by different scripts to:
* Initialize the map and indicate when it is loaded.
* Change the location of the map. This has an impact on other scripts (for example, the light pole positions need to be updated).
* Convert coordinates between the Unity world and the real world.

### Managers
The *Managers* object contain empty game objects with scripts attached to it. They handle everything but the UI.

#### Game manager
The *GameManager.cs* script is responsible for initializing the application. Basically, everything should be started from here (except internal initialization steps like setting up listeners on UI elements..):
* It request for the map to initialize.
* It loads everything else after the map is initialized.

#### IES Manager
The *IESManager.cs* script is responsible for defining light parameters. It is used by the *LightPolesManager.cs* script.

#### Light Poles manager
The *LightPolesManager.cs* script is responsible for setting a light pole configuration:
* A light pole is represented by the *LightPrefab* class.
* Two light configurations can be selected. They are described by the *Assets/Resources/Config1* and *Assets/Resources/Config2* files. They both contain the same number of light poles but differ by the type of light used.

#### Luminance map manager
The *LuminanceMapManager.cs* script computes and provides a luminance map. Information on luminance can be found in the theory part of this documentation. The luminance map converts what the main camera sees into colors representing different luminance values.

The *LuminanceMapManager.cs* script is enabling/disabling the legend and the *LuminanceMapPassAndVolume* object:
* The legend is described by the *LuminanceMapLegend.cs* script. It can set the value corresponding to each color.
* The *LuminanceMapPassAndVolume* object contains a [Custom Pass Volume](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/Custom-Pass.html) that controls how Unity renders the objects in a scene.
    * Unity executes a Custom Pass at a certain point during the HDRP render loop. This custom pass can be injected anywhere in the render loop, before *post process* in our case.
    * The custom pass is described by a material, itself generated by a shader. Here, the *Assets/Materials/Light Computation/FullScreen_LuminanceMapCustomPass* material is used, generated by the *Assets/Materials/Light Computation/LuminanceMapCustomPass* shader.
    * This shader is a fragment shader. We convert the pixel color into a luminance, and then convert the luminance to color. The relevant lines of code are between lines 5 and 7, and between lines 24 and 39. The legend values and colors are passed to the shader.
    * The *LuminanceMapPassAndVolume* also contains a [Volume](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@16.0/manual/Volumes.html) to disable after effects (tonemapping and bloom) because they alter the luminance computation.

### UI
The *UI* object contains the user interface. It contains two elements:
* A *Parameters* window, that is used to change some parameters of the scene. The *ParametersControl.cs* script attached to it is communicating with the managers described above.
* A *LuminanceMapLegend* object, that displays the range of colors used when the luminance map is activated. It is controlled by the *LuminanceMapLegend.cs* script.

### Splash screen
The splash screen can be changed in the player settings, that can be opened by clicking on *Edit*, *Project Settings…*, *Player* tab, and *Splash image* section. The splash images are stored in the *Assets/Textures* folder.

## Theory (luminance)
The luminance is a photometric measure of the luminous intensity per unit area of light traveling in a given direction (more information on [Wikipedia](https://en.wikipedia.org/wiki/Luminance)). It is expressed in cd/m² (candela per square meter).

The luminance can be computed from a color and the camera exposure. In a linear color space, the formula is:
```
luminance = (0.0396819152, 0.458021790, 0.00609653955) * pixelColor / exposure
```
(see the Luminance() function of [this file](https://github.com/TwoTailsGames/Unity-Built-in-Shaders/blob/master/CGIncludes/UnityCG.cginc)).

## Background

In this WP, our objective is the specification and implementation of digital twin (DT) simulators to support the design, planning, and assessment of suitable outdoor lighting environments, as well as the specification and implementation of visual analytics tools to support data analysis. DT will be used for designing operations in the two study sites (pre-intervention phase) as well as supporting the analysis of results related to post-interventions. In the pre-intervention phase, the digital twin will be used to define the proper location and operation procedures related to both envisioned lighting solutions (WP4) and traps and acoustic sensors (WP3). In this stage, the goal is to support clarification of the problem domain, anticipation of problems, and proposition of ideas and solutions, possibly mitigating research risks. In the post-intervention analysis, the DT will used to visualize psychological (WP1) and physiological (WP2) data collected from the different study sites. Information visualization/visual analytics tools will be created, deployed, and validated in the context of supporting data understanding (e.g., identification of patterns of interest, trends, and correlations with other variables – environmental and climate data).

The research questions posed by the other WPs and overall project demand the analysis of large volumes of heterogeneous data (e.g., conventional textual data, images, physiological time series, lighting sensor data, occurrence and spatial distribution, environmental and climate data, and elevation data). We expect to develop an information system for supporting the integration of these data into a single platform. This WP therefore also concerns tasks related to data modelling, data storage and retrieval. We expect to develop new as well as use state- of-the-art unsupervised approaches that exploit the similarity of entities, defined in terms of their attributes, to identify similar patterns and groups. The created information system will be exploited in WP6 and WP7 to communicate project’s results with stakeholders. Our methods for data collection and analysis consist of participatory design for the creation of DT for study sites and information visualization approaches involving interested parties; proposal and implementation of DTs and information visualization approaches for handling different lighting conditions in outdoor settings; proposal and implementation of visual analytics tools to support data analyses (the proposed approaches may benefit from recent results (Leite et al., 2016; Mariano et al., 2017; Mariano et al., 2018; Rodrigues et al., 2019)); proposal and implementation of an information system for data integration and analysis; software validation, considering the use of proposed tools in typical pre- and post- interventions scenarios. Subject-oriented evaluation protocols are expected to be employed.