-----------------
Version 2.6.0
-----------------
CM_SalsaSync is designed to be used in congunction with SALSA with RandomEyes, and a wide range of different character models. It works with models that contain one or more BlendShape enabled objects. It is also designed to work with both eye BlendShapes or eye bones.

Crazy Minnow Studio, LLC
CrazyMinnowStudio.com

This workflow is documented at the following URL, along with a downloadable zip file that contains the supporting files.

http://crazyminnowstudio.com/posts/salsasync-for-custom-models/


Package Contents
----------------
Crazy Minnow Studio/SALSA with RandomEyes/Addons/
	SalsaSync
		Editor
			CM_SalsaSyncEditor.cs
				Custom inspector for CM_SalsaSync.cs
		CM_SalsaSync.cs
			Helper script to apply Salsa and RandomEyes BlendShape data to custom character BlendShapes.
		ReadMe.txt
			This readme file.


Installation Instructions
-------------------------
1. Install SALSA with RandomEyes into your project.
	Select [Window] -> [Asset Store]
	Once the Asset Store window opens, select the download icon, and download and import [SALSA with RandomEyes].

2. Import the SALSA with RandomEyes custom character support package (SalsaSync).
	Select [Assets] -> [Import Package] -> [Custom Package...]
	Browse to the [SALSA_Addon_SalsaSync.unitypackage] file and [Open].


Usage Instructions
------------------
1. Add a custom character to your scene.

2. Add Salsa3D and RandomEyes3D to the root of your custom character, but don't link to any SkinnedMeshRenderer component.

3. Select the character root, then select:
	[Component] -> [Crazy Minnow Studio] -> [SALSA] -> [Addons] -> [CM_SalsaSync]
	This will add the CM_SalsaSync component that is necessary for synchonizing BlendShapes across multiple objects.

4. Map your applicable BlendShapes and bones.

5. Add your dialogue audio file to the Salsa3D [Audio Clip] field.


What [CM_SalsaSync.cs] does
-------------------------------------
1. It alows you to redirect SALSA's BlendShape value output to multiple shapes on one or more SkinnedMeshRenderer components and bones.