using UnityEngine;
using System;
using System.Collections.Generic;
using Opertoon.Panoply;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * Panel
 * Manages all elements that go into creating a panel.
 * Copyright © Erik Loyer
 * erik@opertoon.com
 * Part of the Panoply engine
 */

namespace Opertoon.Panoply {
	public enum PanelComponent {
		PanelFrame,
		PanelCamera,
		PanelPassiveMotion,
		PanelPassiveMotionH,
		PanelPassiveMotionV
	}

	[ExecuteInEditMode()]
	public class Panel: MonoBehaviour {
	     
	    public bool isEditingFrame = true;								// Are we currently editing the frame component?
	    public bool isEditingCamera = false;							// Are we currently editing the camera component?
	    public bool isEditingPassiveMotion = false;						// Are we currently editing the passive motion component?
	    					
	    public int rows = 2;
	    public int columns = 3;
	    
	    public int top;
	    public int left;
	    public int width;
	    public int height;
	    
	    public FrameState frameStateA;
	    public FrameState frameStateB;
	    
	    public CameraState cameraStateA;
	    public CameraState cameraStateB;
	    
	    public PassiveMotionState passiveMotionHStateA;
	    public PassiveMotionState passiveMotionHStateB;
	    public PassiveMotionState passiveMotionVStateA;
	    public PassiveMotionState passiveMotionVStateB;
	    
	    /* -- StateManager properties start -- */
	    
	    public int startIndex = 0;											// The index at which this item's script starts
	    public int scriptIndex = 0;											// Index of our current position in the frame state script
	    
	    public float crossfade;												// Range from -1 to 1; 0 is current frame state, -1 is previous, 1 is next
	    public float progress;
	    
	    public List<FrameState> frameStates = new List<FrameState>();		// Library of possible states
	    public List<string> frameStateScript = new List<string>();			// Array of state ids that describe how it changes over time
	    public int[] frameScriptStateIndices;								// The index of each step's state		
	    
	    public List<CameraState> cameraStates = new List<CameraState>();	// Library of possible states
	    public List<string> cameraStateScript = new List<string>();			// Array of state ids that describe how it changes over time
	    public int[] cameraScriptStateIndices;								// The index of each step's state		
	    
	    public List<PassiveMotionState> passiveMotionStatesH = new List<PassiveMotionState>();	// Library of possible states
	    public List<PassiveMotionState> passiveMotionStatesV = new List<PassiveMotionState>();	// Library of possible states
	    public List<string> passiveMotionStateScript = new List<string>();						// Array of state ids that describe how it changes over time
	    public int[] passiveMotionScriptStateIndices;											// The index of each step's state		
	    
	    /* -- StateManager properties end -- */
	    
	    public bool interceptInteraction = false;							// Does this frame prevent global controller interactions?
	    public Rect frameRect;
	    
	    public Vector3 homePosition;
	    public bool lookAtEnabled = false;
	    
	    public int frameStateCounter = 0;
	    public int cameraStateCounter = 0;
	    public int passiveMotionHStateCounter = 0;
	    public int passiveMotionVStateCounter = 0;
	    
	    public FrameState currentFS;
	    public FrameState previousFS;
	    public FrameState nextFS;
	    public bool interpolatePreviousFS = true;
	    public bool interpolateNextFS = true;
	    
	    Vector3 lastCameraPosition;
	    Quaternion lastCameraRotation;
	    
	    private PanoplyController controller;
		private Rect screenRect = new Rect( 0, 0, Screen.width, Screen.height );
	    
	    [HideInInspector]
	    public Vector3 passiveMotionTranslationH;
	    
	    [HideInInspector]
	    public Vector3 passiveMotionRotationH;
	    
	    [HideInInspector]
	    public Vector3 passiveMotionTranslationV;
	    
	    [HideInInspector]
	    public Vector3 passiveMotionRotationV;
	    
	    public void Awake() {
	    
	    	previousFS = null;
	    	currentFS = null;
	    	nextFS = null;
	    
	    }
	    
