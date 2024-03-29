<div id="top" align="center">
<h1 align="center">Pedagogical Agent (a procedural character animation system)</h1>

  <p align="center">
    A research project during my master's degree at Purdue University; It involves the implementation
    of character procedural animation used in virtual learning environment.
    <br />
    <a href="https://youtu.be/iXXYbPlqv1s">Full Demo</a>
  </p>
</div>

<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a></li>
    <li><a href="#getting-started">Getting Started</a></li>
    <li>
    <a href="#overview">Overview</a>
      <ul>
        <li><a href="#master-controller">Master Controller</a></li>
        <li><a href="#xml-reader">XML Reader</a></li>
        <li><a href="#beat-detection">Beat Detection</a></li>
        <li><a href="#emotion-input">Emotion Input</a></li>
      </ul>
    </li>
    <li><a href="#user-interface">User Interface</a></li>
    <li><a href="#debug-gui">Debug GUI</a></li>
    <li><a href="#character-setup">Character Setup</a></li>
    <li><a href="#scene-structure">Scene Structure</a></li>
    <li><a href="#file-structure">File Structure</a></li>
      <ul>
        <li><a href="#overview-of-folder">Overview of Folder</a></li>
        <li><a href="#resource-folder">Resource Folder</a></li>
        <li><a href="#character-based-assets">Character-based Assets</a></li>
      </ul>
    <li><a href="#character-components">Character Components</a></li>
      <ul>
        <li><a href="#animator-components">Animator Component</a></li>
        <li><a href="#salsa-3d-plugin">Salsa 3D Plugin</a></li>
        <li><a href="#final-ik-plugin">Final IK Plugin</a></li>
        <li><a href="#run-time-script">Run-time Script</a></li>
      </ul>
    <li><a href="#event-system">Event System</a></li>
      <ul>
        <li><a href="#system-components">System Components</a></li>
        <li><a href="#custom-scripts">Custom Scripts</a></li>
        <li><a href="#run-time-event">Run-time Event</a></li>
      </ul>
    <li><a href="#plugin">Plugin</a></li>
  </ol>
</details>

## About The Project

__Project:__ Multimodal Affective Pedagogical Agents for Different Types of Learners

__Faculty:__ Nicoletta Adamo, Purdue (PI), Richard E. Mayer, UCSB (co-PI), Bedrich Benes, Purdue (co-PI)

__Sponsor:__ NSF - IIS - Cyberlearning, award # 1821894 (2018-2021), Purdue Instructional Innovation Grant (2018- 2020)

http://hpcg.purdue.edu/idealab/AffectiveAgents/

## Getting Started

> The source project is based on Unity 2018.3.6f1, the build is tested on Win10x64

Unity Components:

