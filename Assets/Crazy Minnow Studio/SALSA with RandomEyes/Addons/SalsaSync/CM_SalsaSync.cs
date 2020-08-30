using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.Sync
{
    /// <summary>
    /// This is a script that synchronizes Salsa
    /// and RandomEyes shapes to multiple objects.
    /// 
    /// Crazy Minnow Studio, LLC
    /// CrazyMinnowStudio.com
    /// 
    /// NOTE:While every attempt has been made to ensure the safe content and operation of 
    /// these files, they are provided as-is, without warranty or guarantee of any kind. 
    /// By downloading and using these files you are accepting any and all risks associated 
    /// and release Crazy Minnow Studio, LLC of any and all liability.
    [AddComponentMenu("Crazy Minnow Studio/SALSA/Addons/SalsaSync/CM_SalsaSync")]
    public class CM_SalsaSync : MonoBehaviour
    {
        public Salsa3D salsa3D; // Salsa3D mouth component
        public enum AnimControl { BlendShapes, Bones }
        public enum RotationAxis { X_positive, Y_positive, Z_positive, X_negative, Y_negative, Z_negative };
        public List<CM_ShapeGroup> saySmall = new List<CM_ShapeGroup>(); // saySmall shape group
        public List<CM_ShapeGroup> sayMedium = new List<CM_ShapeGroup>(); // sayMedium shape group
        public List<CM_ShapeGroup> sayLarge = new List<CM_ShapeGroup>(); // sayLarge shape group
        public List<CM_ShapeGroup> jawShapes = new List<CM_ShapeGroup>(); // Optional jaw shapes
        public Transform jawBone; // Optional jaw bone
        [Range(0f, 1f)]
        public float jawRangeOfMotion = 0.5f; // Jaw bone range of motion
        public RotationAxis eyeBoneUpAxis = RotationAxis.X_negative; // Specify the eye bone up axis
        public RotationAxis eyeBoneRightAxis = RotationAxis.Y_positive; // Specify the eye bone right axis
        public RotationAxis eyelidBoneAxis = RotationAxis.X_negative; // Specify the eyelid bone down axis
        public RotationAxis jawBoneAxis = RotationAxis.X_positive; // Specify the eye bone up axis
        public AnimControl eyelidControl = AnimControl.BlendShapes; // Specify eyelid control type (BlendShapes or Bones)
        public AnimControl eyeControl = AnimControl.BlendShapes; // Specify eye control type (BlendShapes or Bones)
        public RandomEyes3D re3DEyes; // RandomEyes3D eye instance
        public List<CM_ShapeGroup> blink = new List<CM_ShapeGroup>(); // blink shape group
        public List<CM_ShapeGroup> lookUp = new List<CM_ShapeGroup>(); // lookUp shape group
        public List<CM_ShapeGroup> lookDown = new List<CM_ShapeGroup>(); // lookDown shape group
        public List<CM_ShapeGroup> lookLeft = new List<CM_ShapeGroup>(); // lookLeft shape group
        public List<CM_ShapeGroup> lookRight = new List<CM_ShapeGroup>(); // lookRight shape group
        [Range(0f, 1f)]
        public float eyelidRangeOfMotion = 0.5f; // Eyelid bone range of motion
        public Transform leftEyelidBone; // Left eyelid bone
        public Transform rightEyelidBone; // Right eyelid bone
        public Transform leftEyeBone; // Left eye bone
        public Transform rightEyeBone; // Right eye bone
        public string[] jawBoneNames = new string[] { "jaw" }; // Used in search for jaw bone
        public string[] leftEyelidBoneNames = new string[] { "uplid.l" }; // Used in search for left eyelid bone
        public string[] rightEyelidBoneNames = new string[] { "uplid.r" }; // Used in search for right eyelid bone
        public string[] leftEyeBoneNames = new string[] { "lEye", "lefteye", "eye.l" }; // Used in search for left eye bone
        public string[] rightEyeBoneNames = new string[] { "rEye", "righteye", "eye.r" }; // Used in search for right eye bone

        public RandomEyes3D re3DExpressions; // RandomEyes3D expression instance
        public List<SkinnedMeshRenderer> otherSMRs = new List<SkinnedMeshRenderer>(); // Other SMR's to be sync'd
        private float srcShapeWeight = 0f; // Shape weight for sync'ing shapes
        private int syncIndex = -1; // BlendShape index to be sync'd

        public bool initialize = true; // Initialize once
        public bool showSmall = false; // Collapsible editor section
        public bool showMedium = false; // Collapsible editor section
        public bool showLarge = false; // Collapsible editor section
        public bool showSpeech = true; // Collapsible editor section
        public bool showEyes = true; // Collapsible editor section
        public bool showExpression = true; // Collapsible editor section

        private Transform[] children; // For searching through child objects during initialization
        private float eyeSensativity = 500f; // Eye movement reduction from shape value to bone transform value
        private float blinkWeight; // Blink weight is applied to the body Blink_Left and Blink_Right BlendShapes
        private float vertical; // Vertical eye bone movement amount
        private float horizontal; // Horizontal eye bone movement amount
        private bool lockShapes; // Used to allow access to shape group shapes when SALSA is not talking
        private Vector3 jawAxis = Vector3.zero; // Jaw bone rotation axis
        private float jawAmount; // Jaw bone amount is a scaled value from 0-100(Range of Motion) down to 0-30
        private Vector3 jawStartRot = Vector3.zero; // Stores the jaw start rotation
        private Vector3 jawNewRot = Vector3.zero; // Stores the jaw new rotation
        private Vector3 eyelidAxis = Vector3.zero; // Eyelid bone rotation axis
        private float eyelidAmount; // Eyelid bone amount is a scaled value from 0-100(Range of Motion down to 0-35
        private Vector3 eyelidStartRot = Vector3.zero; // Stores the eyelid start rotation
        private Vector3 eyelidNewRot = Vector3.zero; // Store the eyelid new rotation
        private Vector3 lookUpDownAxis = new Vector3(0f, 0f, 0f); // Specify the Up/Down axis vector
        private Vector3 lookLeftRightAxis = new Vector3(0f, 0f, 0f); // Specify the Left/Right axis vector
        private Vector3 lookUpDownAmount = new Vector3(0f, 0f, 0f); // Temporary Up/Down amount
        private Vector3 lookLeftRightAmount = new Vector3(0f, 0f, 0f); // Temporary Left/Right amount
        private Vector3 lookAmounts = new Vector3(0f, 0f, 0f); // Combined Up/Down and Left/Right amounts
        private Vector3 eyeStartRotL = Vector3.zero; // Stores the left eye start rotation
        private Vector3 eyeStartRotR = Vector3.zero; // Stores the right eye start 

        /// <summary>
        /// Reset the component to default values
        /// </summary>
        void Reset()
        {
            initialize = true;
            GetSalsa3D();
            GetRandomEyes3D();
            GetJawBone();
            GetEyeLidBones();
            GetEyeBones();
            if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
            if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
            if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
        }

        /// <summary>
        /// Initial setup
        /// </summary>
		void Start()
        {
            GetSalsa3D();
            GetRandomEyes3D();
            GetEyeLidBones();
            GetEyeBones();
            if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
            if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
            if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();

            if (jawBone)
            {
                jawStartRot = jawBone.localEulerAngles;
            }

            if (leftEyelidBone)
            {
                eyelidStartRot = leftEyelidBone.localEulerAngles;
            }
            if (rightEyelidBone)
            {
                eyelidStartRot = rightEyelidBone.localEulerAngles;
            }

            if (leftEyeBone)
            {
                eyeStartRotL = leftEyeBone.localEulerAngles;                
            }
            if (rightEyeBone)
            {
                eyeStartRotR = rightEyeBone.localEulerAngles;
            }
        }

        /// <summary>
        /// Perform the blendshape changes in LateUpdate for mechanim compatibility
        /// </summary>
		void LateUpdate()
        {
            if (salsa3D && lockShapes)
            {
                // Sync SALSA shapes
                for (int i = 0; i < saySmall.Count; i++)
                {
                    saySmall[i].shapeValue = ((saySmall[i].shapePercent / 100) * salsa3D.sayAmount.saySmall);
                    saySmall[i].smr.SetBlendShapeWeight(saySmall[i].shapeIndex, saySmall[i].shapeValue);
                }
                for (int i = 0; i < sayMedium.Count; i++)
                {
                    sayMedium[i].shapeValue = ((sayMedium[i].shapePercent / 100) * salsa3D.sayAmount.sayMedium);
                    sayMedium[i].smr.SetBlendShapeWeight(sayMedium[i].shapeIndex, sayMedium[i].shapeValue);
                }
                for (int i = 0; i < sayLarge.Count; i++)
                {
                    sayLarge[i].shapeValue = ((sayLarge[i].shapePercent / 100) * salsa3D.sayAmount.sayLarge);
                    sayLarge[i].smr.SetBlendShapeWeight(sayLarge[i].shapeIndex, sayLarge[i].shapeValue);
                }

                if (jawBone)
                {
                    switch (jawBoneAxis)
                    {
                        case RotationAxis.X_negative:
                            jawAxis = new Vector3(-1, 0, 0);
                            break;
                        case RotationAxis.X_positive:
                            jawAxis = new Vector3(1, 0, 0);
                            break;
                        case RotationAxis.Y_negative:
                            jawAxis = new Vector3(0, -1, 0);
                            break;
                        case RotationAxis.Y_positive:
                            jawAxis = new Vector3(0, 1, 0);
                            break;
                        case RotationAxis.Z_negative:
                            jawAxis = new Vector3(0, 0, -1);
                            break;
                        case RotationAxis.Z_positive:
                            jawAxis = new Vector3(0, 0, 1);
                            break;
                    }

                    jawAmount = ScaleRange(salsa3D.average, 0.0001f, 0.012f, 0f, 15f);
                    jawNewRot = jawStartRot + (jawAxis * (jawAmount * jawRangeOfMotion));
                    jawBone.transform.localEulerAngles = jawNewRot;
                }

                for (int i = 0; i < jawShapes.Count; i++)
                {
                    jawAmount = ScaleRange(salsa3D.average, 0.0001f, 0.012f, 0f, 100f);
                    jawShapes[i].shapeValue = jawAmount * (jawShapes[i].shapePercent / 100);
                    jawShapes[i].smr.SetBlendShapeWeight(jawShapes[i].shapeIndex, jawShapes[i].shapeValue);
                }
            }

            // Toggle shape lock to provide access to shape group shapes when SALSA is not talking
            if (salsa3D)
            {
                if (salsa3D.sayAmount.saySmall == 0f && salsa3D.sayAmount.sayMedium == 0f && salsa3D.sayAmount.sayLarge == 0f)
                {
                    lockShapes = false;
                }
                else
                {
                    lockShapes = true;
                }
            }

            // Sync Blink
            if (re3DEyes)
            {
                blinkWeight = re3DEyes.lookAmount.blink;

                if (eyelidControl == AnimControl.BlendShapes)
                {
                    // Apply blink action
                    for (int i = 0; i < blink.Count; i++)
                    {
                        blink[i].smr.SetBlendShapeWeight(blink[i].shapeIndex, (blinkWeight * blink[i].shapePercent) / 100);
                    }
                }
                else if (eyelidControl == AnimControl.Bones)
                {
                    if (leftEyelidBone || rightEyelidBone)
                    {
                        switch (eyelidBoneAxis)
                        {
                            case RotationAxis.X_negative:
                                eyelidAxis = new Vector3(-1, 0, 0);
                                break;
                            case RotationAxis.X_positive:
                                eyelidAxis = new Vector3(1, 0, 0);
                                break;
                            case RotationAxis.Y_negative:
                                eyelidAxis = new Vector3(0, -1, 0);
                                break;
                            case RotationAxis.Y_positive:
                                eyelidAxis = new Vector3(0, 1, 0);
                                break;
                            case RotationAxis.Z_negative:
                                eyelidAxis = new Vector3(0, 0, -1);
                                break;
                            case RotationAxis.Z_positive:
                                eyelidAxis = new Vector3(0, 0, 1);
                                break;
                        }

                        eyelidAmount = ScaleRange(re3DEyes.lookAmount.blink, 0f, 100f, 0f, 35f);
                        eyelidNewRot = eyelidStartRot + (eyelidAxis * (eyelidAmount * eyelidRangeOfMotion));
                        if (leftEyelidBone) leftEyelidBone.transform.localEulerAngles = eyelidNewRot;
                        if (rightEyelidBone) rightEyelidBone.transform.localEulerAngles = eyelidNewRot;
                    }
                }

                if (eyeControl == AnimControl.BlendShapes)
                {
                    for (int i = 0; i < lookUp.Count; i++)
                    {
                        lookUp[i].smr.SetBlendShapeWeight(lookUp[i].shapeIndex, re3DEyes.lookAmount.lookUp);
                    }
                    for (int i = 0; i < lookDown.Count; i++)
                    {
                        lookDown[i].smr.SetBlendShapeWeight(lookDown[i].shapeIndex, re3DEyes.lookAmount.lookDown);
                    }
                    for (int i = 0; i < lookLeft.Count; i++)
                    {
                        lookLeft[i].smr.SetBlendShapeWeight(lookLeft[i].shapeIndex, re3DEyes.lookAmount.lookLeft);
                    }
                    for (int i = 0; i < lookRight.Count; i++)
                    {
                        lookRight[i].smr.SetBlendShapeWeight(lookRight[i].shapeIndex, re3DEyes.lookAmount.lookRight);
                    }
                }
                else if (eyeControl == AnimControl.Bones)
                {
                    // Apply look amount to bone rotation
                    if (leftEyeBone || rightEyeBone)
                    {
                        // Apply eye movement weight direction variables
                        if (re3DEyes.lookAmount.lookUp > 0)
                            vertical = (re3DEyes.lookAmount.lookUp / eyeSensativity) * re3DEyes.rangeOfMotion;
                        if (re3DEyes.lookAmount.lookDown > 0)
                            vertical = -(re3DEyes.lookAmount.lookDown / eyeSensativity) * re3DEyes.rangeOfMotion;
                        if (re3DEyes.lookAmount.lookLeft > 0)
                            horizontal = -(re3DEyes.lookAmount.lookLeft / eyeSensativity) * re3DEyes.rangeOfMotion;
                        if (re3DEyes.lookAmount.lookRight > 0)
                            horizontal = (re3DEyes.lookAmount.lookRight / eyeSensativity) * re3DEyes.rangeOfMotion;

                        lookUpDownAxis = new Vector3(0f, 0f, 0f); // Specify the Up/Down axis vector
                        lookLeftRightAxis = new Vector3(0f, 0f, 0f); // Specify the Left/Right axis vector
                        lookUpDownAmount = new Vector3(0f, 0f, 0f); // Temporary Up/Down amount
                        lookLeftRightAmount = new Vector3(0f, 0f, 0f); // Temporary Left/Right amount
                        lookAmounts = new Vector3(0f, 0f, 0f); // Combined Up/Down and Left/Right amounts

                        switch (eyeBoneUpAxis)
                        {
                            case RotationAxis.X_positive:
                                lookUpDownAxis = new Vector3(1f, 0f, 0f);
                                lookUpDownAmount = lookUpDownAxis * vertical;
                                break;
                            case RotationAxis.Y_positive:
                                lookUpDownAxis = new Vector3(0f, 1f, 0f);
                                lookUpDownAmount = lookUpDownAxis * vertical;
                                break;
                            case RotationAxis.Z_positive:
                                lookUpDownAxis = new Vector3(0f, 0f, 1f);
                                lookUpDownAmount = lookUpDownAxis * vertical;
                                break;
                            case RotationAxis.X_negative:
                                lookUpDownAxis = new Vector3(1f, 0f, 0f);
                                lookUpDownAmount = lookUpDownAxis * -vertical;
                                break;
                            case RotationAxis.Y_negative:
                                lookUpDownAxis = new Vector3(0f, 1f, 0f);
                                lookUpDownAmount = lookUpDownAxis * -vertical;
                                break;
                            case RotationAxis.Z_negative:
                                lookUpDownAxis = new Vector3(0f, 0f, 1f);
                                lookUpDownAmount = lookUpDownAxis * -vertical;
                                break;
                        }
                        switch (eyeBoneRightAxis)
                        {
                            case RotationAxis.X_positive:
                                lookLeftRightAxis = new Vector3(1f, 0f, 0f);
                                lookLeftRightAmount = lookLeftRightAxis * horizontal;
                                break;
                            case RotationAxis.Y_positive:
                                lookLeftRightAxis = new Vector3(0f, 1f, 0f);
                                lookLeftRightAmount = lookLeftRightAxis * horizontal;
                                break;
                            case RotationAxis.Z_positive:
                                lookLeftRightAxis = new Vector3(0f, 0f, 1f);
                                lookLeftRightAmount = lookLeftRightAxis * horizontal;
                                break;
                            case RotationAxis.X_negative:
                                lookLeftRightAxis = new Vector3(1f, 0f, 0f);
                                lookLeftRightAmount = lookLeftRightAxis * -horizontal;
                                break;
                            case RotationAxis.Y_negative:
                                lookLeftRightAxis = new Vector3(0f, 1f, 0f);
                                lookLeftRightAmount = lookLeftRightAxis * -horizontal;
                                break;
                            case RotationAxis.Z_negative:
                                lookLeftRightAxis = new Vector3(0f, 0f, 1f);
                                lookLeftRightAmount = lookLeftRightAxis * -horizontal;
                                break;
                        }
                        lookAmounts += lookUpDownAmount; // Add up/down amount to combined amounts
                        lookAmounts += lookLeftRightAmount; // Add left/right amount to combined amounts

                        if (leftEyeBone) leftEyeBone.transform.localRotation = Quaternion.Euler(eyeStartRotL + lookAmounts);
                        if (rightEyeBone) rightEyeBone.transform.localRotation = Quaternion.Euler(eyeStartRotR + lookAmounts);
                    }
                }
            }

            if (re3DExpressions)
            {
                // Sync custom shape weights to all other SkinnedMeshRenderers
                for (int i = 0; i < re3DExpressions.skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                {
                    srcShapeWeight = re3DExpressions.skinnedMeshRenderer.GetBlendShapeWeight(i);

                    if (otherSMRs.Count > 0)
                    {
                        for (int othrSmr = 0; othrSmr < otherSMRs.Count; othrSmr++)
                        {
                            if (srcShapeWeight > 0)
                            {
                                syncIndex = -1;

                                if (re3DExpressions.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i).Contains("."))
                                    syncIndex = ShapeSearch(otherSMRs[othrSmr], re3DExpressions.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i).Split(new char[] { '.' })[1]);
                                else
                                    syncIndex = ShapeSearch(otherSMRs[othrSmr], re3DExpressions.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i));

                                if (syncIndex != -1)
                                    otherSMRs[othrSmr].SetBlendShapeWeight(syncIndex, srcShapeWeight);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Call this when initializing characters at runtime
        /// </summary>
        public void Initialize()
        {
            Reset();
        }

        /// <summary>
        /// Get the Salsa3D component
        /// </summary>
        public void GetSalsa3D()
        {
            if (!salsa3D) salsa3D = GetComponent<Salsa3D>();
        }

        /// <summary>
        /// Get the RandomEyes3D component
        /// </summary>
        public void GetRandomEyes3D()
        {
            RandomEyes3D[] randomEyes = GetComponents<RandomEyes3D>();
            if (randomEyes.Length == 1)
            {
                re3DEyes = randomEyes[0];
            }
            else if (randomEyes.Length > 1)
            {
                for (int i = 0; i < randomEyes.Length; i++)
                {
                    // Link the expression instance
                    if (randomEyes[i].useCustomShapesOnly)
                    {
                        re3DExpressions = randomEyes[i];
                    }
                    else // Link the eyes instance
                    {
                        re3DEyes = randomEyes[i];
                    }
                }
            }
        }

        /// <summary>
        /// Find the jaw bone
        /// </summary>
        public void GetJawBone()
        {
            if (!jawBone)
            {
                children = transform.gameObject.GetComponentsInChildren<Transform>();

                for (int i = 0; i < children.Length; i++)
                {
                    for (int l = 0; l < jawBoneNames.Length; l++)
                    {
                        if (children[i].name.ToLower().EndsWith(jawBoneNames[l]))
                        {
                            jawBone = children[i];
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find left and right eye bones
        /// </summary>
        public void GetEyeBones()
        {
            if (!leftEyeBone || !rightEyeBone)
            {
                children = transform.gameObject.GetComponentsInChildren<Transform>();

                for (int i = 0; i < children.Length; i++)
                {
                    for (int l = 0; l < leftEyeBoneNames.Length; l++)
                    {
                        if (children[i].name.ToLower().EndsWith(leftEyeBoneNames[l]))
                        {
                            leftEyeBone = children[i];
                            break;
                        }
                    }
                    for (int r = 0; r < rightEyeBoneNames.Length; r++)
                    {
                        if (children[i].name.ToLower().EndsWith(rightEyeBoneNames[r]))
                        {
                            rightEyeBone = children[i];
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find left and right eyelid bones
        /// </summary>
        public void GetEyeLidBones()
        {
            if (!leftEyelidBone || !rightEyelidBone)
            {
                children = transform.gameObject.GetComponentsInChildren<Transform>();

                for (int i = 0; i < children.Length; i++)
                {
                    for (int l = 0; l < leftEyelidBoneNames.Length; l++)
                    {
                        if (children[i].name.ToLower().EndsWith(leftEyelidBoneNames[l]))
                        {
                            leftEyelidBone = children[i];
                            break;
                        }
                    }
                    for (int r = 0; r < rightEyelidBoneNames.Length; r++)
                    {
                        if (children[i].name.ToLower().EndsWith(rightEyelidBoneNames[r]))
                        {
                            rightEyelidBone = children[i];
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find a child by name that ends with the search string. 
        /// This should compensates for BlendShape name prefixes variations.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public Transform ChildSearch(string endsWith)
        {
            Transform trans = null;

            children = transform.gameObject.GetComponentsInChildren<Transform>();

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name.ToLower().EndsWith(endsWith.ToLower()))
                {
                    trans = children[i];
                    break;
                }
            }

            return trans;
        }

        /// <summary>
        /// Find a shape by name, that ends with the search string.
        /// </summary>
        /// <param name="skndMshRndr"></param>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public int ShapeSearch(SkinnedMeshRenderer skndMshRndr, string endsWith)
        {
            int index = -1;
            if (skndMshRndr)
            {
                for (int i = 0; i < skndMshRndr.sharedMesh.blendShapeCount; i++)
                {
                    if (skndMshRndr.sharedMesh.GetBlendShapeName(i).ToLower().EndsWith(endsWith.ToLower()))
                    {
                        index = i;
                        break;
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// Populate a shapeNameList with shapes from this smr parameter
        /// </summary>
        public string[] GetShapeNames(SkinnedMeshRenderer smr)
        {
            int nameCount = 0;
            string[] shapeNameList = new string[0];

            if (smr != null)
            {
                shapeNameList = new string[smr.sharedMesh.blendShapeCount];
                for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
                {
                    shapeNameList[i] = smr.sharedMesh.GetBlendShapeName(i);
                    if (shapeNameList[i] != "") nameCount++;
                }
            }

            return shapeNameList;
        }

        /// <summary>
        /// Return the last populated SkinnedMeshRenderer in this CM_ShapeGroup, or null.
        /// </summary>
        /// <param name="shapeGroup"></param>
        /// <returns></returns>
        public SkinnedMeshRenderer GetLastSMR(List<CM_ShapeGroup> shapeGroup)
        {
            if (shapeGroup.Count == 1)
            {
                if (shapeGroup[shapeGroup.Count - 1].smr != null)
                    return shapeGroup[shapeGroup.Count - 1].smr;
                else
                    return null;
            }
            else if (shapeGroup.Count > 1)
            {
                if (shapeGroup[shapeGroup.Count - 2].smr != null)
                    return shapeGroup[shapeGroup.Count - 2].smr;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Scale a value from one range to another
        /// </summary>
        /// <returns>The range.</returns>
        /// <param name="val">Value.</param>
        /// <param name="inMin">In minimum.</param>
        /// <param name="inMax">In max.</param>
        /// <param name="outMin">Out minimum.</param>
        /// <param name="outMax">Out max.</param>
        public static float ScaleRange(float val, float inMin, float inMax, float outMin, float outMax)
        {
            val = Mathf.Clamp(val, inMin, inMax);
            float scaled = (((val - inMin) * (outMax - outMin)) / (inMax - inMin)) + outMin;
            return Mathf.Clamp(Mathf.Abs(scaled), outMin, outMax);
        }
    }

    /// <summary>
    /// SkinnedMeshRenderer, shape index, shape name, and percentage class for SalsaSync shape groups
    /// </summary>
    [System.Serializable]
    public class CM_ShapeGroup
    {
        public SkinnedMeshRenderer smr;
        public SkinnedMeshRenderer lastSmr;
        public string[] shapeNameList;
        public int shapeIndex;
        public string shapeName;
        public float shapePercent;
        public float shapeValue;

        public CM_ShapeGroup()
        {
            this.smr = null;
            this.shapeIndex = 0;
            this.shapeName = "";
            this.shapePercent = 100f;
            this.shapeValue = 0f;
        }

        public CM_ShapeGroup(SkinnedMeshRenderer smr, int shapeIndex, string shapeName, float percentage)
        {
            this.smr = smr;
            this.shapeIndex = shapeIndex;
            this.shapeName = shapeName;
            this.shapePercent = percentage;
            this.shapeValue = 0f;
        }
    }
}