	    public void Start() {
	    		
	    	/* -- CAMERA -- */
	    	
	    	if ( cameraStates.Count > 0 ) {
	    	
	    		CameraState state = cameraStates[ 0 ];
	    		GetComponent<Camera>().fieldOfView = state.fieldOfViewVal;
	    		GetComponent<Camera>().transform.position = homePosition + state.position;
	    		
	    		Vector3 cameraRotation = state.rotation;
	    		switch ( state.orientationType ) {
	    		
	    			case CameraOrientationType.Standard:
	    			var tmp_cs1 = GetComponent<Camera>().transform.rotation;
	                tmp_cs1.eulerAngles = cameraRotation;
	                GetComponent<Camera>().transform.rotation = tmp_cs1;
	    			break;
	    			
	    			case CameraOrientationType.LookAt:
	    			GetComponent<Camera>().transform.LookAt( state.lookAt );
	    			GetComponent<Camera>().transform.Rotate( cameraRotation );
	    			break;
	    			
	    		}
	    	}
	    	
	    	/* -- PASSIVE MOTION -- */
	    	
	    	controller = GameObject.Find( "Panoply" ).GetComponent<PanoplyController>();
			
			if (PanoplyCore.panoplyRenderer != null) {
				PanoplyCore.panoplyRenderer.UpdateInventory();
			}

			if (homePosition == Vector3.zero) {
				float maxHomePositionX = 0;
				Panel[] panels = FindObjectsOfType(typeof(Panel)) as Panel[];
				foreach (Panel panel in panels) {
					maxHomePositionX = Mathf.Max(maxHomePositionX, panel.homePosition.x);
				}
				GameObject go = GameObject.Find( "Panoply" );
				PanoplyScene scene = go.GetComponent<PanoplyScene>();
				homePosition = new Vector3(maxHomePositionX + scene.homePositionXSpacing, 0, 0);
			}
		}
	    
	    /* -- StateManager methods start -- */
	    
	    public object GetStateForId( PanelComponent pc, string id ) {
	    
	    	int i = 0;
	    	int n = 0;
	    	PassiveMotionState passiveMotionState = null;
	    	
