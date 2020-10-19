using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Setup Pose transition for character animation
 * Needs to be called by the global control or in other places */

[System.Serializable]
public class AnimationData {
	public AnimationClip clip;
	public string clipName;
	public float speed = 1;
	public float transitionDuration = -1;
	public WrapMode wrapMode;
	public bool applyRootMotion;
	[HideInInspector] public int timesPlayed = 0;
	[HideInInspector] public float secondsPlayed = 0;
	[HideInInspector] public float length = 0;
	[HideInInspector] public int stateHash;
	[HideInInspector] public string stateName;
}

[RequireComponent (typeof (Animator))]
public class MecanimControl : MonoBehaviour {

	public AnimationData defaultAnimation = new AnimationData();
	public AnimationData[] animations = new AnimationData[0];
	public bool debugMode = false;
	public bool alwaysPlay = false;
	public bool overrideRootMotion = false;
	public float defaultTransitionDuration = 0.15f;
	public WrapMode defaultWrapMode = WrapMode.Loop;

	private Animator animator;

    private float startTime;
    private float currentTime = 0.0f;

	private int state1Hash;
	private int state2Hash;
	
	private RuntimeAnimatorController controller1;
	private RuntimeAnimatorController controller2;
	private RuntimeAnimatorController controller3;
	private RuntimeAnimatorController controller4;

	private AnimationData currentAnimationData;
	private bool currentMirror;

	public delegate void AnimEvent(AnimationData animationData);
	public static event AnimEvent OnAnimationBegin;
	public static event AnimEvent OnAnimationEnd;
	public static event AnimEvent OnAnimationLoop;
	
	// UNITY METHODS
	void Awake () {
		animator = gameObject.GetComponent<Animator>();

		switch (animator.name)
		{
			case "David_defined":
				controller1 = Resources.Load<RuntimeAnimatorController>("Controller/David_controller");
				break;
			case "Luna_defined":
				controller1 = Resources.Load<RuntimeAnimatorController>("Controller/Luna_controller");
				break;
			default:
				controller1 = Resources.Load<RuntimeAnimatorController>("Controller/controller1");
				break;
		}

		controller2 = Resources.Load<RuntimeAnimatorController>("Controller/controller2");
		controller3 = Resources.Load<RuntimeAnimatorController>("Controller/controller3");
		controller4 = Resources.Load<RuntimeAnimatorController>("Controller/controller4");

		foreach (AnimationData animData in animations) {
			if (animData.wrapMode == WrapMode.Default) animData.wrapMode = defaultWrapMode;
			animData.clip.wrapMode = animData.wrapMode;
		}
	}
	
	void Start(){
		BatchLoadClips();  // load all clips

        startTime = Time.time;  // start recording time length

		if (defaultAnimation.clip == null && animations.Length > 0){
			SetDefaultClip(animations[0].clip, "Default", animations[0].speed, animations[0].wrapMode, false);
		}
		
		if (defaultAnimation.clip != null){
			foreach(AnimationData animData in animations) {
				if (animData.clip == defaultAnimation.clip)
					defaultAnimation.clip = (AnimationClip) Instantiate(defaultAnimation.clip);
			}
			AnimatorOverrideController overrideController = new AnimatorOverrideController();
			overrideController.runtimeAnimatorController = controller1;
			
			currentAnimationData = defaultAnimation;
			currentAnimationData.stateName = "State2";
			currentAnimationData.length = currentAnimationData.clip.length;

			overrideController["State1"] = currentAnimationData.clip;
			overrideController["State2"] = currentAnimationData.clip;

			animator.runtimeAnimatorController = overrideController;
			animator.Play("State2", 0, 0);

			if (overrideRootMotion) animator.applyRootMotion = currentAnimationData.applyRootMotion;
			SetSpeed(currentAnimationData.speed);
		}
	}
	
