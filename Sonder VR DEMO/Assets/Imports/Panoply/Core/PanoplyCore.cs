using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Opertoon.Panoply;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * The PanoplyCore class manages essential elements of the engine.
 * Copyright © Erik Loyer
 * erik@opertoon.com
 * Part of the Panoply engine
 */

namespace Opertoon.Panoply {

	public enum PassiveInputType {
		Mouse,
		Accelerometer,
		LeapMotion
	}

	public enum StateType {
		Key,
		Hold,
		End
	}

	public enum PanoplyStepDirection {
		BothDirections,
		ForwardOnly,
		BackwardOnly
	} 

	[ExecuteInEditMode()]
	public class PanoplyCore: MonoBehaviour {

	    
	    public static int targetStep = 0;
	    public static float interpolatedStep = 0.0f;
	    public static PassiveInputType passiveInputType;
	    public static float resolutionScale = 0.5f;
		public static PanoplyRenderer panoplyRenderer;
	    public static PanoplyScene scene;

	    static PanoplyEventManager eventManager;
	    
	    public void Start() {
	    
	    	switch ( Application.platform ) {
	    	
	    		case RuntimePlatform.IPhonePlayer:
	    		case RuntimePlatform.Android:
	    		passiveInputType = PassiveInputType.Accelerometer;
	    		break;
	    		
	    		default:
	    		passiveInputType = PassiveInputType.Mouse;
	    		break;
	    	
	    	}

			GameObject go = GameObject.Find( "Panoply" );
			panoplyRenderer = go.GetComponent<PanoplyRenderer>();
			eventManager = go.GetComponent<PanoplyEventManager>();
			scene = go.GetComponent<PanoplyScene>();

	    	float scaleH = Screen.width / panoplyRenderer.referenceScreenSize.x;
	    	float scaleV = Screen.height / panoplyRenderer.referenceScreenSize.y;
	    	resolutionScale = Mathf.Lerp( scaleH, scaleV, panoplyRenderer.matchWidthHeight ) * 0.5f;
	    	
	    	string direction = PlayerPrefs.GetString( "SceneChangeDirection", "Forward" );
	    
	    	if ( direction == "Backward" ) {
	    		interpolatedStep = ( float )( scene.stepCount - 1 );
	            targetStep = ( int )interpolatedStep;
	    	}
	    	
	    }

		public static void IncrementStep() {
			IncrementStep( false );
		}

	    public static void IncrementStep( bool ignoreStepCount ) {
	    	if (( targetStep < ( scene.stepCount - 1 ) ) || ignoreStepCount ) {
	    		targetStep++;
				eventManager.HandleTargetStepChanged( targetStep - 1, targetStep );
			}
	    }

		public static void DecrementStep() {
			DecrementStep( false );
		}

		public static void DecrementStep( bool ignoreStepCount ) {
	    	if (( targetStep > 0 ) || ignoreStepCount ) {
	    		targetStep--;
				eventManager.HandleTargetStepChanged( targetStep + 1, targetStep );
			}
	    }
	    
	    public static void GoToFirstStep() {
			int lastStep = targetStep;
			targetStep = 0;
			eventManager.HandleTargetStepChanged( lastStep, targetStep );
		}
	    
	    public static void GoToLastStep() {
			int lastStep = targetStep;
			targetStep = scene.stepCount - 1;
			eventManager.HandleTargetStepChanged( lastStep, targetStep );
		}
	    
	    public static void SetTargetStep( int v ) {
			int lastStep = targetStep;
	    	targetStep = Math.Min( scene.stepCount - 1, Math.Max( 0, v ) );
			eventManager.HandleTargetStepChanged( lastStep, targetStep );
		}
	    
	    public static void SetInterpolatedStep( float v ) {
	    	interpolatedStep = Math.Min( ( float )( scene.stepCount - 1 ), Math.Max( 0.0f, v ) );
	    }

		#if UNITY_EDITOR
		public static void InsertStateGlobally() {
			Panel[] panels = FindObjectsOfType(typeof(Panel)) as Panel[];
			foreach (Panel panel in panels) {
				KeyframeManager.InsertStateForTimelineObject(panel);
			}
			Caption[] captions = FindObjectsOfType(typeof(Caption)) as Caption[];
			foreach (Caption caption in captions) {
				KeyframeManager.InsertStateForTimelineObject(caption);
			}
			Artwork[] artworks = FindObjectsOfType(typeof(Artwork)) as Artwork[];
			foreach (Artwork artwork in artworks) {
				KeyframeManager.InsertStateForTimelineObject(artwork);
			}
			AudioTrack[] audioTracks = FindObjectsOfType(typeof(AudioTrack)) as AudioTrack[];
			foreach (AudioTrack audioTrack in audioTracks) {
				KeyframeManager.InsertStateForTimelineObject(audioTrack);
			}
			GameObject.Find( "Panoply" ).GetComponent<PanoplySequencer>().InsertStateForAllInstruments();
		}

		public static void DeleteAllCurrentStatesGlobally() {
			Panel[] panels = FindObjectsOfType(typeof(Panel)) as Panel[];
			foreach (Panel panel in panels) {
				KeyframeManager.DeleteCurrentStateForTimelineObject(panel);
			}
			Caption[] captions = FindObjectsOfType(typeof(Caption)) as Caption[];
			foreach (Caption caption in captions) {
				KeyframeManager.DeleteCurrentStateForTimelineObject(caption);
			}
			Artwork[] artworks = FindObjectsOfType(typeof(Artwork)) as Artwork[];
			foreach (Artwork artwork in artworks) {
				KeyframeManager.DeleteCurrentStateForTimelineObject(artwork);
			}
			AudioTrack[] audioTracks = FindObjectsOfType(typeof(AudioTrack)) as AudioTrack[];
			foreach (AudioTrack audioTrack in audioTracks) {
				KeyframeManager.DeleteCurrentStateForTimelineObject(audioTrack);
			}
			GameObject.Find( "Panoply" ).GetComponent<PanoplySequencer>().DeleteCurrentStateOfAllInstruments();
		}
		#endif

		public static int[] RemoveItemFromIntArray(int[] intArray, int index) {
			if (index < intArray.Length) {
				ArrayList arrayList = new ArrayList(intArray);
				arrayList.RemoveAt(index);
				intArray = new int[arrayList.Count];
				arrayList.CopyTo(intArray);
			}
			return intArray;
		}
		
		public static int[] AddItemToIntArray(int[] intArray, int index, int item) {
			if (index <= intArray.Length) {
				ArrayList arrayList = new ArrayList(intArray);
				arrayList.Insert(index, item);
				intArray = new int[arrayList.Count];
				arrayList.CopyTo(intArray);
			}
			return intArray;
		}

	    public void Update() {

			if ( scene == null ) {
				scene = GameObject.Find( "Panoply" ).GetComponent<PanoplyScene>();
			}

	    	if ( panoplyRenderer == null ) {
				panoplyRenderer = GameObject.Find( "Panoply" ).GetComponent<PanoplyRenderer>();
	    	}
	    	
			float scaleH = panoplyRenderer.screenRect.width / panoplyRenderer.referenceScreenSize.x;
			float scaleV = panoplyRenderer.screenRect.height / panoplyRenderer.referenceScreenSize.y;
			resolutionScale = Mathf.Lerp( scaleH, scaleV, panoplyRenderer.matchWidthHeight ) * 0.5f;
	    
	    }
	}
}