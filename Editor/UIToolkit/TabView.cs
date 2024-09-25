using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace UIToolkit
{
	public class TabsView : VisualElement
	{
		private VisualElement tabContainer;       // Container for tab buttons
		private VisualElement contentContainer;   // Container for the content of the selected tab
		private int selectedTabIndex = -1;        // Track the selected tab index
		private readonly string tabClass = "tab";
		private readonly string selectedTabClass = "selected-tab";
		private readonly string contentClass = "tab-content";
    
		public TabsView()
		{
			// Initialize containers

			this.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/gameever.common/Editor/UIToolkit/Style_TabView.uss"));
			tabContainer = new VisualElement();
			contentContainer = new VisualElement();
        
			// Add them to the root container (this TabsView element)
			tabContainer.style.flexDirection = FlexDirection.Row;
			tabContainer.style.flexWrap = Wrap.Wrap;  // Tab buttons wrap if there's not enough space
			Add(tabContainer);
			Add(contentContainer);

			// Add default styles if needed (Optional)
			style.flexDirection = FlexDirection.Column;  // Tabs stacked vertically, tabContainer first then content
		}

		// Function to add a new tab with title and content
		public void Add(string tabTitle, VisualElement tabContent)
		{
			int tabIndex = tabContainer.childCount;
        
			// Create the button for the tab
			var tabButton = new Button(() => OnTabSelected(tabIndex))
			{
				text = tabTitle
			};
        
			// Apply style classes
			tabButton.AddToClassList(tabClass);
        
			// Add button to the tab container
			tabContainer.Add(tabButton);
        
			// Prepare content and hide initially
			tabContent.style.display = DisplayStyle.None;
			tabContent.AddToClassList(contentClass);
			contentContainer.Add(tabContent);
        
			// Select the first tab by default
			if (selectedTabIndex == -1)
			{
				OnTabSelected(0);
			}
		}

		// Function to handle tab selection
		private void OnTabSelected(int index)
		{
			if (selectedTabIndex == index)
			{
				return; // Tab is already selected
			}

			// Hide the previous tab's content
			if (selectedTabIndex != -1)
			{
				var previousTab = contentContainer[selectedTabIndex];
				var previousButton = tabContainer[selectedTabIndex];
				previousTab.style.display = DisplayStyle.None;
				previousButton.RemoveFromClassList(selectedTabClass);
			}

			// Show the new tab's content
			var newTab = contentContainer[index];
			var newButton = tabContainer[index];
			newButton.AddToClassList(selectedTabClass);

			// Play fade-in animation (simple opacity animation)
			newTab.style.display = DisplayStyle.Flex;
			newTab.style.opacity = 0;
			newTab.experimental.animation.Start(new StyleValues { opacity = 0 }, new StyleValues { opacity = 1 }, 300);
			selectedTabIndex = index;
		}
    
		// Optionally add UXML support (Not required for base functionality)
		public new class UxmlFactory : UxmlFactory<TabsView, UxmlTraits> { }
	}
	
	
}