	void FixedUpdate(){
		//WrapMode emulator
		if (currentAnimationData.clip == null) return;
		if (currentAnimationData.secondsPlayed == currentAnimationData.length){
			if (currentAnimationData.clip.wrapMode == WrapMode.Loop || currentAnimationData.clip.wrapMode == WrapMode.PingPong) {
				if (MecanimControl.OnAnimationLoop != null) MecanimControl.OnAnimationLoop(currentAnimationData);
				currentAnimationData.timesPlayed ++;
				
				if (currentAnimationData.clip.wrapMode == WrapMode.Loop) {
					SetCurrentClipPosition(0);
				}
				
				if (currentAnimationData.clip.wrapMode == WrapMode.PingPong) {
					SetSpeed(currentAnimationData.clipName, -currentAnimationData.speed);
					SetCurrentClipPosition(0);
				}
				
			}else if (currentAnimationData.timesPlayed == 0) {
				if (MecanimControl.OnAnimationEnd != null) MecanimControl.OnAnimationEnd(currentAnimationData);
				currentAnimationData.timesPlayed = 1;
				
				if (currentAnimationData.clip.wrapMode == WrapMode.Once && alwaysPlay) {
					Play(defaultAnimation, currentMirror);
				}else if (!alwaysPlay){
					animator.speed = 0;
				}
			}
		}else{
            //currentAnimationData.secondsPlayed += Time.deltaTime * animator.speed * Time.timeScale;
            currentTime = Time.time - startTime;
			currentAnimationData.secondsPlayed += (Time.fixedDeltaTime * animator.speed);
			if (currentAnimationData.secondsPlayed > currentAnimationData.length) 
				currentAnimationData.secondsPlayed = currentAnimationData.length;
		}
	}
	
	void OnGUI(){
		//Toggle debug mode to see the live data in action
		if (debugMode) {
			GUI.Box (new Rect (Screen.width - 340,40,340,420), "Animation Data");
			GUI.BeginGroup(new Rect (Screen.width - 330,60,400,420));{
				
				AnimatorClipInfo[] animationInfoArray = animator.GetCurrentAnimatorClipInfo(0);
				foreach (AnimatorClipInfo animationInfo in animationInfoArray){
					AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
					GUILayout.Label(animationInfo.clip.name);
					GUILayout.Label("-Wrap Mode: "+ animationInfo.clip.wrapMode);
					GUILayout.Label("-Is Playing: "+ IsPlaying(animationInfo.clip));
					GUILayout.Label("-Blend Weight: "+ animationInfo.weight);
					GUILayout.Label("-Normalized Time: "+ animatorStateInfo.normalizedTime);
					GUILayout.Label("-Length: "+ animationInfo.clip.length);
					GUILayout.Label("----");
				}
				
				GUILayout.Label("--Current Animation Data--");
				GUILayout.Label("-Current Clip Name: "+ currentAnimationData.clipName);
				GUILayout.Label("-Current Speed: "+ GetSpeed().ToString());
				GUILayout.Label("-Times Played: "+ currentAnimationData.timesPlayed);
				GUILayout.Label("-Seconds Played: "+ currentAnimationData.secondsPlayed);
				GUILayout.Label("-Emul. Normalized: "+ (currentAnimationData.secondsPlayed/currentAnimationData.length));
				GUILayout.Label("-Lengh: "+ currentAnimationData.length);

                GUILayout.Label("----");
                GUILayout.Label("Total Time: " + currentTime.ToString());
			}GUI.EndGroup();
		}
	}

	// auto-load all clips under character resource directory
	public void BatchLoadClips() {

		string filePath = "NULL";
		switch (animator.name)
		{
			case "David_defined":
				filePath = "David_Clips";
				break;
			case "Luna_defined":
				filePath = "Luna_Clips";
				break;
			case "Dana_defined":
				filePath = "Dana_Clips";
				break;
		}

		AnimationClip[] allClips = Resources.LoadAll<AnimationClip>(filePath);
		foreach (var clip in allClips)
			AddClip(clip, clip.name);
	}

	// MECANIM CONTROL METHODS
	public void RemoveClip(string name) {
		List<AnimationData> animationDataList = new List<AnimationData>(animations);
		animationDataList.Remove(GetAnimationData(name));
		animations = animationDataList.ToArray();
	}

	public void RemoveClip(AnimationClip clip) {
		List<AnimationData> animationDataList = new List<AnimationData>(animations);
		animationDataList.Remove(GetAnimationData(clip));
		animations = animationDataList.ToArray();
	}
	