Download the [release package](https://github.com/leixingyu/pedagogical-agent/releases/tag/v0.4.0) which consists of both the final build and source code.

Emotion Recognition:

[__Emotion Recognition Overview__](https://www.notion.so/emotion-recognition-2f2c082af6ba465fbb300c0d8d7e537c)
|
[__Emotion Recognition Documentation__](https://www.notion.so/Emotion-Recognition-Deployment-Instruction-6f853779e8664479812f4ee8bb999249)

## Overview
<p align="right"><a href="#top">back to top</a></p>

The animated agent is controlled by four high-level controller:

<img src="https://i.imgur.com/4sxHtvU.jpg" alt="overview" height="65%" width="65%">

### Master Controller

- **Body Gesture**: Mecanim accesses the built-in state-machine in Unity. 
The procedurally generated body gesture is achieved by pose interpolation – the transitioning between any two predetermined poses sequentially.

    Body Offset generate randomness as well as emotion-based body variation such as leaning forward/backward, body contract/expand. 
It contains a IK-based shoulder and hand control, a FK-based spine offset and a COG hip control.


- **Facial Deformation**: Facial Expression has access to predetermined emotion including happy, bored, angry and content.
The component blends naturally between each blendshapes. 

    Salsa Plugin enables facial automation including: eye-blink, gaze and lip-sync based on audio input.
Other subcomponents include eyebrow raise during speech generated algorithmically by [Beat Detection](#beat-detection).


- **Hand and Foot**: The agent's hand gesture is layered on top of body gesture when performing actions like pointing. 
It has pre-determined hand shapes like relax, holding fist, stretched palm.

    The foot position similar to hand shape can be locked to the ground, 
free (inherit motion from mocap data), and procedural auto-stepping (not-implemented).

### XML Reader 

This XML-based script works like a behavioral markup language that controls the agent during the speech. 
It specifies events to trigger on certain moments: such as emotional changes, 
or body and hand gesture transition. It also controls when to switch PowerPoint, and when to detect user's [Emotion Input](#emotion-input).

### Beat Detection

This functionality analyzes the spectrum data of audio input and detects beat in real-time. 
The detection is based on an algorithm that calculates the average sound energy and triggers when a variance greater
when certain threshold has occurred. 
When a beat is detected, a series of Beat Events are invoked. Idle Events are invoked after the Beat Events are completed. (More on [Event System](#event-system))

### Emotion Input

This is the core mechanic used to communicate with the user. The XML Reader sends requests for emotional state updates. 
The Emotion Update components communicate with the Python `emotion_recognition` which constantly checks the emotion and stores it in a queue. 
When a request for an emotional state update is received, the most frequent emotion in that queue is returned. 
The Emotion Input triggers emotional based events based on the return results.

## User Interface
<p align="right"><a href="#top">back to top</a></p>

<img src="https://i.imgur.com/THfWylf.png" alt="overview" height="65%" width="65%">

| Menu Item  | Operation |
| ------------- | ------------- |
| Debug |               Open up the run-time GUI for debugging  |
| Component Checkbox  | Toggle on/off controller components  |
|Reload |			    Restart the scene
|Pause/Play |			Pause/Continue playing the scene
|Slider |				Adjust camera distance from full-body to close-up
|Arrow button |			Control slides
|Character button |		Switch character and restart the scene

## Debug GUI
<p align="right"><a href="#top">back to top</a></p>

<img src="https://i.imgur.com/WowAc7Y.png" alt="overview" height="65%" width="65%">

| Menu Item  | Operation |
| ------------- | ------------- |
|Facial Expression| 	Change facial expression
|Foot lock|		Toggle foot position lock to ground or free moving
|Hand pose|		Change hand pose left or right separately
|Body parameter| 	Modify animation speed, body rotation and blend time
|Body pose| 		Switch poses

## Character Setup
<p align="right"><a href="#top">back to top</a></p>

### Character Requirement
- **Joint**: ideally no more than 40 joints
- **Mesh**: no sub-mesh
- **Size**: 200mb and less

### Character Clean-up
The clean-up process ensures that the `.fbx` is accepted in Unity, 
which is basically a transfer from animation rig to a game rig

1. The output should be a character with a single mesh skinned to a single joint hierarchy [H-Anim Standard].
2. Make sure that all end joints have zeroed-out rotation and unit scaling.
3. Whenever the character has updated blendshapes, it needs to be re-imported

### Character Template
The character template is used in MotionBuilder after identifying key joint components,
this will smoothen the retargeting process.

### Character Import
The character `.fbx` should be placed in the character folder, 
adjust `Animation Type` to `Humanoid`, make sure `Avatar Definition` is set to `Create From this Model`, then configure the joint as needed.

Existing character settings:

- David Latest version: v2 
scaling: 1.4
- Luna Latest version: v3
scaling: 12

(Note: all scale settings need to be adjusted in scene view, not in character configuration)

## Scene Structure

<p align="right"><a href="#top">back to top</a></p>

### Overview of Hierarchy

| Hierarchy  | Description |
| ------------- | ------------- |
|Event System| 	Scene related control (GUI, audio control, global logic)
|Character| 		Character related control components (animation behavior)
|Environment| 		3D environment models
|Lighting|		Lighting components
|Canvas| 		Predefined user interface
|MainCam|		Master camera

### Character
| Hierarchy  | Description |
| ------------- | ------------- |
|BaseMesh| 		Skinned mesh (has skinned mesh renderer for blendshapes)
|Skeleton| 		Character joint hierarchy
|Foot IKs| 		Pre-defined IK handle for locking the foot


## File Structure

<p align="right"><a href="#top">back to top</a></p>

### Overview of Folder
| Hierarchy  | Description |
| ------------- | ------------- |
|Character| 		Character mask, avatar definition and rig (`.fbx`)
|Plugins| 		All plugin used (Salsa, Mecanim and Final IK)
|Resources| 		Assets accessed during run-time
|Scenes|		Game scene (one for each character)
|Scripts| 		All custom scripts
|Texture| 		Materials and textures

### Resource Folder
| Hierarchy  | Description |
| ------------- | ------------- |
|Audio|			In-game speech audio file
|Controller| 		Animator logic controller (mecanim state machine)
|HandShape|		Predefined hand pose (`.fbx`)
|MotionLibrary| 	Predefined body pose (`.fbx`)
|not-in-use| 		PowerPoint slide not-in-use
|Slide| 			PowerPoint slide texture
|XML| 			character behavioral scripts

### Character-based Assets

Character sub assets have individual folders, which can be modified (see `Global.cs`)

```csharp
// character animation clips location
public static string lunaAnim = "MotionLibrary/Luna";
public static string davidAnim = "MotionLibrary/David";

// character controller location
public static string lunaController  = "Controller/Luna_controller";
public static string davidController = "Controller/David_controller";

// subsequence audio location
public static string lunaAudio = "Audio/Luna";
public static string davidAudio = "Audio/David";
```

## Character Components

<p align="right"><a href="#top">back to top</a></p>

### Animator Component

`Animator` component is required for a state machine-based animation transition. 
Using `MecanimControl.cs` evokes different states in `Animator` for gesture transition.

- Leave `Controller` blank for it will be assigned by `MecanimControl.cs` in run-time
- Set the `Avatar` to the latest character definition
- Uncheck `Apply Root Motion`

### Salsa 3D Plugin 

**Queue Processor**: Required component to use and monitor Salsa event queue.

**Eyes**: Salsa Eyes component is used to create random eye shifts, eye blinks

- `Properties` control random eye-gaze shift, adjust physical range and frequency
- `Eyelid properties` controls the random eye-blinks, adjust frequency
- To use random eye and fix axis, unpack character prefab

**Salsa**: Salsa Lip-sync, used for creating automatic lip-sync based on audio input

- Adjust blendshapes for visemes
- Adjust visemes trigger threshold
- Attach corresponding audio file

### Final IK Plugin

**Full body biped IK**: This component is used to create full body IK setup; 
foot IK is used to lock feet in place; COG/Shoulder/Hand IK can be used to create 
body offset to alter the character’s gesture style

- Assign corresponding bones
- Parent foot effector under character hierarchy
- Set parameters such as iterations (2 is fine)

### Run-time Script

**Mecanim Control**: assigns the appropriate `Controller` to the `Animator`, 
creates two states, and assigns different clips to these two states during run-time, 
achieving any state to any state transition. (see `MecanimControl.cs`)

**Expression Control**: change character’s blendshape which in result,
controls the facial expression (see `ExpressionControl.cs`)

**Hand Control**: switch a character's hand pose. 
The hand pose is controlled by a sub-layer (with a character mask) in the animator controller, 
it needs to be manually modified and connected.

**Foot Control**: used to lock or unlock the character’s foot in-place. 
It is done by accessing the `FinalIK` component.

**Body Effector Setup**: provided by the Final IK package, this component is 
required for the following body offset features (`MainOffset` and `SpineOffset`)

**MainOffset**: controls body parts such as Hand and Shoulder by adjusting its 
translation value to create body moving outward or inward. (see `mainOffset.cs`)

**SpineOffset**: controls body part mainly Spine by adjusting its rotation value 
to create body leaning forward or backward. (see `spineOffset.cs`)


## Event System

<p align="right"><a href="#top">back to top</a></p>

### System Components

**Event System, Standalone Input Module & Base Input (added at run-time)**:
These components are required for GUI related controls.

**Audio Source**:
This component is responsible for playing audio, it also serves as the source for Salsa Lip-sync, 
controlled by `audioControl.cs`.

- `AudioClip` is assigned and adjusted at run-time
- Output is affected by a certain `Audio Mixer` group, which needs to be manually assigned

### Custom Scripts

**Canvas Manager**:
responsible for creating the GUI and connecting its functionality.

**Master Control**:
is the core and the wrapper of all controls, it initializes most of the run-time components. 
(see `masterControl.cs`)

**XML Reader**:
takes an `.xml` file, the file specifies scripted behavior of the character, 
and the behavior will be layered on top.

**Beat Detection (Disabled by default)**:
takes the current playing audio from the `AudioSource`, 
detects its beat and triggers `onBeat()` event. 
The `onIdle()` event is triggered whenever the beat duration is reached.

**Emotion Input**:
when a check signal is sent by XML Reader,
the module requests an emotion state from the `EmotionUpdate.cs` and performs an according reaction.

### Run-time Event

**Slide Control**:
controls the change of PowerPoint slide displayed in the background.

**Audio Control**:
switches the audio clip for `AudioSource`, the audio clips are switched based on the change of slide as of now.

**Emotion Update**:
is added by `EmotionInput.cs`. It communicates with the emotion detection module, 
detects every 2 seconds for 10 seconds, it then returns the most 
frequent emotion detected back to Emotion Input components.


## Plugin

<p align="right"><a href="#top">back to top</a></p>

[Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290)

[Salsa Suite](https://assetstore.unity.com/packages/tools/animation/salsa-lipsync-suite-148442)

[Mecanim Control](https://assetstore.unity.com/packages/tools/animation/mecanim-control-15156)