	    	switch ( pc ) {
	    	
	    		case PanelComponent.PanelFrame:
	    		FrameState frameState = null;
	    		n = frameStates.Count;
	    		id = GetCleanIdFromId( id );
	    		for( i = 0; i < n; i++ ) {
	    			frameState = frameStates[ i ];
	    			if ( frameState.id == id ) {
	    				return frameState;
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelCamera:
	    		CameraState cameraState = null;
	    		n = cameraStates.Count;
	    		id = GetCleanIdFromId( id );
	    		for( i = 0; i < n; i++ ) {
	    			cameraState = cameraStates[ i ];
	    			if ( cameraState.id == id ) {
	    				return cameraState;
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelPassiveMotion:
	    		case PanelComponent.PanelPassiveMotionH:
	    		n = passiveMotionStatesH.Count;
	    		id = GetCleanIdFromId( id );
	    		for( i = 0; i < n; i++ ) {
	    			passiveMotionState = passiveMotionStatesH[ i ];
	    			if ( passiveMotionState.id == id ) {
	    				return passiveMotionState;
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelPassiveMotionV:
	    		n = passiveMotionStatesV.Count;
	    		id = GetCleanIdFromId( id );
	    		for( i = 0; i < n; i++ ) {
	    			passiveMotionState = passiveMotionStatesV[ i ];
	    			if ( passiveMotionState.id == id ) {
	    				return passiveMotionState;
	    			}
	    		}
	    		break;
	    	
	    	}
	    	
	    	return null;
	    }
	    
	    public int GetStateIndexForId( PanelComponent pc, string id ) {
	    
	    	int i = 0;
	    	int n = 0;
	    	PassiveMotionState passiveMotionState = null;
	    	
	    	switch ( pc ) {
	    	
	    		case PanelComponent.PanelFrame:
	    		FrameState frameState = null;
	    		n = frameStates.Count;
	    		id = GetCleanIdFromId( id );
	    		for( i = 0; i < n; i++ ) {
	    			frameState = frameStates[ i ];
	    			if ( frameState.id == id ) {
	    				return i;
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelCamera:
	    		CameraState cameraState = null;
	    		n = cameraStates.Count;
	    		id = GetCleanIdFromId( id );
	    		for( i = 0; i < n; i++ ) {
	    			cameraState = cameraStates[ i ];
	    			if ( cameraState.id == id ) {
	    				return i;
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelPassiveMotion:
	    		case PanelComponent.PanelPassiveMotionH:
	    		case PanelComponent.PanelPassiveMotionV:
	    		n = passiveMotionStatesH.Count;
	    		id = GetCleanIdFromId( id );
	    		for( i = 0; i < n; i++ ) {
	    			passiveMotionState = passiveMotionStatesH[ i ];
	    			if ( passiveMotionState.id == id ) {
	    				return i;
	    			}
	    		}
	    		break;
	    	
	    	}
	    	
	    	return -1;
	    }
	    
	    public string GetCleanIdFromId( string id ) {
	    
	    	string cleanId = null;
	    	
	    	string[] temp = id.Split( new string[] {") "}, StringSplitOptions.None );
	    	if ( temp.Length > 1 ) {
	    		cleanId = String.Join( ") ", temp, 1, temp.Length - 1 );
	    	} else {
	    		cleanId = id;
	    	}
	    
	    	return cleanId;
	    }
	    
	    public void PushState( PanelComponent pc, string id, bool deleteFuture ) {
	    
	    	switch ( pc ) {
	    	
	    		case PanelComponent.PanelFrame:
	    		if ( deleteFuture && ( scriptIndex < ( frameStateScript.Count - 1 ) ) ) {
	    			frameStateScript.RemoveRange( scriptIndex + 1, frameStateScript.Count - scriptIndex );
	    		}
	    		FrameState frameState = GetStateForId( pc, id ) as FrameState;
	    		if ( frameState != null ) {
	    			frameStateScript.Add( frameState.id );
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelCamera:
	    		if ( deleteFuture && ( scriptIndex < ( frameStateScript.Count - 1 ) ) ) {
	    			cameraStateScript.RemoveRange( scriptIndex + 1, cameraStateScript.Count - scriptIndex );
	    		}
	    		CameraState cameraState = GetStateForId( pc, id ) as CameraState;
	    		if ( cameraState != null ) {
	    			cameraStateScript.Add( cameraState.id );
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelPassiveMotion:
	    		case PanelComponent.PanelPassiveMotionH:
	    		case PanelComponent.PanelPassiveMotionV:
	    		if ( deleteFuture && ( scriptIndex < ( passiveMotionStateScript.Count - 1 ) ) ) {
	    			passiveMotionStateScript.RemoveRange( scriptIndex + 1, passiveMotionStateScript.Count - scriptIndex );
	    		}
	    		PassiveMotionState passiveMotionState = GetStateForId( pc, id ) as PassiveMotionState;
	    		if ( passiveMotionState != null ) {
	    			passiveMotionStateScript.Add( passiveMotionState.id );
	    		}
	    		break;
	    	
	    	}
	    	
	    }
	    
	    public void SetCurrentState( PanelComponent pc, string id ) {
	    
	    	switch ( pc ) {
	    	
	    		case PanelComponent.PanelFrame:
	    		FrameState frameState = GetStateForId( pc, id ) as FrameState;
	    		if ( frameState != null ) {
	    			if ( frameStateScript.Count == 0 ) {
	    				PushState( pc, frameState.id, false );
	    			} else {
	    				frameStateScript[ scriptIndex ] = frameState.id;
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelCamera:
	    		CameraState cameraState = GetStateForId( pc, id ) as CameraState;
	    		if ( cameraState != null ) {
	    			if ( cameraStateScript.Count == 0 ) {
	    				PushState( pc, cameraState.id, false );
	    			} else {
	    				cameraStateScript[ scriptIndex ] = cameraState.id;
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelPassiveMotion:
	    		case PanelComponent.PanelPassiveMotionH:
	    		case PanelComponent.PanelPassiveMotionV:
	    		PassiveMotionState passiveMotionState = GetStateForId( pc, id ) as PassiveMotionState;
	    		if ( passiveMotionState != null ) {
	    			if ( passiveMotionStateScript.Count == 0 ) {
	    				PushState( pc, passiveMotionState.id, false );
	    			} else {
	    				passiveMotionStateScript[ scriptIndex ] = passiveMotionState.id;
	    			}
	    		}
	    		break;
	    	
	    	}
	    
	    }
	    
	    public void DecrementState() {
	    
	    	if ( scriptIndex > 1 ) {
	    		scriptIndex--;
	    	}
	    
	    }
	    
	    public void IncrementState( PanelComponent pc ) {
	    
	    	switch ( pc ) {
	    	
	    		case PanelComponent.PanelFrame:
	    		if ( scriptIndex < ( frameStateScript.Count - 1 ) ) {
	    			scriptIndex++;
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelCamera:
	    		if ( scriptIndex < ( cameraStateScript.Count - 1 ) ) {
	    			scriptIndex++;
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelPassiveMotion:
	    		case PanelComponent.PanelPassiveMotionH:
	    		case PanelComponent.PanelPassiveMotionV:
	    		if ( scriptIndex < ( passiveMotionStateScript.Count - 1 ) ) {
	    			scriptIndex++;
	    		}
	    		break;
	    	
	    	}
	    	
	    	
	    }
	    
	    public object GetKeyStateAtScriptIndex( PanelComponent pc, int index ) {
	    
	    	string id = null;
	    	
	    	switch ( pc ) {
	    	
	    		case PanelComponent.PanelFrame:
	    		if (( index >= 0 ) && ( index < frameStateScript.Count )) {
	    			id = frameStateScript[ index ];
	    			if ( id != FrameState.HoldCommand ) {
	    				return GetStateForId( pc, id );
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelCamera:
	    		if (( index >= 0 ) && ( index < cameraStateScript.Count )) {
	    			id = cameraStateScript[ index ];
	    			if ( id != CameraState.HoldCommand ) {
	    				return GetStateForId( pc, id );
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelPassiveMotion:
	    		case PanelComponent.PanelPassiveMotionH:
	    		case PanelComponent.PanelPassiveMotionV:
	    		if (( index >= 0 ) && ( index < passiveMotionStateScript.Count )) {
	    			id = passiveMotionStateScript[ index ];
	    			if ( id != PassiveMotionState.HoldCommand ) {
	    				return GetStateForId( pc, id );
	    			}
	    		}
	    		break;
	    	
	    	}
	    	
	    	return null;
	    }
	    
	    public object GetStateAtScriptIndex( PanelComponent pc, int index ) {
	    
	    	int i = 0;
	    	string id = null;
	    	
	    	switch ( pc ) {
	    	
	    		case PanelComponent.PanelFrame:
	    		if (( index >= 0 ) && ( index < frameStateScript.Count )) {
	    			id = frameStateScript[ index ];
	    			
	    			if ( id == FrameState.HoldCommand ) {
	    			
	    				FrameState frameState = null;
	    				
	    				for( i = ( index - 1 ); i >= 0; i-- ) {
	    					frameState = GetStateForId( pc, frameStateScript[ i ] ) as FrameState;
	    					if ( frameState != null ) {
	    						return frameState;
	    					}
	    				}
	    				
	    			} else {
	    				return GetStateForId( pc, id );
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelCamera:
	    		if (( index >= 0 ) && ( index < cameraStateScript.Count )) {
	    			id = cameraStateScript[ index ];
	    			
	    			if ( id == CameraState.HoldCommand ) {
	    			
	    				CameraState cameraState = null;
	    				
	    				for( i = ( index - 1 ); i >= 0; i-- ) {
	    					cameraState = GetStateForId( pc, cameraStateScript[ i ] ) as CameraState;
	    					if ( cameraState != null ) {
	    						return cameraState;
	    					}
	    				}
	    				
	    			} else {
	    				return GetStateForId( pc, cameraStateScript[ index ] );
	    			}
	    		}
	    		break;
	    	
	    		case PanelComponent.PanelPassiveMotion:
	    		case PanelComponent.PanelPassiveMotionH:
	    		case PanelComponent.PanelPassiveMotionV:
	    		if (( index >= 0 ) && ( index < passiveMotionStateScript.Count )) {
	    			id = passiveMotionStateScript[ index ];
	    			
	    			if ( id == PassiveMotionState.HoldCommand ) {
	    			
	    				PassiveMotionState passiveMotionState = null;
	    				
	    				for( i = ( index - 1 ); i >= 0; i-- ) {
	    					passiveMotionState = GetStateForId( pc, passiveMotionStateScript[ i ] ) as PassiveMotionState;
	    					if ( passiveMotionState != null ) {
	    						return passiveMotionState;
	    					}
	    				}
	    				
	    			} else {
	    				return GetStateForId( pc, passiveMotionStateScript[ index ] );
	    			}
	    		}
	    		break;
	    	
	    	}
	    
	    	return null;
	    }
	    
	    public void SetPreviousFrameState( FrameState fs ) {
	    	SetPreviousFrameState( fs, true );
	    }
	    
	    public void SetPreviousFrameState( FrameState fs, bool interpolate ) {
	    	previousFS = fs;
	    	interpolatePreviousFS = interpolate;
	    }
	    
	    public void SetCurrentFrameState( FrameState fs ) {
	    	currentFS = fs;
	    }
	    
	    public void SetNextFrameState( FrameState fs ) {
	    	SetNextFrameState( fs, true );
	    }
	    
	    public void SetNextFrameState( FrameState fs, bool interpolate ) {
	    	nextFS = fs;
	    	interpolateNextFS = interpolate;
	    }
	    
	    public void PushFrameState( FrameState fs ) {
	    	previousFS = currentFS;
	    	currentFS = nextFS;
	    	nextFS = fs;
	    }
	    
	    public FrameState PreviousFrameState() {

			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex - 1, 0, PanoplyCore.scene.stepCount - 1 );
	    
		    	/*if (( previousFS != null ) && Application.isPlaying ) {
		    		return previousFS;
		    	} else*/ if ( ( frameStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( frameStateScript.Count - 1 ) ) ) {
		    		return null;
				} else {
					return GetStateAtScriptIndex( PanelComponent.PanelFrame, scriptIndexProxy ) as FrameState;
		    	}
			} else {
				return null;
			}

	    }
	    
	    public FrameState CurrentFrameState() {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex, 0, PanoplyCore.scene.stepCount - 1 );

		    	/*if (( currentFS != null ) && Application.isPlaying ) {
		    		return currentFS;
		    	} else*/ if ( ( frameStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( frameStateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( PanelComponent.PanelFrame, scriptIndexProxy ) as FrameState;
		    	}
			} else {
				return null;
			}

	    }
	    
	    public FrameState NextFrameState() {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex + 1, 0, PanoplyCore.scene.stepCount - 1 );

		    	/*if (( nextFS != null ) && Application.isPlaying ) {
		    		return nextFS;
		    	} else*/ if ( ( frameStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( frameStateScript.Count - 1 ) ) ) {
		    		return null;
				} else {
					return GetStateAtScriptIndex( PanelComponent.PanelFrame, scriptIndexProxy ) as FrameState;
		    	}
			} else {
				return null;
			}

	    }
	    
	    public CameraState PreviousCameraState() {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex - 1, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( cameraStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( cameraStateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( PanelComponent.PanelCamera, scriptIndexProxy ) as CameraState;
		    	}
			} else {
				return null;
			}

	    }
	    
	    public CameraState CurrentCameraState() {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( cameraStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( cameraStateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( PanelComponent.PanelCamera, scriptIndexProxy ) as CameraState;
		    	}
			} else {
				return null;
			}

	    }
	    
	    public CameraState NextCameraState() {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex + 1, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( cameraStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( cameraStateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( PanelComponent.PanelCamera, scriptIndexProxy ) as CameraState;
		    	}
			} else {
				return null;
			}

	    }
	    
	    public PassiveMotionState PreviousPassiveMotionState( PanelComponent pc ) {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex - 1, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( passiveMotionStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( passiveMotionStateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( pc, scriptIndexProxy ) as PassiveMotionState;
		    	}
			} else {
				return null;
			}

	    }
	    
	    public PassiveMotionState CurrentPassiveMotionState( PanelComponent pc ) {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( passiveMotionStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( passiveMotionStateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( pc, scriptIndexProxy ) as PassiveMotionState;
		    	}
			} else {
				return null;
			}

	    }
	    
	    public PassiveMotionState NextPassiveMotionState( PanelComponent pc ) {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex + 1, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( passiveMotionStateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( passiveMotionStateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( pc, scriptIndexProxy ) as PassiveMotionState;
		    	}
			} else {
				return null;
			}

	    }

	    void CalculatePassiveMotion( PanelComponent pc ) {
	    
	    	if ( controller == null ) {
	    		controller = ( PanoplyController )FindObjectOfType( typeof( PanoplyController ) );
	    	}
	    
	    	PassiveMotionState stateA = null;
	    	PassiveMotionState stateB = null;
	    
	    	if ( pc == PanelComponent.PanelPassiveMotionH ) {
	    		stateA = passiveMotionHStateA;
	    		stateB = passiveMotionHStateB;
	    	} else if ( pc == PanelComponent.PanelPassiveMotionV ) {
	    		stateA = passiveMotionVStateA;
	    		stateB = passiveMotionVStateB;
	    	}
	    
	    	Vector3 translation = Vector3.zero;
	    	Vector3 translationA = Vector3.zero;
	    	Vector3 translationB = Vector3.zero;
	    	Vector3 rotation = Vector3.zero;
	    	Vector3 rotationA = Vector3.zero;
	    	Vector3 rotationB = Vector3.zero;
	    	float passiveMotionOutput = 0.0f;

	    	if ( ( stateA != null ) && ( stateB != null ) ) {
	    		
	    		// INTERPOLATE INPUT VARIANCES
	    		
	    		translationA = Vector3.zero;
	    		translationB = Vector3.zero;
	    		rotationA = Vector3.zero;
	    		rotationB = Vector3.zero;
	    		
	    		// loop through each variance and apply its affects to the specified properties
	    		switch ( stateA.input ) {
	    		
	    			case PassiveMotionControllerInput.HorizontalTilt:
	    			passiveMotionOutput = stateA.GetOutput( controller.horizontalTilt );
	    			break;
	    			
	    			case PassiveMotionControllerInput.VerticalTilt:
	    			passiveMotionOutput = stateA.GetOutput( controller.verticalTilt );
	    			break;
	    		
	    		}
	    		switch ( stateA.output ) {
	    		
	    			case PassiveMotionOutputType.Position:
	    			if (stateA.outputProperty == PassiveMotionOutputProperty.X) {
	    				translationA.x = passiveMotionOutput;
	    			} else if (stateA.outputProperty == PassiveMotionOutputProperty.Y) {
	    				translationA.y = passiveMotionOutput;
	    			} else if (stateA.outputProperty == PassiveMotionOutputProperty.Z) {
	    				translationA.z = passiveMotionOutput;
	    			}
	    			break;
	    			
	    			case PassiveMotionOutputType.Rotation:
	    			if (stateA.outputProperty == PassiveMotionOutputProperty.X) {
	    				rotationA.x = passiveMotionOutput;
	    			} else if (stateA.outputProperty == PassiveMotionOutputProperty.Y) {
	    				rotationA.y = passiveMotionOutput;
	    			} else if (stateA.outputProperty == PassiveMotionOutputProperty.Z) {
	    				rotationA.z = passiveMotionOutput;
	    			}
	    			break;
	    			
	    		}
	    		
	    		// loop through each variance and apply its affects to the specified properties
	    		switch ( stateB.input ) {
	    		
	    			case PassiveMotionControllerInput.HorizontalTilt:
	    			passiveMotionOutput = stateB.GetOutput( controller.horizontalTilt );
	    			break;
	    			
	    			case PassiveMotionControllerInput.VerticalTilt:
	    			passiveMotionOutput = stateB.GetOutput( controller.verticalTilt );
	    			break;
	    		
	    		}
	    		switch ( stateB.output ) {
	    		
	    			case PassiveMotionOutputType.Position:
	    			if (stateB.outputProperty == PassiveMotionOutputProperty.X) {
	    				translationB.x = passiveMotionOutput;
	    			} else if (stateB.outputProperty == PassiveMotionOutputProperty.Y) {
	    				translationB.y = passiveMotionOutput;
	    			} else if (stateB.outputProperty == PassiveMotionOutputProperty.Z) {
	    				translationB.z = passiveMotionOutput;
	    			}
	    			break;
	    			
	    			case PassiveMotionOutputType.Rotation:
	    			if (stateB.outputProperty == PassiveMotionOutputProperty.X) {
	    				rotationB.x = passiveMotionOutput;
	    			} else if (stateB.outputProperty == PassiveMotionOutputProperty.Y) {
	    				rotationB.y = passiveMotionOutput;
	    			} else if (stateB.outputProperty == PassiveMotionOutputProperty.Z) {
	    				rotationB.z = passiveMotionOutput;
	    			}
	    			break;
	    			
	    		}
	    		
	    		translation = Vector3.Lerp(translationA, translationB, progress);
	    		rotation = Vector3.Lerp(rotationA, rotationB, progress);
	    
	    		if ( pc == PanelComponent.PanelPassiveMotionH ) {
	    			passiveMotionTranslationH = Vector3.Lerp( passiveMotionTranslationH, translation, Time.deltaTime * 10);
	    			passiveMotionRotationH = Vector3.Lerp( passiveMotionRotationH, rotation, Time.deltaTime * 10);
	    		} else if ( pc == PanelComponent.PanelPassiveMotionV ) {
	    			passiveMotionTranslationV = Vector3.Lerp( passiveMotionTranslationV, translation, Time.deltaTime * 10);
	    			passiveMotionRotationV = Vector3.Lerp( passiveMotionRotationV, rotation, Time.deltaTime * 10);
	    		}
	    
	    	}
	    
	    }
	    
	    /* -- StateManager methods end -- */
	    
	    public void Update() {
			
			//this.enabled = !(Application.isPlaying && (frameStates.Count == 1) && (cameraStates.Count == 1) && (passiveMotionStatesH.Count == 1) && (passiveMotionStatesV.Count == 1));

	    	/* -- StateManager update start --*/
	    	
			int targetStepProxy = ( int ) Mathf.Max ( Mathf.Floor( PanoplyCore.interpolatedStep ), Mathf.Min ( Mathf.Ceil ( PanoplyCore.interpolatedStep ), PanoplyCore.targetStep ));
	    	scriptIndex = targetStepProxy - startIndex;
			crossfade = PanoplyCore.interpolatedStep - targetStepProxy;
	    	
	    	if ( PanoplyCore.panoplyRenderer != null ) {
				screenRect = PanoplyCore.panoplyRenderer.screenRect;
			}
	    	
	    	if ( crossfade < 0 ) {
				frameStateA = PreviousFrameState();
				frameStateB = CurrentFrameState();
	    		
	    		// preserve camera settings even if we're past the last specified step
	    		if ( scriptIndex > ( cameraStateScript.Count - 1 ) ) {
	    			cameraStateA = cameraStateB = GetKeyStateAtScriptIndex( PanelComponent.PanelCamera, cameraStateScript.Count - 1 ) as CameraState;
	    		} else {
	    			cameraStateA = PreviousCameraState();
	    			cameraStateB = CurrentCameraState();
	    		}
	    		
	    		// preserve passive motion settings even if we're past the last specified step
	    		if ( scriptIndex > ( passiveMotionStateScript.Count - 1 ) ) {
	    			passiveMotionHStateA = passiveMotionHStateB = GetKeyStateAtScriptIndex( PanelComponent.PanelPassiveMotionH, passiveMotionStateScript.Count - 1 ) as PassiveMotionState;
	    			passiveMotionVStateA = passiveMotionVStateB = GetKeyStateAtScriptIndex( PanelComponent.PanelPassiveMotionV, passiveMotionStateScript.Count - 1 ) as PassiveMotionState;
	    			
	    		} else {
	    			passiveMotionHStateA = PreviousPassiveMotionState( PanelComponent.PanelPassiveMotionH );
	    			passiveMotionHStateB = CurrentPassiveMotionState( PanelComponent.PanelPassiveMotionH );
	    			passiveMotionVStateA = PreviousPassiveMotionState( PanelComponent.PanelPassiveMotionV );
	    			passiveMotionVStateB = CurrentPassiveMotionState( PanelComponent.PanelPassiveMotionV );
	    		}
	    		progress = crossfade + 1;
	    		if ( !interpolatePreviousFS ) {
	    			progress = Mathf.Floor( progress );
	    		}
	    		
	    	} else {
				frameStateA = CurrentFrameState();
	    		frameStateB = NextFrameState();
	    		
	    		// preserve camera settings even if we're past the last specified step
	    		if ( scriptIndex > ( cameraStateScript.Count - 1 ) ) {
	    			cameraStateA = cameraStateB = GetKeyStateAtScriptIndex( PanelComponent.PanelCamera, cameraStateScript.Count - 1 ) as CameraState;
	    		} else {
	    			cameraStateA = CurrentCameraState();
	    			cameraStateB = NextCameraState();
	    		}
	    		
	    		// preserve passive motion settings even if we're past the last specified step
	    		if ( scriptIndex > ( passiveMotionStateScript.Count - 1 ) ) {
	    			passiveMotionHStateA = passiveMotionHStateB = GetKeyStateAtScriptIndex( PanelComponent.PanelPassiveMotionH, passiveMotionStateScript.Count - 1 ) as PassiveMotionState;
	    			passiveMotionVStateA = passiveMotionVStateB = GetKeyStateAtScriptIndex( PanelComponent.PanelPassiveMotionV, passiveMotionStateScript.Count - 1 ) as PassiveMotionState;
	    			
	    		} else {
	    			passiveMotionHStateA = CurrentPassiveMotionState( PanelComponent.PanelPassiveMotionH );
	    			passiveMotionHStateB = NextPassiveMotionState( PanelComponent.PanelPassiveMotionH );
	    			passiveMotionVStateA = CurrentPassiveMotionState( PanelComponent.PanelPassiveMotionV );
	    			passiveMotionVStateB = NextPassiveMotionState( PanelComponent.PanelPassiveMotionV );
	    		}
	    		progress = crossfade;
	    		if ( !interpolateNextFS ) {
	    			progress = Mathf.Ceil( progress );
	    		}
	    	}
	    	
	    	/* -- StateManager update end --*/
	    	
	    	/* -- FRAME -- */

			//bool arConstraintsSet = false;

	    	if ( ( frameStateA != null ) && ( frameStateB != null )) {
	    	
	    		Rect rectA = new Rect();
	    		Rect rectB = new Rect();
	    		Rect marginlessFrameRect = new Rect();
	    		Vector2 posVector = Vector2.zero;
	    		Vector2 sizeVector = Vector2.zero;
	    			
	    		rectA = frameStateA.GetRect();
	    		rectB = frameStateB.GetRect();

				// when a frame goes offscreen, preserve its last onscreen size so
				// captions don't end up sticking around
				if (!screenRect.Overlaps(rectA)) {
					if ((rectA.x + rectA.width) <= screenRect.x) {
						rectA.x = (rectA.x + rectA.width) - rectB.width;
					}
					if ((rectA.y + rectA.height) <= screenRect.y) {
						rectA.y = (rectA.y + rectA.height) - rectB.height;
					}
					rectA.width = rectB.width;
					rectA.height = rectB.height;
				}
				if (!screenRect.Overlaps(rectB)) {
					if ((rectB.x + rectB.width) <= screenRect.x) {
						rectB.x = (rectB.x + rectB.width) - rectA.width;
					}
					if ((rectB.y + rectB.height) <= screenRect.y) {
						rectB.y = (rectB.y + rectB.height) - rectA.height;
					}
					rectB.width = rectA.width;
					rectB.height = rectA.height;
				}

				posVector = Vector2.Lerp( new Vector2( rectA.x, rectA.y ), new Vector2( rectB.x, rectB.y ), progress );
	    		sizeVector = Vector2.Lerp( new Vector2( rectA.width, rectA.height ), new Vector2( rectB.width, rectB.height ), progress );
	    		sizeVector.x = Mathf.Max( sizeVector.x, 1.0f );
	    		sizeVector.y = Mathf.Max( sizeVector.y, 1.0f );
	    		frameRect = new Rect( posVector.x, posVector.y, sizeVector.x, sizeVector.y );
				if (( screenRect ).Overlaps( frameRect ) && (frameRect.width >= 1) && (frameRect.height >= 1) && (((frameRect.x - screenRect.x) + frameRect.width) >= 1) && (((frameRect.y - screenRect.y) + frameRect.height) >= 1) && ((screenRect.height - (frameRect.y - screenRect.y)) >= 1) && ((screenRect.width - (frameRect.x - screenRect.x)) >= 1)) {
					GetComponent<Camera>().pixelRect = frameRect;
					GetComponent<Camera>().enabled = true;
				} else {
					GetComponent<Camera>().enabled = false;
				}
	    		
	    		rectA = frameStateA.GetRect( false );
	    		rectB = frameStateB.GetRect( false );
	    		posVector = Vector2.Lerp( new Vector2( rectA.x, rectA.y ), new Vector2( rectB.x, rectB.y ), progress );
	    		sizeVector = Vector2.Lerp( new Vector2( rectA.width, rectA.height ), new Vector2( rectB.width, rectB.height ), progress );
	    		sizeVector.x = Mathf.Max( sizeVector.x, 1.0f );
	    		sizeVector.y = Mathf.Max( sizeVector.y, 1.0f );
	    		marginlessFrameRect = new Rect( posVector.x, posVector.y, sizeVector.x, sizeVector.y );
	    		
	    		// accounts for differences caused by margins
	    		float heightRatio = Mathf.Lerp( frameStateA.heightVal, frameStateB.heightVal, progress ) * .01f;
	    		heightRatio *= marginlessFrameRect.y / frameRect.y;
	    
	    		//Debug.Log( frameRect.height );
	    		/*if (( Mathf.Round( frameRect.width ) == 1 ) || ( frameRect.height == 1 )) {
	    			Debug.Log( 'disable' );
	    			GetComponent.<Camera>().enabled = false;
	    		} else {*/
	    			//GetComponent<Camera>().enabled = ( screenRect ).Overlaps( frameRect );
	    		//}
	    		
	    		/*if (( frameStateA.minAspectRatioVal == 0 ) && ( frameStateA.maxAspectRatioVal == 0 )) {
	    			arConstraintsSet = true;
	    		}*/
	    		
	    	} else {
				frameRect.x = 0;
				frameRect.y = 0;
				frameRect.width = 0;
				frameRect.height = 0;
	    		GetComponent<Camera>().enabled = false;
	    	}
	    	
	    	/*Debug.Log( '----' );
	    	var i : int;
	    	var n : int = passiveMotionStatesH.Count;
	    	for ( i = 0; i < n; i++ ) {
	    		Debug.Log( i + ' - ' + passiveMotionStatesH[ i ] );
	    	}*/
	    	
	    	/* -- PASSIVE MOTION -- */
	    	
	    	CalculatePassiveMotion( PanelComponent.PanelPassiveMotionH );
	    	CalculatePassiveMotion( PanelComponent.PanelPassiveMotionV );
	    	
	    	/* -- CAMERA -- */
	    	
	    	if ( ( cameraStateA != null ) && ( cameraStateB != null ) ) {
	    		
	    		float currentFOV = Mathf.Lerp( cameraStateA.fieldOfViewVal, cameraStateB.fieldOfViewVal, progress );
	    		float targetVerticalFOV = 0.0f;
	    		
	    		// default: crop horizontal, scale vertical (constant fov)
	    		targetVerticalFOV = currentFOV;
	    		
	    		// correct for vertical scale, now crop horizontal and vertical
				/*if ( PanoplyCore.panoplyRenderer != null ) {
					targetVerticalFOV *= ( screenRect.height / PanoplyCore.panoplyRenderer.referenceScreenSize.y );
				}
	    		
	    		// apply scale based on reference dimensions
	    		targetVerticalFOV /= ( PanoplyCore.resolutionScale * 2 );*/
	    		
	    		// make sure we crop vertically even in cases where the panel is not full height
	    		targetVerticalFOV *= ( GetComponent<Camera>().pixelRect.height / screenRect.height );
	    		
	    		GetComponent<Camera>().fieldOfView = Mathf.Min( 180.0f, targetVerticalFOV );
	    		
	    		//GetComponent.<Camera>().fieldOfView = Mathf.Lerp( stateA.fieldOfViewVal, stateB.fieldOfViewVal, progress );
	    		lastCameraPosition = GetComponent<Camera>().transform.position = homePosition + Vector3.Lerp( cameraStateA.position, cameraStateB.position, progress );
	    	
	    		GetComponent<Camera>().transform.Translate( passiveMotionTranslationH );
	    		GetComponent<Camera>().transform.Translate( passiveMotionTranslationV );
	    	
	    		GetComponent<Camera>().transform.Rotate( passiveMotionRotationV );
	    		GetComponent<Camera>().transform.Rotate( passiveMotionRotationV );
	    		
	    		Vector3 cameraRotation = Vector3.Lerp( cameraStateA.rotation, cameraStateB.rotation, progress );
	    		switch ( cameraStateA.orientationType ) {
	    		
	    			case CameraOrientationType.Standard:
	    			var tmp_cs2 = GetComponent<Camera>().transform.rotation;
	                tmp_cs2.eulerAngles = cameraRotation;
	                GetComponent<Camera>().transform.rotation = tmp_cs2;
	    			break;
	    			
	    			case CameraOrientationType.LookAt:
					GetComponent<Camera>().transform.LookAt( Vector3.Lerp( cameraStateA.lookAt, cameraStateB.lookAt, progress ) + homePosition);
	    			GetComponent<Camera>().transform.Rotate( cameraRotation );
	    			break;
	    			
	    		}
	    		
	    		lastCameraRotation = GetComponent<Camera>().transform.rotation;
	    
	    	} else {
	    		GetComponent<Camera>().transform.position = lastCameraPosition;
	    		GetComponent<Camera>().transform.rotation = lastCameraRotation;
	    	}
	    	
	    }
	}
}
