using UnityEngine;
using System;
using System.Collections.Generic;
using Opertoon.Panoply;

/**
 * Caption
 * Manages text superimposed over a panel.
 * Copyright © Erik Loyer
 * erik@opertoon.com
 * Part of the Panoply engine
 */

namespace Opertoon.Panoply {

	[ExecuteInEditMode()]
	public class Caption: MonoBehaviour {  
	    
	    /* -- StateManager properties start -- */
	    
	    public List<CaptionState> states = new List<CaptionState>();		// Library of possible states
	    
	    public int startIndex = 0;											// The index at which this item's script starts
	    public List<string> stateScript = new List<string>();				// Array of state ids that describe how it changes over time
	    public int scriptIndex = 0;											// Index of our current position in the state script
	    
	    public float crossfade;												// Range from -1 to 1; 0 is current frame state, -1 is previous, 1 is next
	    
	    public int[] scriptStateIndices;										// The index of each step's state							
	    public float editorScrubberScriptIndex;								// Position of the scrubber being used to edit this component
	    
	    /* -- StateManager properties end -- */
	    
	    public int stateCounter = 0;
	    
	    private CaptionState stateQ;
		private float progress;
		private float tailProgress = 0.0f;
		private Panel panel;
		//private ScriptablePanel scriptablePanel;
		private float rotOffset = -90.0f;
		private AbstractLocalizer localizer;
	    
	    public void Start() {
	    
	    	panel = GetComponent<Panel>();
	    	//scriptablePanel = GetComponent<ScriptablePanel>();

			if (PanoplyCore.panoplyRenderer != null) {
				PanoplyCore.panoplyRenderer.UpdateInventory();
			}

			/*int i;
			int n = states.Count;
			CaptionState state;
			for (i=0; i<n; i++) {
				state = states[i];
				if (state.skin == null) {
					state.skin = Resources.Load ("CenteredRoundRectCaption") as GUISkin;
				}
				if (state.tail == null) {
					state.tail = Resources.Load ("balloon_stem") as Texture2D;
				}
			}*/
	    	
	    }
	    
	    /* -- StateManager methods start -- */
	    
	    public CaptionState GetStateForId( string id ) {
	    
	    	int i = 0;
	    	int n = states.Count;
	    	CaptionState state = null;
	    	id = GetCleanIdFromId( id );
	    	
	    	for( i = 0; i < n; i++ ) {
	    		state = states[ i ];
	    		if ( state.id == id ) {
	    			return state;
	    		}
	    	}
	    	
	    	return null;
	    }
	    