	public void SetDefaultClip(AnimationClip clip, string name, float speed, WrapMode wrapMode, bool mirror) {
		defaultAnimation.clip = (AnimationClip) Instantiate(clip);
		defaultAnimation.clip.wrapMode = wrapMode;
		defaultAnimation.clipName = name;
		defaultAnimation.speed = speed;
		defaultAnimation.transitionDuration = -1;
		defaultAnimation.wrapMode = wrapMode;
	}
	
	public void AddClip(AnimationClip clip, string newName) {
		AddClip(clip, newName, 1, defaultWrapMode);
	}

	public void AddClip(AnimationClip clip, string newName, float speed, WrapMode wrapMode) {
		if (GetAnimationData(newName) != null) Debug.LogWarning("An animation with the name '"+ newName +"' already exists.");
		AnimationData animData = new AnimationData();
		animData.clip = (AnimationClip) Instantiate(clip);
		if (wrapMode == WrapMode.Default) wrapMode = defaultWrapMode;
		animData.clip.wrapMode = wrapMode;
		animData.clip.name = newName;
		animData.clipName = newName;
		animData.speed = speed;
		animData.length = clip.length;
		animData.wrapMode = wrapMode;

		List<AnimationData> animationDataList = new List<AnimationData>(animations);
		animationDataList.Add(animData);
		animations = animationDataList.ToArray();
	}

	public AnimationData GetAnimationData(string clipName){
		foreach(AnimationData animData in animations){
			if (animData.clipName == clipName){
				return animData;
			}
		}
		if (clipName == defaultAnimation.clipName) return defaultAnimation;
		return null;
	}

	public AnimationData GetAnimationData(AnimationClip clip){
		foreach(AnimationData animData in animations){
			if (animData.clip == clip){
				return animData;
			}
		}
		if (clip == defaultAnimation.clip) return defaultAnimation;
		return null;
	}
	
	public void CrossFade(string clipName, float blendingTime){
		CrossFade(clipName, blendingTime, 0, currentMirror);
	}

	public void CrossFade(string clipName, float blendingTime, float normalizedTime, bool mirror){
		_playAnimation(GetAnimationData(clipName), blendingTime, normalizedTime, mirror);
	}
	
	public void CrossFade(AnimationData animationData, float blendingTime, float normalizedTime, bool mirror){
		_playAnimation(animationData, blendingTime, normalizedTime, mirror);
	}

	public void Play(string clipName, float blendingTime, float normalizedTime, bool mirror){
		_playAnimation(GetAnimationData(clipName), blendingTime, normalizedTime, mirror);
	}
	
	public void Play(AnimationClip clip, float blendingTime, float normalizedTime, bool mirror){
		_playAnimation(GetAnimationData(clip), blendingTime, normalizedTime, mirror);
	}

	public void Play(string clipName, bool mirror){
		_playAnimation(GetAnimationData(clipName), 0, 0, mirror);
	}

	public void Play(AnimationData animationData, float blendingTime)
	{
		_playAnimation(animationData, blendingTime, 0, currentMirror);
	}

	public void Play(string clipName){
		_playAnimation(GetAnimationData(clipName), 0, 0, currentMirror);
	}
	
	public void Play(AnimationClip clip, bool mirror){
		_playAnimation(GetAnimationData(clip), 0, 0, mirror);
	}

	public void Play(AnimationClip clip){
		_playAnimation(GetAnimationData(clip), 0, 0, currentMirror);
	}

	public void Play(AnimationData animationData, bool mirror){
		_playAnimation(animationData, animationData.transitionDuration, 0, mirror);
	}

	public void Play(AnimationData animationData){
		_playAnimation(animationData, animationData.transitionDuration, 0, currentMirror);
	}
	
	public void Play(AnimationData animationData, float blendingTime, float normalizedTime, bool mirror){
		_playAnimation(animationData, blendingTime, normalizedTime, mirror);
	}

	public void Play(){
		animator.speed = Mathf.Abs(currentAnimationData.speed);
	}


