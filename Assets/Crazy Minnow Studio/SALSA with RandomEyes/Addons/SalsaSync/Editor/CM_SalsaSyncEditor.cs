using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.Sync
{
    /// <summary>
    /// This is the custom inspector for CM_SalsaSync, a script that synchronizes Salsa
    /// and RandomEyes shapes to multiple objects.
    /// 
    /// Crazy Minnow Studio, LLC
    /// CrazyMinnowStudio.com
    /// 
    /// NOTE:While every attempt has been made to ensure the safe content and operation of 
    /// these files, they are provided as-is, without warranty or guarantee of any kind. 
    /// By downloading and using these files you are accepting any and all risks associated 
    /// and release Crazy Minnow Studio, LLC of any and all liability.
    [CustomEditor(typeof(CM_SalsaSync)), CanEditMultipleObjects]
    public class CM_SalsaSyncEditor : Editor
    {
        private CM_SalsaSync salsaSync; // CM_SalsaSync reference
        private bool dirtySMRs; // Other SMR dirty inspector status
        private float width = 0f; // Inspector width
        private const float deleteWidth = 30f; // Percentage
        private const float shapeNameWidth = 28f; // Percentage
        private const float smrWidth = 35f; // Percentage
        private Color bgColorDefault;
        //private readonly Color bgColorDefault = new Color(0.76f,0.76f,0.76f); // Inspector default color
        private readonly Color bgColor = new Color(0.8f,0.8f,0.8f); // Inspector alt color

        public void OnEnable()
        {
            // Get reference
            salsaSync = target as CM_SalsaSync;

            // Initialize
            if (salsaSync.initialize)
            {
                salsaSync.GetSalsa3D();
                salsaSync.GetRandomEyes3D();
                salsaSync.GetEyeLidBones();
                salsaSync.GetEyeBones();
                if (salsaSync.saySmall == null) salsaSync.saySmall = new List<CM_ShapeGroup>();
                if (salsaSync.sayMedium == null) salsaSync.sayMedium = new List<CM_ShapeGroup>();
                if (salsaSync.sayLarge == null) salsaSync.sayLarge = new List<CM_ShapeGroup>();

                salsaSync.initialize = false;
            }

            bgColorDefault = GUI.backgroundColor;
        }

        public override void OnInspectorGUI()
        {
            // Minus 45 width for the scroll bar
            width = Screen.width - 60f;

            // Make sure the CM_ShapeGroups are initialized
            if (salsaSync.saySmall == null) salsaSync.saySmall = new System.Collections.Generic.List<CM_ShapeGroup>();
            if (salsaSync.sayMedium == null) salsaSync.sayMedium = new System.Collections.Generic.List<CM_ShapeGroup>();
            if (salsaSync.sayLarge == null) salsaSync.sayLarge = new System.Collections.Generic.List<CM_ShapeGroup>();

            GUILayout.Space(10);

            #region Speech
            GUI.backgroundColor = bgColorDefault;        
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUI.indentLevel++;
                salsaSync.showSpeech = EditorGUILayout.Foldout(salsaSync.showSpeech, "Speech");
                EditorGUI.indentLevel--;
                if (salsaSync.showSpeech)
                {
                    EditorGUILayout.HelpBox("Salsa3D blendshapes and jaw bone", MessageType.None);
                    
                    GUILayout.Space(10);                                        
                    
                    EditorGUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
                    {
                        salsaSync.salsa3D = EditorGUILayout.ObjectField(
                            new GUIContent("Salsa3D", "Select your Salsa3D lip-sync instance"), salsaSync.salsa3D, typeof(Salsa3D), true) as Salsa3D;
                    }
                    EditorGUILayout.EndVertical();
                    
                    GUILayout.Space(10);            
                    
                    GUI.backgroundColor = bgColor;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        GUI.backgroundColor = bgColorDefault;   
                        EditorGUI.indentLevel++;
                        salsaSync.showSmall = EditorGUILayout.Foldout(salsaSync.showSmall, "SaySmall Shapes");
                        EditorGUI.indentLevel--;
                        if (salsaSync.showSmall)
                        {
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
                            {
                                EditorGUILayout.LabelField("Add SaySmall Shapes");
                                if (GUILayout.Button("Add Shape", new GUILayoutOption[] {GUILayout.Width(100)}))
                                {
                                    salsaSync.saySmall.Add(new CM_ShapeGroup());
                                    salsaSync.initialize = false;
                                    salsaSync.saySmall[salsaSync.saySmall.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.saySmall);
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            if (salsaSync.saySmall.Count > 0)
                            {
                                GUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Width(width)});
                                {
                                    EditorGUILayout.LabelField(
                                        new GUIContent("Del", "Remove shape"),
                                        GUILayout.Width(deleteWidth));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("SMR", "SkinnedMeshRenderer"),
                                        GUILayout.Width((smrWidth / 100) * width));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                        GUILayout.Width((shapeNameWidth / 100) * width));
                                }
                                GUILayout.EndHorizontal();

                                for (int i = 0; i < salsaSync.saySmall.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.saySmall[i].shapeIndex + ")"),
                                            GUILayout.Width(deleteWidth)))
                                        {
                                            salsaSync.saySmall.RemoveAt(i);
                                            break;
                                        }

                                        salsaSync.saySmall[i].smr = (SkinnedMeshRenderer) EditorGUILayout.ObjectField(
                                            salsaSync.saySmall[i].smr, typeof(SkinnedMeshRenderer),
                                            true, GUILayout.Width((smrWidth / 100) * width));
                                        if (salsaSync.saySmall[i].lastSmr != salsaSync.saySmall[i].smr)
                                        {
                                            salsaSync.saySmall[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.saySmall[i].smr);
                                            salsaSync.saySmall[i].lastSmr = salsaSync.saySmall[i].smr;
                                        }

                                        if (salsaSync.saySmall[i].smr)
                                        {
                                            salsaSync.saySmall[i].shapeIndex = EditorGUILayout.Popup(
                                                salsaSync.saySmall[i].shapeIndex, salsaSync.saySmall[i].shapeNameList);
                                            salsaSync.saySmall[i].shapeName =
                                                salsaSync.saySmall[i].smr.sharedMesh.GetBlendShapeName(salsaSync.saySmall[i].shapeIndex);

                                            salsaSync.initialize = false;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                    GUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("", GUILayout.Width(deleteWidth));
                                        salsaSync.saySmall[i].shapePercent = EditorGUILayout.Slider(
                                            salsaSync.saySmall[i].shapePercent, 0f, 100f);

                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(10);
        
                    GUI.backgroundColor = bgColor;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        GUI.backgroundColor = bgColorDefault;   
                        EditorGUI.indentLevel++;
                        salsaSync.showMedium = EditorGUILayout.Foldout(salsaSync.showMedium, "SayMedium Shapes");
                        EditorGUI.indentLevel--;
                        if (salsaSync.showMedium)
                        {
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
                            {
                                EditorGUILayout.LabelField("Add SayMedium Shapes");
                                if (GUILayout.Button("Add Shape", new GUILayoutOption[] {GUILayout.Width(100)}))
                                {
                                    salsaSync.sayMedium.Add(new CM_ShapeGroup());
                                    salsaSync.initialize = false;
                                    salsaSync.sayMedium[salsaSync.sayMedium.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.sayMedium);
                                    if (salsaSync.sayMedium[salsaSync.sayMedium.Count - 1].smr == null)
                                        salsaSync.sayMedium[salsaSync.sayMedium.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.saySmall);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (salsaSync.sayMedium.Count > 0)
                            {
                                GUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Width(width)});
                                {
                                    EditorGUILayout.LabelField(
                                        new GUIContent("Delete", "Remove shape"),
                                        GUILayout.Width(deleteWidth));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("SMR", "SkinnedMeshRenderer"),
                                        GUILayout.Width((smrWidth / 100) * width));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                        GUILayout.Width((shapeNameWidth / 100) * width));
                                }
                                GUILayout.EndHorizontal();

                                for (int i = 0; i < salsaSync.sayMedium.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.sayMedium[i].shapeIndex + ")"),
                                            GUILayout.Width(deleteWidth)))
                                        {
                                            salsaSync.sayMedium.RemoveAt(i);
                                            break;
                                        }

                                        salsaSync.sayMedium[i].smr = (SkinnedMeshRenderer) EditorGUILayout.ObjectField(
                                            salsaSync.sayMedium[i].smr, typeof(SkinnedMeshRenderer),
                                            true, GUILayout.Width((smrWidth / 100) * width));
                                        if (salsaSync.sayMedium[i].lastSmr != salsaSync.sayMedium[i].smr)
                                        {
                                            salsaSync.sayMedium[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.sayMedium[i].smr);
                                            salsaSync.sayMedium[i].lastSmr = salsaSync.sayMedium[i].smr;
                                        }

                                        if (salsaSync.sayMedium[i].smr)
                                        {
                                            salsaSync.sayMedium[i].shapeIndex = EditorGUILayout.Popup(
                                                salsaSync.sayMedium[i].shapeIndex, salsaSync.sayMedium[i].shapeNameList);
                                            salsaSync.sayMedium[i].shapeName =
                                                salsaSync.sayMedium[i].smr.sharedMesh.GetBlendShapeName(salsaSync.sayMedium[i].shapeIndex);

                                            salsaSync.initialize = false;
                                        }
                                    }
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    {
                                        salsaSync.sayMedium[i].shapePercent = EditorGUILayout.Slider(
                                            salsaSync.sayMedium[i].shapePercent, 0f, 100f);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    GUILayout.Space(10);
        
                    GUI.backgroundColor = bgColor;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        GUI.backgroundColor = bgColorDefault;   
                        EditorGUI.indentLevel++;
                        salsaSync.showLarge = EditorGUILayout.Foldout(salsaSync.showLarge, "SayLarge Shapes");
                        EditorGUI.indentLevel--;
                        if (salsaSync.showLarge)
                        {
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                            {
                                EditorGUILayout.LabelField("Add SayLarge Shapes");
                                if (GUILayout.Button("Add Shape", new GUILayoutOption[] { GUILayout.Width(100) }))
                                {
                                    salsaSync.sayLarge.Add(new CM_ShapeGroup());
                                    salsaSync.initialize = false;
                                    salsaSync.sayLarge[salsaSync.sayLarge.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.sayLarge);
                                    if (salsaSync.sayLarge[salsaSync.sayLarge.Count - 1].smr == null)
                                        salsaSync.sayLarge[salsaSync.sayLarge.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.sayMedium);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (salsaSync.sayLarge.Count > 0)
                            {
                                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(width) });
                                {
                                    EditorGUILayout.LabelField(
                                        new GUIContent("Delete", "Remove shape"),
                                        GUILayout.Width(deleteWidth));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("SMR", "SkinnedMeshRenderer"),
                                        GUILayout.Width((smrWidth / 100) * width));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                        GUILayout.Width((shapeNameWidth / 100) * width));
                                }
                                GUILayout.EndHorizontal();
            
                                for (int i = 0; i < salsaSync.sayLarge.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.sayLarge[i].shapeIndex + ")"),
                                            GUILayout.Width(deleteWidth)))
                                        {
                                            salsaSync.sayLarge.RemoveAt(i);
                                            break;
                                        }
                                        salsaSync.sayLarge[i].smr = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                                            salsaSync.sayLarge[i].smr, typeof(SkinnedMeshRenderer),
                                            true, GUILayout.Width((smrWidth / 100) * width));
                                        if (salsaSync.sayLarge[i].lastSmr != salsaSync.sayLarge[i].smr)
                                        {
                                            salsaSync.sayLarge[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.sayLarge[i].smr);
                                            salsaSync.sayLarge[i].lastSmr = salsaSync.sayLarge[i].smr;
                                        }
                                        if (salsaSync.sayLarge[i].smr)
                                        {
                                            salsaSync.sayLarge[i].shapeIndex = EditorGUILayout.Popup(
                                                salsaSync.sayLarge[i].shapeIndex, salsaSync.sayLarge[i].shapeNameList);
                                            salsaSync.sayLarge[i].shapeName =
                                                salsaSync.sayLarge[i].smr.sharedMesh.GetBlendShapeName(salsaSync.sayLarge[i].shapeIndex);
            
                                            salsaSync.initialize = false;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
            
                                    GUILayout.BeginHorizontal();
                                    {
                                        salsaSync.sayLarge[i].shapePercent = EditorGUILayout.Slider(
                                            salsaSync.sayLarge[i].shapePercent, 0f, 100f);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                    
                    GUI.backgroundColor = bgColorDefault;
                    
                    GUILayout.Space(20);
                    
                    EditorGUILayout.HelpBox("Use this section to sync Salsa3D lip-sync shapes to a jaw bone or jaw BlendShapes.", MessageType.None);
        
                    GUILayout.Space(10);
        
                    GUI.backgroundColor = bgColor;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        GUI.backgroundColor = bgColorDefault;
                        salsaSync.jawBoneAxis = (CM_SalsaSync.RotationAxis)EditorGUILayout.EnumPopup(
                            new GUIContent("Jaw Local Rotation Axis", "Local Up Rotation Axis"),
                            salsaSync.jawBoneAxis);
            
                        salsaSync.jawBone = (Transform)EditorGUILayout.ObjectField("Jaw Bone", salsaSync.jawBone, typeof(Transform), true);
            
                        salsaSync.jawRangeOfMotion = EditorGUILayout.Slider("Jaw Range of Motion", salsaSync.jawRangeOfMotion, 0f, 1f);
            
                        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                        {
                            EditorGUILayout.LabelField("Jaw Shapes");
                            if (GUILayout.Button("Add Shape", new GUILayoutOption[] { GUILayout.Width(100) }))
                            {
                                salsaSync.jawShapes.Add(new CM_ShapeGroup());
                                salsaSync.initialize = false;
                                salsaSync.jawShapes[salsaSync.jawShapes.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.jawShapes);
                                if (salsaSync.jawShapes[salsaSync.jawShapes.Count - 1].smr == null)
                                    salsaSync.jawShapes[salsaSync.jawShapes.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.sayLarge);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        if (salsaSync.jawShapes.Count > 0)
                        {
                            GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(width) });
                            {
                                EditorGUILayout.LabelField(
                                    new GUIContent("Del", "Remove shape"),
                                    GUILayout.Width(deleteWidth));
                                EditorGUILayout.LabelField(
                                    new GUIContent("SMR", "SkinnedMeshRenderer"),
                                    GUILayout.Width((smrWidth / 100) * width));
                                EditorGUILayout.LabelField(
                                    new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                    GUILayout.Width((shapeNameWidth / 100) * width));
                            }
                            GUILayout.EndHorizontal();
            
                            for (int i = 0; i < salsaSync.jawShapes.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button(
                                        new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.jawShapes[i].shapeIndex + ")"),
                                        GUILayout.Width(deleteWidth)))
                                    {
                                        salsaSync.jawShapes.RemoveAt(i);
                                        break;
                                    }
                                    salsaSync.jawShapes[i].smr = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                                        salsaSync.jawShapes[i].smr, typeof(SkinnedMeshRenderer),
                                        true, GUILayout.Width((smrWidth / 100) * width));
                                    if (salsaSync.jawShapes[i].lastSmr != salsaSync.jawShapes[i].smr)
                                    {
                                        salsaSync.jawShapes[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.jawShapes[i].smr);
                                        salsaSync.jawShapes[i].lastSmr = salsaSync.jawShapes[i].smr;
                                    }
                                    if (salsaSync.jawShapes[i].smr)
                                    {
                                        salsaSync.jawShapes[i].shapeIndex = EditorGUILayout.Popup(
                                            salsaSync.jawShapes[i].shapeIndex, salsaSync.jawShapes[i].shapeNameList);
                                        salsaSync.jawShapes[i].shapeName =
                                            salsaSync.jawShapes[i].smr.sharedMesh.GetBlendShapeName(salsaSync.jawShapes[i].shapeIndex);
            
                                        salsaSync.initialize = false;
                                    }
                                }
                                GUILayout.EndHorizontal();
            
                                GUILayout.BeginHorizontal();
                                {
                                    salsaSync.jawShapes[i].shapePercent = EditorGUILayout.Slider(
                                        salsaSync.jawShapes[i].shapePercent, 0f, 100f);
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            #endregion

            GUILayout.Space(10);

            #region Eyes
            GUI.backgroundColor = bgColorDefault;
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUI.indentLevel++;
                salsaSync.showEyes = EditorGUILayout.Foldout(salsaSync.showEyes, "Eyes");
                EditorGUI.indentLevel--;
                if (salsaSync.showEyes)
                {
                    EditorGUILayout.HelpBox("RandomEyes3D eye and eyelid blendshapes and bones", MessageType.None);
        
                    GUILayout.Space(10);
        
                    salsaSync.re3DEyes = EditorGUILayout.ObjectField(
                        new GUIContent("RandomEyes3D Eyes", "The RandomEyes3D instance for controlling eye movement."),
                        salsaSync.re3DEyes, typeof(RandomEyes3D), true) as RandomEyes3D;
                    GUILayout.Space(10);
        
                    GUI.backgroundColor = bgColor;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        GUI.backgroundColor = bgColorDefault;
                        salsaSync.eyelidControl = (CM_SalsaSync.AnimControl)EditorGUILayout.EnumPopup(
                            new GUIContent("Eyelid Control", "Eyelid control using BlendShapes or eye bones?"),
                            salsaSync.eyelidControl);
            
                        if (salsaSync.eyelidControl == CM_SalsaSync.AnimControl.BlendShapes)
                        {
                            GUILayout.Space(10);
            
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                            {
                                EditorGUILayout.LabelField("Blink Shapes");
                                if (GUILayout.Button("Add Shape", new GUILayoutOption[] { GUILayout.Width(100) }))
                                {
                                    salsaSync.blink.Add(new CM_ShapeGroup());
                                    salsaSync.initialize = false;
                                    salsaSync.blink[salsaSync.blink.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.blink);
                                    if (salsaSync.blink[salsaSync.blink.Count - 1].smr == null)
                                        salsaSync.blink[salsaSync.blink.Count - 1].smr = salsaSync.GetLastSMR(salsaSync.sayLarge);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (salsaSync.blink.Count > 0)
                            {
                                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(width) });
                                {
                                    EditorGUILayout.LabelField(
                                        new GUIContent("Delete", "Remove shape"),
                                        GUILayout.Width(deleteWidth));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("SMR", "SkinnedMeshRenderer"),
                                        GUILayout.Width((smrWidth / 100) * width));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                        GUILayout.Width((shapeNameWidth / 100) * width));
                                }
                                GUILayout.EndHorizontal();
            
                                for (int i = 0; i < salsaSync.blink.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.blink[i].shapeIndex + ")"),
                                            GUILayout.Width(deleteWidth)))
                                        {
                                            salsaSync.blink.RemoveAt(i);
                                            break;
                                        }
                                        salsaSync.blink[i].smr = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                                            salsaSync.blink[i].smr, typeof(SkinnedMeshRenderer),
                                            true, GUILayout.Width((smrWidth / 100) * width));
                                        if (salsaSync.blink[i].lastSmr != salsaSync.blink[i].smr)
                                        {
                                            salsaSync.blink[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.blink[i].smr);
                                            salsaSync.blink[i].lastSmr = salsaSync.blink[i].smr;
                                        }
                                        if (salsaSync.blink[i].smr)
                                        {
                                            salsaSync.blink[i].shapeIndex = EditorGUILayout.Popup(
                                                salsaSync.blink[i].shapeIndex, salsaSync.blink[i].shapeNameList);
                                            salsaSync.blink[i].shapeName =
                                                salsaSync.blink[i].smr.sharedMesh.GetBlendShapeName(salsaSync.blink[i].shapeIndex);
            
                                            salsaSync.initialize = false;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
            
                                    GUILayout.BeginHorizontal();
                                    {
                                        salsaSync.blink[i].shapePercent = EditorGUILayout.Slider(
                                            salsaSync.blink[i].shapePercent, 0f, 100f);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                        else if (salsaSync.eyelidControl == CM_SalsaSync.AnimControl.Bones)
                        {
                            salsaSync.eyelidBoneAxis = (CM_SalsaSync.RotationAxis)EditorGUILayout.EnumPopup(
                                new GUIContent("Eyelid Local Down Rotation Axis", "Local Down Rotation Axis"),
                                salsaSync.eyelidBoneAxis);
            
                            salsaSync.leftEyelidBone = EditorGUILayout.ObjectField(
                                "Eyelid Bone Left", salsaSync.leftEyelidBone, typeof(Transform), true) as Transform;
                            salsaSync.rightEyelidBone = EditorGUILayout.ObjectField(
                                "Eyelid Bone Right", salsaSync.rightEyelidBone, typeof(Transform), true) as Transform;
            
                            salsaSync.eyelidRangeOfMotion = EditorGUILayout.Slider("Eyelid Range of Motion", salsaSync.eyelidRangeOfMotion, 0f, 1f);
                        }
                    }
                    EditorGUILayout.EndVertical();
        
                    GUILayout.Space(20);
        
                    GUI.backgroundColor = bgColor;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        GUI.backgroundColor = bgColorDefault;
                        salsaSync.eyeControl = (CM_SalsaSync.AnimControl)EditorGUILayout.EnumPopup(
                            new GUIContent("Eye Control", "Eye control using BlendShapes or eye bones?"),
                            salsaSync.eyeControl);
            
                        if (salsaSync.eyeControl == CM_SalsaSync.AnimControl.BlendShapes)
                        {
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                            {
                                EditorGUILayout.LabelField("LookUp Shapes");
                                if (GUILayout.Button("Add Shape", new GUILayoutOption[] { GUILayout.Width(100) }))
                                {
                                    salsaSync.lookUp.Add(new CM_ShapeGroup());
                                    salsaSync.initialize = false;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (salsaSync.lookUp.Count > 0)
                            {
                                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(width) });
                                {
                                    EditorGUILayout.LabelField(
                                        new GUIContent("Delete", "Remove shape"),
                                        GUILayout.Width(deleteWidth));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("SMR", "SkinnedMeshRenderer"),
                                        GUILayout.Width((smrWidth / 100) * width));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                        GUILayout.Width((shapeNameWidth / 100) * width));
                                }
                                GUILayout.EndHorizontal();
            
                                for (int i = 0; i < salsaSync.lookUp.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.lookUp[i].shapeIndex + ")"),
                                            GUILayout.Width(deleteWidth)))
                                        {
                                            salsaSync.lookUp.RemoveAt(i);
                                            break;
                                        }
                                        salsaSync.lookUp[i].smr = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                                            salsaSync.lookUp[i].smr, typeof(SkinnedMeshRenderer),
                                            true, GUILayout.Width((smrWidth / 100) * width));
                                        if (salsaSync.lookUp[i].lastSmr != salsaSync.lookUp[i].smr)
                                        {
                                            salsaSync.lookUp[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.lookUp[i].smr);
                                            salsaSync.lookUp[i].lastSmr = salsaSync.lookUp[i].smr;
                                        }
                                        if (salsaSync.lookUp[i].smr)
                                        {
                                            salsaSync.lookUp[i].shapeIndex = EditorGUILayout.Popup(
                                                salsaSync.lookUp[i].shapeIndex, salsaSync.lookUp[i].shapeNameList);
                                            salsaSync.lookUp[i].shapeName =
                                                salsaSync.lookUp[i].smr.sharedMesh.GetBlendShapeName(salsaSync.lookUp[i].shapeIndex);
            
                                            salsaSync.initialize = false;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
            
                                    GUILayout.BeginHorizontal();
                                    {
                                        salsaSync.lookUp[i].shapePercent = EditorGUILayout.Slider(
                                            salsaSync.lookUp[i].shapePercent, 0f, 100f);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
            
                            GUILayout.Space(10);
            
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                            {
                                EditorGUILayout.LabelField("LookDown Shapes");
                                if (GUILayout.Button("Add Shape", new GUILayoutOption[] { GUILayout.Width(100) }))
                                {
                                    salsaSync.lookDown.Add(new CM_ShapeGroup());
                                    salsaSync.initialize = false;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (salsaSync.lookDown.Count > 0)
                            {
                                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(width) });
                                {
                                    EditorGUILayout.LabelField(
                                        new GUIContent("Delete", "Remove shape"),
                                        GUILayout.Width(deleteWidth));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("SMR", "SkinnedMeshRenderer"),
                                        GUILayout.Width((smrWidth / 100) * width));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                        GUILayout.Width((shapeNameWidth / 100) * width));
                                }
                                GUILayout.EndHorizontal();
            
                                for (int i = 0; i < salsaSync.lookDown.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.lookDown[i].shapeIndex + ")"),
                                            GUILayout.Width(deleteWidth)))
                                        {
                                            salsaSync.lookDown.RemoveAt(i);
                                            break;
                                        }
                                        salsaSync.lookDown[i].smr = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                                            salsaSync.lookDown[i].smr, typeof(SkinnedMeshRenderer),
                                            true, GUILayout.Width((smrWidth / 100) * width));
                                        if (salsaSync.lookDown[i].lastSmr != salsaSync.lookDown[i].smr)
                                        {
                                            salsaSync.lookDown[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.lookDown[i].smr);
                                            salsaSync.lookDown[i].lastSmr = salsaSync.lookDown[i].smr;
                                        }
                                        if (salsaSync.lookDown[i].smr)
                                        {
                                            salsaSync.lookDown[i].shapeIndex = EditorGUILayout.Popup(
                                                salsaSync.lookDown[i].shapeIndex, salsaSync.lookDown[i].shapeNameList);
                                            salsaSync.lookDown[i].shapeName =
                                                salsaSync.lookDown[i].smr.sharedMesh.GetBlendShapeName(salsaSync.lookDown[i].shapeIndex);
            
                                            salsaSync.initialize = false;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
            
                                    GUILayout.BeginHorizontal();
                                    {
                                        salsaSync.lookDown[i].shapePercent = EditorGUILayout.Slider(
                                            salsaSync.lookDown[i].shapePercent, 0f, 100f);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
            
                            GUILayout.Space(10);
            
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                            {
                                EditorGUILayout.LabelField("LookLeft Shapes");
                                if (GUILayout.Button("Add Shape", new GUILayoutOption[] { GUILayout.Width(100) }))
                                {
                                    salsaSync.lookLeft.Add(new CM_ShapeGroup());
                                    salsaSync.initialize = false;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (salsaSync.lookLeft.Count > 0)
                            {
                                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(width) });
                                {
                                    EditorGUILayout.LabelField(
                                        new GUIContent("Delete", "Remove shape"),
                                        GUILayout.Width(deleteWidth));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("SMR", "SkinnedMeshRenderer"),
                                        GUILayout.Width((smrWidth / 100) * width));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                        GUILayout.Width((shapeNameWidth / 100) * width));
                                }
                                GUILayout.EndHorizontal();
            
                                for (int i = 0; i < salsaSync.lookLeft.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.lookLeft[i].shapeIndex + ")"),
                                            GUILayout.Width(deleteWidth)))
                                        {
                                            salsaSync.lookLeft.RemoveAt(i);
                                            break;
                                        }
                                        salsaSync.lookLeft[i].smr = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                                            salsaSync.lookLeft[i].smr, typeof(SkinnedMeshRenderer),
                                            true, GUILayout.Width((smrWidth / 100) * width));
                                        if (salsaSync.lookLeft[i].lastSmr != salsaSync.lookLeft[i].smr)
                                        {
                                            salsaSync.lookLeft[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.lookLeft[i].smr);
                                            salsaSync.lookLeft[i].lastSmr = salsaSync.lookLeft[i].smr;
                                        }
                                        if (salsaSync.lookLeft[i].smr)
                                        {
                                            salsaSync.lookLeft[i].shapeIndex = EditorGUILayout.Popup(
                                                salsaSync.lookLeft[i].shapeIndex, salsaSync.lookLeft[i].shapeNameList);
                                            salsaSync.lookLeft[i].shapeName =
                                                salsaSync.lookLeft[i].smr.sharedMesh.GetBlendShapeName(salsaSync.lookLeft[i].shapeIndex);
            
                                            salsaSync.initialize = false;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
            
                                    GUILayout.BeginHorizontal();
                                    {
                                        salsaSync.lookLeft[i].shapePercent = EditorGUILayout.Slider(
                                            salsaSync.lookLeft[i].shapePercent, 0f, 100f);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
            
                            GUILayout.Space(10);
            
                            EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                            {
                                EditorGUILayout.LabelField("LookRight Shapes");
                                if (GUILayout.Button("Add Shape", new GUILayoutOption[] { GUILayout.Width(100) }))
                                {
                                    salsaSync.lookRight.Add(new CM_ShapeGroup());
                                    salsaSync.initialize = false;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            if (salsaSync.lookRight.Count > 0)
                            {
                                GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(width) });
                                {
                                    EditorGUILayout.LabelField(
                                        new GUIContent("Delete", "Remove shape"),
                                        GUILayout.Width(deleteWidth));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("SMR", "SkinnedMeshRenderer"),
                                        GUILayout.Width((smrWidth / 100) * width));
                                    EditorGUILayout.LabelField(
                                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                                        GUILayout.Width((shapeNameWidth / 100) * width));
                                }
                                GUILayout.EndHorizontal();
            
                                for (int i = 0; i < salsaSync.lookRight.Count; i++)
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this shape from the list (index:" + salsaSync.lookRight[i].shapeIndex + ")"),
                                            GUILayout.Width(deleteWidth)))
                                        {
                                            salsaSync.lookRight.RemoveAt(i);
                                            break;
                                        }
                                        salsaSync.lookRight[i].smr = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                                            salsaSync.lookRight[i].smr, typeof(SkinnedMeshRenderer),
                                            true, GUILayout.Width((smrWidth / 100) * width));
                                        if (salsaSync.lookRight[i].lastSmr != salsaSync.lookRight[i].smr)
                                        {
                                            salsaSync.lookRight[i].shapeNameList = salsaSync.GetShapeNames(salsaSync.lookRight[i].smr);
                                            salsaSync.lookRight[i].lastSmr = salsaSync.lookRight[i].smr;
                                        }
                                        if (salsaSync.lookRight[i].smr)
                                        {
                                            salsaSync.lookRight[i].shapeIndex = EditorGUILayout.Popup(
                                                salsaSync.lookRight[i].shapeIndex, salsaSync.lookRight[i].shapeNameList);
                                            salsaSync.lookRight[i].shapeName =
                                                salsaSync.lookRight[i].smr.sharedMesh.GetBlendShapeName(salsaSync.lookRight[i].shapeIndex);
            
                                            salsaSync.initialize = false;
                                        }
                                    }
                                    GUILayout.EndHorizontal();
            
                                    GUILayout.BeginHorizontal();
                                    {
                                        salsaSync.lookRight[i].shapePercent = EditorGUILayout.Slider(
                                            salsaSync.lookRight[i].shapePercent, 0f, 100f);
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                        else if (salsaSync.eyeControl == CM_SalsaSync.AnimControl.Bones)
                        {
                            salsaSync.eyeBoneUpAxis = (CM_SalsaSync.RotationAxis)EditorGUILayout.EnumPopup(
                                new GUIContent("Eye Local Up Rotation Axis", "Local Up Rotation Axis"),
                                salsaSync.eyeBoneUpAxis);
                            salsaSync.eyeBoneRightAxis = (CM_SalsaSync.RotationAxis)EditorGUILayout.EnumPopup(
                                new GUIContent("Eye Local Right Rotation Axis", "Local Right Rotation Axis"),
                                salsaSync.eyeBoneRightAxis);
            
                            salsaSync.leftEyeBone = EditorGUILayout.ObjectField(
                                "Eye Bone Left", salsaSync.leftEyeBone, typeof(Transform), true) as Transform;
                            salsaSync.rightEyeBone = EditorGUILayout.ObjectField(
                                "Eye Bone Right", salsaSync.rightEyeBone, typeof(Transform), true) as Transform;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            #endregion

            GUILayout.Space(10);

            #region Expression
            GUI.backgroundColor = bgColorDefault;
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUI.indentLevel++;
                salsaSync.showExpression = EditorGUILayout.Foldout(salsaSync.showExpression, "Expressions");
                EditorGUI.indentLevel--;
                if (salsaSync.showExpression)
                {
                    EditorGUILayout.HelpBox("If you're using a RandomEyes3D instance for expressions, link your RandomEyes3D expression " +
                                            "instance and add additional SkinnedMeshRenderers (SMR's) below to automatically sync expression shapes to other " +
                                            "SMR's (Beards, Moustaches, Teeth, etc.)", MessageType.None);
        
                    GUILayout.Space(10);
        
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("RandomEyes3D Expressions");
                        salsaSync.re3DExpressions = (RandomEyes3D)EditorGUILayout.ObjectField(salsaSync.re3DExpressions, typeof(RandomEyes3D), true);
                    }
                    EditorGUILayout.EndHorizontal();
        
                    GUILayout.Space(10);
                    
                    GUI.backgroundColor = bgColor;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        GUI.backgroundColor = bgColorDefault;
                        EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                        {
                            EditorGUILayout.LabelField("Other SkinnedMeshRenderers");
                            if (GUILayout.Button("Add SMR", new GUILayoutOption[] { GUILayout.Width(100) }))
                            {
                                salsaSync.otherSMRs.Add(new SkinnedMeshRenderer());
                            }
                        }
                        EditorGUILayout.EndHorizontal();
            
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                if (salsaSync.otherSMRs.Count > 0)
                                {
                                    for (int i = 0; i < salsaSync.otherSMRs.Count; i++)
                                    {
                                        GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(width) });
                                        if (GUILayout.Button(
                                            new GUIContent("X", "Remove this SkinnedMeshRenderer from the list"),
                                            new GUILayoutOption[] { GUILayout.Width(deleteWidth) }))
                                        {
                                            salsaSync.otherSMRs.RemoveAt(i);
                                            dirtySMRs = true;
                                        }
                                        if (!dirtySMRs) salsaSync.otherSMRs[i] = EditorGUILayout.ObjectField(
                                            salsaSync.otherSMRs[i], typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
                                        GUILayout.EndHorizontal();
                                    }
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            #endregion
        }
    }
}