	    public int GetStateIndexForId( string id ) {
	    
	    	int i = 0;
	    	int n = states.Count;
	    	CaptionState state = null;
	    	id = GetCleanIdFromId( id );
	    	
	    	for( i = 0; i < n; i++ ) {
	    		state = states[ i ];
	    		if ( state.id == id ) {
	    			return i;
	    		}
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
	    
	    public void PushState( string id, bool deleteFuture ) {
	    
	    	if ( deleteFuture && ( scriptIndex < ( stateScript.Count - 1 ) ) ) {
	    		stateScript.RemoveRange( scriptIndex + 1, stateScript.Count - scriptIndex );
	    	}
	    
	    	CaptionState state = GetStateForId( id );
	    	if ( state != null ) {
	    		stateScript.Add( state.id );
	    	}
	    	
	    }
	    
	    public void SetCurrentState( string id ) {
	    
	    	CaptionState state = GetStateForId( id );
	    	if ( state != null ) {
	    		if ( stateScript.Count == 0 ) {
	    			PushState( state.id, false );
	    		} else {
	    			stateScript[ scriptIndex ] = state.id;
	    		}
	    	}
	    
	    }
	    
	    public void DecrementState() {
	    
	    	if ( scriptIndex > 1 ) {
	    		scriptIndex--;
	    	}
	    
	    }
	    
	    public void IncrementState() {
	    	
	    	if ( scriptIndex < ( stateScript.Count - 1 ) ) {
	    		scriptIndex++;
	    	}
	    	
	    }
	    
	    public CaptionState GetKeyStateAtScriptIndex( int index ) {
	    
	    	if (( index >= 0 ) && ( index < stateScript.Count )) {
	    		string id = stateScript[ index ];
	    		
	    		if ( id != CaptionState.HoldCommand ) {
	    			return GetStateForId( stateScript[ index ] );
	    		}
	    	}
	    	
	    	return null;
	    }
	    
	    public CaptionState GetStateAtScriptIndex( int index ) {
	    
	    	if (( index >= 0 ) && ( index < stateScript.Count )) {
	    		string id = stateScript[ index ];
	    		
	    		if ( id == CaptionState.HoldCommand ) {
	    		
	    			int i = 0;
	    			CaptionState captionState = null;
	    			
	    			for( i = ( index - 1 ); i >= 0; i-- ) {
	    				captionState = GetStateForId( stateScript[ i ] );
	    				if ( captionState != null ) {
	    					return captionState;
	    				}
	    			}
	    			
	    		} else {
	    			return GetStateForId( stateScript[ index ] );
	    		}
	    	}
	    
	    	return null;
	    }
	    
	    public CaptionState PreviousState() {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex - 1, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( stateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( stateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( scriptIndexProxy );
		    	}
			} else {
				return null;
			}

	    }
	    
	    public CaptionState CurrentState() {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( stateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( stateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( scriptIndexProxy );
		    	}
			} else {
				return null;
			}

	    }
	    
	    public CaptionState NextState() {
			
			if (PanoplyCore.scene != null) {
				int scriptIndexProxy = Mathf.Clamp( scriptIndex + 1, 0, PanoplyCore.scene.stepCount - 1 );

		    	if ( ( stateScript.Count == 0 ) || ( scriptIndex < 0 ) || ( scriptIndex > ( stateScript.Count - 1 ) ) ) {
		    		return null;
		    	} else {
					return GetStateAtScriptIndex( scriptIndexProxy );
		    	}
			} else {
				return null;
			}

	    }
	    
	    public void RenameStates() {
	    
	    	int i = 0;
	    	int j = 0;
	    	int n = states.Count;
	    	int o = stateScript.Count;
	    	CaptionState state = null;
	    	string oldId = null;
	    	
	    	// rename the states numerically, and update references in
	    	// the state script to match (not foolproof if the state script
	    	// already references two states with the same name)
	    	for( i = 0; i < n; i++ ) {
	    		state = states[ i ];
	    		oldId = state.id;
	    		state.id = "State " + ( i + 1 );
	    		for( j = 0; j < o; j++ ) {
	    			if ( stateScript[ j ] == oldId ) {
	    				stateScript[ j ] = state.id;
	    			}
	    		}
	    	}
	    
	    }

	    /* -- StateManager methods end -- */

	    public void Render() {

	    	/* -- StateManager update start --*/
	    				
			int targetStepProxy = ( int ) Mathf.Max ( Mathf.Floor( PanoplyCore.interpolatedStep ), Mathf.Min ( Mathf.Ceil ( PanoplyCore.interpolatedStep ), PanoplyCore.targetStep ));
			scriptIndex = targetStepProxy - startIndex;
			crossfade = PanoplyCore.interpolatedStep - targetStepProxy;

	    	CaptionState stateA = null;
	    	CaptionState stateB = null;
	    	
	    	if ( crossfade < 0 ) {
	    		stateA = PreviousState();
	    		stateB = CurrentState();
	    		progress = crossfade + 1;
	    		
	    	} else {
	    		stateA = CurrentState();
	    		stateB = NextState();
	    		progress = crossfade;
	    	}
	    	
	    	if ( Mathf.Round( progress ) == 0 ) {
	    		stateQ = stateA;
	    	} else {
	    		stateQ = stateB;
	    	}
	    	
	    	/* -- StateManager update end --*/
			
			bool okToCalc = true;
			/*if (Application.isPlaying && (states.Count == 1)) {
				okToCalc = false;
			} else if ((scriptIndex > 0) && (scriptIndex < (stateScript.Count-1))) {
				if ((stateScript[scriptIndex-1] == CaptionState.HoldCommand) && (stateScript[scriptIndex] == CaptionState.HoldCommand) && (stateScript[scriptIndex+1] == CaptionState.HoldCommand)) {
					okToCalc = false;
				}
			}*/

	    	if ( ( stateA != null ) && ( stateB != null ) && okToCalc ) {
	    
	    		if (( panel != null ) && (PanoplyCore.resolutionScale != 0) /*|| ( scriptablePanel != null )*/) {
	    			
	    			GUI.depth = 1;
	    		
	    			float guiScale = PanoplyCore.resolutionScale;
	    			Matrix4x4 scaledMatrix = Matrix4x4.Scale( new Vector3( guiScale, guiScale, guiScale ));
	        		GUI.matrix = scaledMatrix;
	    		
	    			GUISkin currentSkin = GUI.skin;
	    			GUI.skin = stateQ.skin;
	    			
	    			Vector2 posVector = Vector2.zero;
	    			Vector2 sizeVector = Vector2.zero;
	    			Rect layout = new Rect();
	    			Vector2 dropShadow = Vector2.zero;
	    			Color dropShadowColor = Color.clear;
	    			float tailWidth = 0.0f;
	    			float tailHeight = 0.0f;
	    			float tailRotation = 0.0f;
	    			Color color = Color.clear;
	    			
	    			Rect frameRect = new Rect();
	    			if ( panel != null ) {
	    				frameRect = panel.frameRect;
	    			/*} else if ( scriptablePanel != null ) {
	    				frameRect = scriptablePanel.frameRect;*/
					}

					if ((frameRect.width > 1) && (frameRect.height > 1)) {

						frameRect.x /= PanoplyCore.resolutionScale;
		    			frameRect.y /= PanoplyCore.resolutionScale;
		    			frameRect.width /= PanoplyCore.resolutionScale;
		    			frameRect.height /= PanoplyCore.resolutionScale;
		    			
		    			float xOffsetA = 0.0f;
		    			float yOffsetA = 0.0f;
		    			float xOffsetB = 0.0f;
		    			float yOffsetB = 0.0f;
		    		
		    			if (( stateA.offsetUnitsX == PositionUnits.Pixels ) || stateQ.drawTail )  {
		    				xOffsetA = stateA.layout.x;
		    			} else {
		    				xOffsetA = frameRect.width * ( stateA.layout.x * .01f );
		    			}
		    		
		    			if (( stateA.offsetUnitsY == PositionUnits.Pixels ) || stateQ.drawTail ) {
		    				yOffsetA = stateA.layout.y;
		    			} else {
		    				yOffsetA = frameRect.height * ( stateA.layout.y * .01f );
		    			}
		    		
		    			if (( stateB.offsetUnitsX == PositionUnits.Pixels ) || stateQ.drawTail ) {
		    				xOffsetB = stateB.layout.x;
		    			} else {
		    				xOffsetB = frameRect.width * ( stateB.layout.x * .01f );
		    			}
		    		
		    			if (( stateB.offsetUnitsY == PositionUnits.Pixels ) || stateQ.drawTail ) {
		    				yOffsetB = stateB.layout.y;
		    			} else {
		    				yOffsetB = frameRect.height * ( stateB.layout.y * .01f );
		    			}
		    			
		    			//Debug.Log( xOffsetA + ' ' + yOffsetA + ' ' + xOffsetB + ' ' + yOffsetB );
		    			
		    			posVector = Vector2.Lerp( new Vector2( xOffsetA, yOffsetA ), new Vector2( xOffsetB, yOffsetB ), progress );
		    			sizeVector = Vector2.Lerp( new Vector2( stateA.layout.width, stateA.layout.height ), new Vector2( stateB.layout.width, stateB.layout.height ), progress );
		    			layout = new Rect( posVector.x, posVector.y, sizeVector.x, sizeVector.y );
		    			dropShadow = Vector2.Lerp( new Vector2( stateA.dropShadow.x, stateA.dropShadow.y ), new Vector2( stateB.dropShadow.x, stateB.dropShadow.y ), progress );
		    			dropShadowColor = Color.Lerp( stateA.dropShadowColor, stateB.dropShadowColor, progress );
		    			tailWidth = Mathf.Lerp( stateA.tailWidth, stateB.tailWidth, progress );
		    			tailHeight = Mathf.Lerp( stateA.tailHeight, stateB.tailHeight, progress );
		    			tailRotation = Mathf.Lerp( stateA.tailRotation, stateB.tailRotation, progress );
		    			color = Color.Lerp( stateA.color, stateB.color, progress );
		    			
		    			sizeVector *= 2;
		    			dropShadow *= 2;
		    			layout.width *= 2.0f;
		    			layout.height *= 2.0f;
		    			tailWidth *= 2.0f;
		    			tailHeight *= 2.0f;
		    			
		    			/*sizeVector *= PanoplyCore.resolutionScale;
		    			dropShadow *= PanoplyCore.resolutionScale;
		    			tailWidth *= PanoplyCore.resolutionScale;
		    			tailHeight *= PanoplyCore.resolutionScale;
		    			layout.width *= PanoplyCore.resolutionScale;
		    			layout.height *= PanoplyCore.resolutionScale;*/
		    			
		    			bool drawDropShadow = (( dropShadow.x != 0 ) && ( dropShadow.y != 0 ));

						string currentText;
						if ( localizer == null ) {
							currentText = stateQ.text;
						} else {
							currentText = localizer.GetLocalizedText( stateQ.text );
							if (currentText == null) {
								Debug.Log ("ALERT: Could not find localized text for the caption on "+this.name);
							}
						}
		    		
		    			if ( stateQ.drawTail ) {
		    				
		    				float tailRotationRadians = 0.0f;
		    				float tailOffset = 0.0f;
		    				Vector2 tailTipOffset = Vector2.zero;
		    
		    				Rect pixelPosition = layout;
		    				pixelPosition.x *= frameRect.width;
		    				pixelPosition.x += frameRect.x;
		    				pixelPosition.y *= frameRect.height;
							pixelPosition.y += PanoplyCore.panoplyRenderer.scaledScreenRect.height - ( frameRect.y + frameRect.height ) + ( PanoplyCore.panoplyRenderer.scaledScreenRect.y * 2.0f );
		    				
		    				//pixelPosition.width *= ( PanoplyCore.resolutionScale * 2 );
		    				//pixelPosition.height *= ( PanoplyCore.resolutionScale * 2 );
		    			
		    				tailRotationRadians = Mathf.Deg2Rad * tailRotation;
		    				tailOffset = Mathf.Lerp( pixelPosition.width * .5f, pixelPosition.height * .5f, Mathf.Abs( Mathf.Cos( tailRotationRadians ) ) ) * ( .75f * ( tailHeight / 200.0f ) );
		    				tailOffset /= 2.0f;
		    				tailTipOffset = new Vector2( ( tailOffset + tailHeight ) * Mathf.Cos( tailRotationRadians + ( rotOffset * Mathf.Deg2Rad ) ), ( tailOffset + tailHeight ) * Mathf.Sin( tailRotationRadians + ( rotOffset * Mathf.Deg2Rad ) ) );
		    				
		    				if ( stateQ.color.a == 1.0f ) {
		    					tailProgress = Mathf.Lerp( tailProgress, 1.0f, Time.deltaTime * 10 );
		    				} else {
		    					tailProgress = 0.0f;
		    				}
		    				
		    				//Vector2 center = new Vector2( pixelPosition.x + ( pixelPosition.width * .5f ) + tailTipOffset.x, pixelPosition.y + ( pixelPosition.height * .5f ) + tailTipOffset.y );
		    				
		    				float dropShadowAngle = Mathf.Atan2( dropShadow.y, dropShadow.x ) - tailRotationRadians;
		    				Vector2 dropShadowOffset = new Vector2( dropShadow.magnitude * Mathf.Cos( dropShadowAngle ), dropShadow.magnitude * Mathf.Sin( dropShadowAngle ) );
		    
		    				// stroke elements
		    				if ( drawDropShadow ) {
		    					GUI.color = dropShadowColor;
		    					var tmp_cs1 = GUI.color;
		                        tmp_cs1.a = color.a;
		                        GUI.color = tmp_cs1;
		    					GUI.Label( new Rect( pixelPosition.x + ( dropShadow.x * color.a ) + tailTipOffset.x - ( pixelPosition.width * .5f ), pixelPosition.y + ( dropShadow.y * color.a ) + tailTipOffset.y - ( pixelPosition.height * .5f ), pixelPosition.width, pixelPosition.height ), "" );
		    					GUIUtility.RotateAroundPivot( tailRotation, new Vector2( pixelPosition.x * PanoplyCore.resolutionScale, pixelPosition.y * PanoplyCore.resolutionScale ) );
								if (( stateQ.color.a == 1.0f ) && (stateQ.tail != null)) {
		    						GUI.DrawTexture( new Rect( pixelPosition.x - ( tailWidth * .5f ) + dropShadowOffset.x, pixelPosition.y - tailHeight + dropShadowOffset.y, tailWidth, ( tailHeight * tailProgress ) ), stateQ.tail );
		    					}
		    				} else {
		    					GUIUtility.RotateAroundPivot( tailRotation, new Vector2( pixelPosition.x * PanoplyCore.resolutionScale, pixelPosition.y * PanoplyCore.resolutionScale ) );
		    				}
		    				
		    				// main elements
		    				GUI.color = color;
							if (( stateQ.color.a == 1.0f ) && (stateQ.tail != null)) {
		    					GUI.DrawTexture( new Rect( pixelPosition.x - ( tailWidth * .5f ), pixelPosition.y - tailHeight, tailWidth, ( tailHeight * tailProgress ) ), stateQ.tail );
		    				}
		    				GUIUtility.RotateAroundPivot( -tailRotation, new Vector2( pixelPosition.x * PanoplyCore.resolutionScale, pixelPosition.y * PanoplyCore.resolutionScale ) );
							GUI.Label( new Rect( pixelPosition.x + tailTipOffset.x - ( pixelPosition.width * .5f ), pixelPosition.y + tailTipOffset.y - ( pixelPosition.height * .5f ), pixelPosition.width, pixelPosition.height ), currentText );
		    				
		    			} else {
		    			
		    				//layout.x *= 2.0f;
		    				//layout.y *= 2.0f;
		    				//layout.x *= PanoplyCore.resolutionScale;
		    				//layout.y *= PanoplyCore.resolutionScale;
		    			
		    				Rect labelRect = layout;
							if (PanoplyCore.panoplyRenderer != null) {
								frameRect.y = PanoplyCore.panoplyRenderer.scaledScreenRect.height - ( frameRect.y + frameRect.height ) + ( PanoplyCore.panoplyRenderer.scaledScreenRect.y * 2.0f );
							}

		    				switch ( stateQ.horzAlign ) {
		    				
		    					case CaptionHorzAlign.Left:
		    					labelRect.x = frameRect.x + layout.x;
		    					break;
		    					
		    					case CaptionHorzAlign.Center:
		    					labelRect.x = frameRect.x + ( frameRect.width * .5f ) - ( layout.width * .5f ) + layout.x;
		    					break;
		    					
		    					case CaptionHorzAlign.Right:
		    					labelRect.x = frameRect.x + frameRect.width - layout.width - layout.x;
		    					break;
		    					
		    				}
		    				
		    				switch ( stateQ.vertAlign ) {
		    				
		    					case CaptionVertAlign.Top:
		    					labelRect.y = frameRect.y + layout.y;
		    					break;
		    					
		    					case CaptionVertAlign.Center:
		    					labelRect.y = frameRect.y + ( frameRect.height * .5f ) - ( layout.height * .5f ) + layout.y;
		    					break;
		    					
		    					case CaptionVertAlign.Bottom:
		    					labelRect.y = frameRect.y + frameRect.height - layout.height - layout.y;
		    					break;
		    					
		    				}
		    				
		    				if ( drawDropShadow ) {
		    					GUI.color = dropShadowColor;
		    					var tmp_cs2 = GUI.color;
		                        tmp_cs2.a = color.a;
		                        GUI.color = tmp_cs2;
		    					labelRect.x += dropShadow.x;
		    					labelRect.y += dropShadow.y;
		    					GUI.Label( labelRect, "" );
		    				}
		    				GUI.color = color;
		    				labelRect.x -= dropShadow.x;
		    				labelRect.y -= dropShadow.y;
		    				
		    				labelRect.x = Mathf.Round( labelRect.x );
		    				labelRect.y = Mathf.Round( labelRect.y );
		    				labelRect.width = Mathf.Round( labelRect.width );
		    				labelRect.height = Mathf.Round( labelRect.height );

							GUI.Label( labelRect, currentText );
		    			
		    			}
						
					}

	    			GUI.skin = currentSkin;
	    			GUI.color = Color.white;
					GUI.matrix = Matrix4x4.identity;
	    		
	    		}
	    		
	    	}
	    
	    }
	    
	    public void Update() {

			if (localizer == null) {
				if (PanoplyCore.scene != null) {
					if (PanoplyCore.scene.useLocalization) {
						localizer = PanoplyCore.scene.localizer;
					}
				}
			}

	    }
	}
}