	private void _playAnimation(AnimationData targetAnimationData, float blendingTime, float normalizedTime, bool mirror){
		//The overrite machine. Creates an overrideController, replace its core animations and restate it back in
		if (targetAnimationData == null || targetAnimationData.clip == null) return;

		AnimatorOverrideController overrideController = new AnimatorOverrideController();

		currentMirror = mirror;

		float newAnimatorSpeed = Mathf.Abs(targetAnimationData.speed);
		float currentNormalizedTime = GetCurrentClipPosition();

		if (mirror){
			if (targetAnimationData.speed > 0){
				overrideController.runtimeAnimatorController = controller2;
			}else{
				overrideController.runtimeAnimatorController = controller4;
			}
		}else{
			if (targetAnimationData.speed > 0){
				overrideController.runtimeAnimatorController = controller1;
			}else{
				overrideController.runtimeAnimatorController = controller3;
			}
		}
		
		overrideController["State1"] = currentAnimationData.clip;
		overrideController["State2"] = targetAnimationData.clip;
		 

		if (blendingTime == -1) blendingTime = currentAnimationData.transitionDuration;
		if (blendingTime == -1) blendingTime = defaultTransitionDuration;

		if (blendingTime <= 0){
			animator.runtimeAnimatorController = overrideController;
			animator.Play("State2", 0, normalizedTime);
		}else{
			animator.runtimeAnimatorController = overrideController;

			//animator.Play(state1Hash, 0, currentNormalizedTime);
			//currentAnimationData.secondsPlayed = currentNormalizedTime * currentAnimationData.length;

			//currentAnimationData.stateHash = state1Hash;
			currentAnimationData.stateName = "State1";
			SetCurrentClipPosition(currentNormalizedTime);

			animator.Update(0);
			animator.CrossFade("State2", 5*blendingTime / (newAnimatorSpeed*currentAnimationData.clip.length), 0, normalizedTime);
		}


		targetAnimationData.timesPlayed = 0;
		targetAnimationData.secondsPlayed = (normalizedTime * targetAnimationData.clip.length) / newAnimatorSpeed;
		targetAnimationData.length = targetAnimationData.clip.length;

		if (overrideRootMotion) animator.applyRootMotion = targetAnimationData.applyRootMotion;
		SetSpeed(targetAnimationData.speed);

		currentAnimationData = targetAnimationData;
		currentAnimationData.stateName = "State2";
		//currentAnimationData.stateHash = state2Hash;

		if (MecanimControl.OnAnimationBegin != null) MecanimControl.OnAnimationBegin(currentAnimationData);
	}
	
	public bool IsPlaying(string clipName){
		return IsPlaying(GetAnimationData(clipName));
	}
	
	public bool IsPlaying(string clipName, float weight){
		return IsPlaying(GetAnimationData(clipName), weight);
	}
	
	public bool IsPlaying(AnimationClip clip){
		return IsPlaying(GetAnimationData(clip));
	}
	
	public bool IsPlaying(AnimationClip clip, float weight){
		return IsPlaying(GetAnimationData(clip), weight);
	}
	
	public bool IsPlaying(AnimationData animData){
		return IsPlaying(animData, 0);
	}
	
	public bool IsPlaying(AnimationData animData, float weight){
		if (animData == null) return false;
		if (currentAnimationData == null) return false;
		if (currentAnimationData == animData && animData.wrapMode == WrapMode.Once && animData.timesPlayed > 0) return false;
		if (currentAnimationData == animData) return true;

		AnimatorClipInfo[] animationInfoArray = animator.GetCurrentAnimatorClipInfo(0);
		foreach (AnimatorClipInfo animationInfo in animationInfoArray){
			if (animData.clip == animationInfo.clip && animationInfo.weight >= weight) return true;
		}
		return false;
	}
	
	public string GetCurrentClipName(){
		return currentAnimationData.clipName;
	}
	
	public AnimationData GetCurrentAnimationData(){
		return currentAnimationData;
	}
	
	public int GetCurrentClipPlayCount(){
		return currentAnimationData.timesPlayed;
	}
	
	public float GetCurrentClipTime(){
		return currentAnimationData.secondsPlayed;
	}

