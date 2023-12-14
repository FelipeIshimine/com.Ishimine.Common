using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Legion.Editor
{
    [Overlay(typeof(SceneView), "Layers", true, defaultDockPosition = DockPosition.Top, defaultDockZone = DockZone.RightToolbar)]
    public class LayerSceneViewOverlay : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement()
            {
                name = "Layers"
            };

            var container = new VisualElement();

            // Get all layer names
            string[] layerNames = UnityEditorInternal.InternalEditorUtility.layers;

            for (var index = 0; index < layerNames.Length; index++)
            {
                var layerName = layerNames[index];
                // Create a horizontal container for each layer
                var layerContainer = new VisualElement() { style = { flexDirection = FlexDirection.Row } };

                // Image for visibility icon
                var visibilityOnIcon = EditorGUIUtility.IconContent("d_scenevis_visible").image as Texture2D;
                var visibilityOffIcon = EditorGUIUtility.IconContent("d_scenevis_hidden").image as Texture2D;
                var layerIndex = index;

                var flag = (1 << layerIndex);

                // Clickable image for toggling visibility
                bool isVisible = (Tools.visibleLayers & flag) != 0;
                var toggleVisibility = new Image();
                toggleVisibility.image = isVisible ? visibilityOnIcon : visibilityOffIcon; // Default to showing the layer
                toggleVisibility.AddManipulator(new Clickable(() =>
                {
	                isVisible = (Tools.visibleLayers & flag) != 0;
	                
                    // Toggle visibility on click
                    toggleVisibility.image = isVisible ? visibilityOffIcon : visibilityOnIcon;
                    Tools.visibleLayers = isVisible
                        ? (Tools.visibleLayers & ~flag)
                        : (Tools.visibleLayers | flag);
                }));

                // Image for interactability icon
                var lockedOnIcon = EditorGUIUtility.IconContent("d_scenepicking_notpickable").image as Texture2D;
                var lockedOffIcon = EditorGUIUtility.IconContent("d_scenepicking_pickable").image as Texture2D;

                // Clickable image for toggling interactability
                bool isLocked = (Tools.lockedLayers & flag) != 0;
                var toggleLocked = new Image();
                toggleLocked.image = isLocked ?lockedOnIcon:lockedOffIcon; // Default to interactable
                toggleLocked.AddManipulator(new Clickable(() =>
                {
	                isLocked = (Tools.lockedLayers & flag) != 0;
                    toggleLocked.image = isLocked ? lockedOffIcon : lockedOnIcon;
                    Tools.lockedLayers = isLocked ? (Tools.lockedLayers & ~flag) : (Tools.lockedLayers | flag);
                }));

                // Label for the layer name
                var layerLabel = new Label($"{index}: {layerName}");

                layerContainer.Add(layerLabel);
                layerContainer.Add(new VisualElement()
                {
	                style =
	                {
		                flexGrow = 1
	                }
                });
                layerContainer.Add(toggleVisibility);
                layerContainer.Add(toggleLocked);

                // Add the layer container to the main container
                container.Add(layerContainer);
            }

            root.Add(container);

            return root;
        }
        
        
        
    }
}
