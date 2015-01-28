using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using WellFired;

namespace PixelCrushers.DialogueSystem.SequencerCommands {
	
	/// <summary>
	/// Implements sequencer command uSeq(USSequence), which loads a uSequencer sequence from 
	/// Resources and plays it.
	/// 
	/// <b>Syntax</b>: <c>uSeq(</c><em>USSequence</em>[, <c>once|cache</c>[, <em>index</em>, <em>object</em>...]]<c>)</c>
	/// 
	/// <b>Description</b>: Loads a uSequencer sequence from Resources and plays it. To prepare a 
	/// uSequencer sequence, create it and save a prefab inside a Resources folder. Then provide the
	/// path to that prefab to this command. 
	/// 
	/// By default, each uSequencer timeline container remembers the path to its affected object so it can
	/// find the object when you instantiate the prefab. However, if the object's actual path is different
	/// at runtime, you can set the affected object using the <em>index</em> and <em>object</em>
	/// parameters as described below.
	/// 
	/// <b>Parameters</b>:
	/// - <em>USSequence</em>: The path to a uSequencer sequence inside a Resources folder.
	/// - <c>once|cache</c>: (Optional) If <c>once</c>, the sequence is unloaded from memory when done 
	/// playing; if <c>cache</c>, it's kept in memory. Use <c>once</c> for sequences that only play once, 
	/// such as a unique one-off sequence. Use <c>cache</c> for generic sequences that may be used again, 
	/// such as a generic body gesture. Defaults to <c>cache</c>. Note: Since Resources.UnloadAsset()
	/// doesn't work for certain types such as uSequencer sequence prefabs, the unload calls
	/// Resources.UnloadUnusedAssets(). This process can slow down framerate, so you may choose to simply
	/// keep the sequence in memory and let garbage collection automatically unload it later.
	/// - <em>index</em>: (Optional) The index number of a USTimelineContainer in the sequence. This 
	/// parameter must be paired with an <em>object</em>.
	/// - <em>object</em>: (Optional): The name of an object in the scene, or <c>speaker</c> or
	/// <c>listener</c>. The affected object of the USTimelineContainer will be set to this object.
	/// - Note: You can provide any number of "<em>index</em>, <em>object</em>" parameters to this command.
	/// 
	/// <b>Examples</b>:
	/// - <c>uSeq(Cutscenes/USCutscene1)</c>
	/// 	- Plays the uSequencer sequence "Cutscenes/USCutscene1" located inside a Resources folder.
	/// - <c>uSeq(Cutscenes/Bow, cache, 1, speaker)</c>
	/// 	- Plays the uSequencer sequence "Cutscenes/Bow" using the speaker as the affected object 
	/// for the USTimelineContainer whose index is 1. Keeps the sequence in memory for re-use.
	/// - <c>uSeq(Cutscenes/GetToDaChoppa, once, 1, Arnold, 2, Helicopter)</c>
	/// 	- Plays the uSequencer sequence "Cutscenes/GetToDaChoppa" using the game object "Arnold" as 
	/// the affected object for the USTimelineContainer whose index is 1 and the game object "Helicopter" 
	/// as the affected object for the USTimelineContainer whose index is 2. When done playing, removes 
	/// the sequence from memory.
	/// </summary>
	public class SequencerCommanduSeq : SequencerCommand {
		
		/// <summary>
		/// The uSequencer sequence.
		/// </summary>
		private USSequencer sequence = null;
		
		/// <summary>
		/// If <c>true</c>, unload the sequence from memory when done playing.
		/// </summary>
		private bool unloadWhenDone = true;
	
		/// <summary>
		/// Starts the command by preparing and playing the sequence.
		/// </summary>
		public void Start() {
			LoadSequence();
			if (sequence != null) {
				sequence.transform.parent = this.transform;
				unloadWhenDone = (string.Compare("once", GetParameter(1, string.Empty)) == 0);
				BindAffectedObjects();
				sequence.Play();
			} else {
				Stop();
			}
		}
		
		/// <summary>
		/// Loads the sequence specified in parameter 0 and assigns it to the sequence variable.
		/// </summary>
		private void LoadSequence() {
			string sequencePath = GetParameter(0);
			if (string.IsNullOrEmpty(sequencePath)) {
				if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: uSeq({1}): No uSequencer sequence specified.", DialogueDebug.Prefix, GetParameters()));
			} else {
				Object resource = Resources.Load(sequencePath);
				if (resource == null) {
					if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: uSeq({1}): Couldn't load prefab {2}.", DialogueDebug.Prefix, GetParameters(), sequencePath));
				} else {
					GameObject go = Instantiate(resource) as GameObject;
					sequence = (go != null) ? go.GetComponentInChildren<USSequencer>() : null;
					if (sequence == null) {
						if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: uSeq({1}): No USSequencer component found on prefab {2}.", DialogueDebug.Prefix, GetParameters(), sequencePath));
					}
				}
			}
		}
		
		/// <summary>
		/// Binds the affected objects specified in any parameters 2+.
		/// </summary>
		private void BindAffectedObjects() {
			int parameterIndex = 2;
			while (parameterIndex < Parameters.Length) {
				int containerIndex = GetParameterAsInt(parameterIndex, 0);
				parameterIndex++;
				Transform affectedObject = GetSubject(parameterIndex, Sequencer.Speaker);
				parameterIndex++;
				if (affectedObject != null) {
					BindAffectedObjectToContainer(affectedObject, containerIndex);
				} else {
					if (DialogueDebug.LogWarnings) Debug.LogWarning(string.Format("{0}: Sequencer: uSeq({1}): Affected object {2} for container {3} not found.", DialogueDebug.Prefix, GetParameters(), GetParameter(parameterIndex - 1), containerIndex));
				}
			}
		}
		
		/// <summary>
		/// Binds the affected object to a USTimelineContainer.
		/// </summary>
		/// <param name='affectedObject'>
		/// The affected object.
		/// </param>
		/// <param name='containerIndex'>
		/// Index of a USTimelineContainer in the sequence.
		/// </param>
		private void BindAffectedObjectToContainer(Transform affectedObject, int containerIndex) {
			foreach (Transform child in sequence.transform) {
				USTimelineContainer container = child.GetComponentInChildren<USTimelineContainer>();
				if ((container != null) && (container.Index == containerIndex)) {
					container.AffectedObject = affectedObject;
					return;
				}
			}
		}
		
		/// <summary>
		/// Checks the status of the sequence; if done playing, calls Stop().
		/// </summary>
		public void Update() {
			if ((sequence == null) || (!sequence.IsPlaying)) Stop();
		}
		
		/// <summary>
		/// Handles cleanup at the end of the sequence.
		/// </summary>
		public void OnDestroy() {
			if (sequence != null) Destroy(sequence.gameObject);
			if (unloadWhenDone) Resources.UnloadUnusedAssets();
		}
		
	}

}