	public float GetCurrentClipLength(){
		return currentAnimationData.length;
	}

	public void SetCurrentClipPosition(float normalizedTime){
		SetCurrentClipPosition(normalizedTime, false);
	}

	public void SetCurrentClipPosition(float normalizedTime, bool pause){
		//animator.Play(currentAnimationData.currentState.nameHash, 0, normalizedTime);
		animator.Play(currentAnimationData.stateName, 0, normalizedTime);
		currentAnimationData.secondsPlayed = normalizedTime * currentAnimationData.length;
		if (pause) Pause();
	}

	public float GetCurrentClipPosition(){
		/*if (currentAnimationData == null) return 0;
		if (currentAnimationData.clip == null) return 0;

		float trueNormalizedtime = -1;
		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
		AnimatorClipInfo[] animationInfoArray = animator.GetCurrentAnimatorClipInfo(0);
		foreach (AnimatorClipInfo animationInfo in animationInfoArray){
			if (animationInfo.clip != null && animationInfo.clip == currentAnimationData.clip){
				trueNormalizedtime = info.normalizedTime;
			}
		}

		if (trueNormalizedtime >= 0){
			return trueNormalizedtime;
		}else{
			return currentAnimationData.secondsPlayed/currentAnimationData.length;
		}*/
		return currentAnimationData.secondsPlayed/currentAnimationData.length;
	}
	
	public void Stop(){
		Play(defaultAnimation.clip, defaultTransitionDuration, 0, currentMirror);
	}
	
	public void Pause(){
		animator.speed = 0;
	}
	
	public void SetSpeed(AnimationClip clip, float speed){
		AnimationData animData = GetAnimationData(clip);
		animData.speed = speed;
		if (IsPlaying(clip)) SetSpeed(speed);
	}
	
	public void SetSpeed(string clipName, float speed){
		AnimationData animData = GetAnimationData(clipName);
		if (animData.speed == speed && animator.speed == Mathf.Abs(speed)) return;
		animData.speed = speed;
		if (IsPlaying(clipName)) SetSpeed(speed);
	}
	
	public void SetSpeed(float speed){
		animator.speed = Mathf.Abs(speed);
	}
	
	public void RestoreSpeed(){
		//SetCurrentClipPosition(GetCurrentClipPosition());
		SetSpeed(currentAnimationData.speed);
	}
	
	public void Rewind(){
		SetSpeed(-currentAnimationData.speed);
	}

	public void SetWrapMode(WrapMode wrapMode){
		defaultWrapMode = wrapMode;
	}
	
	public void SetWrapMode(AnimationData animationData, WrapMode wrapMode){
		animationData.wrapMode = wrapMode;
		animationData.clip.wrapMode = wrapMode;
	}

	public void SetWrapMode(AnimationClip clip, WrapMode wrapMode){
		AnimationData animData = GetAnimationData(clip);
		animData.wrapMode = wrapMode;
		animData.clip.wrapMode = wrapMode;
	}

	public void SetWrapMode(string clipName, WrapMode wrapMode){
		AnimationData animData = GetAnimationData(clipName);
		animData.wrapMode = wrapMode;
		animData.clip.wrapMode = wrapMode;
	}

	public float GetSpeed(AnimationClip clip){
		AnimationData animData = GetAnimationData(clip);
		return animData.speed;
	}

	public float GetSpeed(string clipName){
		AnimationData animData = GetAnimationData(clipName);
		return animData.speed;
	}

	public float GetSpeed(){
		return animator.speed;
	}
	
	public bool GetMirror(){
		return currentMirror;
	}

	public void SetMirror(bool toggle){
		SetMirror(toggle, 0, false);
	}
	
	public void SetMirror(bool toggle, float blendingTime){
		SetMirror(toggle, blendingTime, false);
	}

	public void SetMirror(bool toggle, float blendingTime, bool forceMirror){
		if (currentMirror == toggle && !forceMirror) return;
		
		if (blendingTime == 0) blendingTime = defaultTransitionDuration;
		_playAnimation(currentAnimationData, blendingTime, GetCurrentClipPosition(), toggle);
	}
}
