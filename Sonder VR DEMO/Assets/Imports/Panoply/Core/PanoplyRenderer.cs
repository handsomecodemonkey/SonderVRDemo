using UnityEngine;
using System;
using Opertoon.Panoply;

/**
 * The PanoplyRenderer class executes global rendering tasks.
 * Copyright © Erik Loyer
 * erik@opertoon.com
 * Part of the Panoply engine
 */

namespace Opertoon.Panoply {
	[ExecuteInEditMode()]
	public class PanoplyRenderer: MonoBehaviour {
	     
	    public Vector2 referenceScreenSize = new Vector2( 1024.0f, 768.0f );
	    
	    [Range(0.0f,1.0f)]
	    public float matchWidthHeight = 0.5f;

		public bool enforceAspectRatio = false;

		[HideInInspector]
		public float aspectRatio;
		
		[HideInInspector]
		public Rect screenRect = new Rect();			// size of the addressable screen area after aspect ratio enforcement (if any)
		
		[HideInInspector]
		public Rect scaledScreenRect = new Rect();		// the screenRect, scaled according to the current resolutionScale

	    Texture2D matteTexture;
	    Panel[] panels;
		Caption[] captions;
	    //ScriptablePanel[] scriptablePanels;
	    
	    public void Start() {
	    
	    	matteTexture = new Texture2D (1, 1);
	    	matteTexture.SetPixel(0, 0, Color.white);
	    	matteTexture.Apply();
	    
			UpdateInventory();

	    }

		public void UpdateInventory() {
			panels = FindObjectsOfType( typeof( Panel ) ) as Panel[];
			captions = FindObjectsOfType( typeof( Caption ) ) as Caption[];
			//scriptablePanels = FindObjectsOfType( typeof( ScriptablePanel ) ) as ScriptablePanel[];
		}

		public void RenderBlackout( Rect drawRect ) {
			GUI.color = Color.black;
			GUI.DrawTexture( new Rect( drawRect ), matteTexture );
		}
	    
	    public void RenderFrame( FrameState stateA,
	                             FrameState stateB,
	                             Rect drawRect,
	                             float progress ) {
	    
	    	Color color = Color.clear;
	    
	    	if (( stateA != null ) && ( stateB != null )) {
	    		color = Color.Lerp( stateA.matteColor, stateB.matteColor, progress );
	    		if ( color != Color.clear ) {
	    			GUI.color = color;
	    			drawRect.y = Screen.height - drawRect.y - drawRect.height;
	    			drawRect.y--;
	    			drawRect.height++;
	    			drawRect.x--;
	    			drawRect.width++;
	    			GUI.DrawTexture( new Rect( drawRect ), matteTexture );
	    		}
	    	}
	    
	    }
	    
	    public void RenderSelection( Rect drawRect ) {
	    
	    	//GUI.color = Color( .3, .49, .84, 1.0 );
	    	GUI.color = Color.gray;
	    	GUI.DrawTexture( new Rect( drawRect.x, drawRect.y, drawRect.width, 1.0f ), matteTexture ); 
	    	GUI.DrawTexture( new Rect( drawRect.x + drawRect.width - 1, drawRect.y, 1.0f, drawRect.height ), matteTexture ); 
	    	GUI.DrawTexture( new Rect( drawRect.x, drawRect.y + drawRect.height - 1, drawRect.width, 1.0f ), matteTexture ); 
	    	GUI.DrawTexture( new Rect( drawRect.x, drawRect.y, 1.0f, drawRect.height ), matteTexture ); 
	    	GUI.color = Color.white;
	    
	    }
	    
	    public void OnGUI() {
	    
	    	GUI.depth = 10;
	    
	    	int i = 0;
	    	int n = 0;
	    	Panel panel = null;
			Caption caption;
	    	//ScriptablePanel scriptablePanel = null;
			
			aspectRatio = referenceScreenSize.x / referenceScreenSize.y;
			
			if ( enforceAspectRatio ) {
				float screenAR = Screen.width / ( float )Screen.height;
				if ( aspectRatio > screenAR ) {
					screenRect.width = Screen.width;
					screenRect.height = Screen.width / aspectRatio;
				} else {
					screenRect.width = Screen.height * aspectRatio;
					screenRect.height = Screen.height;
				}
				screenRect.x = ( Screen.width - screenRect.width ) * .5f;
				screenRect.y = ( Screen.height - screenRect.height ) * .5f;
			} else {
				screenRect.x = 0.0f;
				screenRect.y = 0.0f;
				screenRect.width = Screen.width;
				screenRect.height = Screen.height;
			}
			scaledScreenRect.x = screenRect.x / PanoplyCore.resolutionScale;
			scaledScreenRect.y = screenRect.y / PanoplyCore.resolutionScale;
			scaledScreenRect.width = screenRect.width / PanoplyCore.resolutionScale;
			scaledScreenRect.height = screenRect.height / PanoplyCore.resolutionScale;

	    	n = panels.Length;
	    	for( i = 0; i < n; i++ ) {
	    		panel = panels[ i ];
	    		if (( panel != null ) && panel.enabled ) {
	    			RenderFrame( panel.frameStateA, panel.frameStateB, panel.frameRect, panel.progress );
	    		}
	    	}
			
			n = captions.Length;
			for( i = 0; i < n; i++ ) {
				caption = captions[ i ];
				if (( caption != null ) && caption.enabled ) {
					caption.Render();
				}
			}

			// draw pillarbox bars
			if ( screenRect.x > 0.0f ) {
				RenderBlackout( new Rect( 0.0f, 0.0f, screenRect.x, Screen.height ) );
				RenderBlackout( new Rect( Screen.width - screenRect.x, 0.0f, screenRect.x, Screen.height ) );
			}

			// draw letterbox bars
			if ( screenRect.y > 0.0f ) {
				RenderBlackout( new Rect( 0.0f, 0.0f, Screen.width, screenRect.y ) );
				RenderBlackout( new Rect( 0.0f, Screen.height - screenRect.y, Screen.width, screenRect.y ) );
			}

	    	/*n = scriptablePanels.Length;
	    	for( i = 0; i < n; i++ ) {
	    		scriptablePanel = scriptablePanels[ i ];
	    		if ( scriptablePanel != null ) {
	    			renderFrame( scriptablePanel.frameStateA, scriptablePanel.frameStateB, scriptablePanel.frameRect, scriptablePanel.progress );
	    		}
	    	}*/
	    
	    }

		public void Update() {
			aspectRatio = referenceScreenSize.x / referenceScreenSize.y;
		}
	}
}