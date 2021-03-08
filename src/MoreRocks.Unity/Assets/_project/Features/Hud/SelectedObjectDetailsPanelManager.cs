using Assets._project.Features;
using Assets._project.Features.Units;
using Assets.Features.Player;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectedObjectDetailsPanelManager : MonoBehaviour
{
    [Header("Single select")]
    [SerializeField] GameObject singleObjectPanel;
    [SerializeField] Text displayNameText;

    [Header("Multi select")]
    [SerializeField] GameObject multiObjectPanel;

    private void HideAllPanels()
    {
        if (singleObjectPanel != null)
        {
            singleObjectPanel.SetActive(false);
        }

        if (multiObjectPanel != null)
        {
            multiObjectPanel.SetActive(false);
        }
    }

    private void DisplaySelectableObject(ISelectable selectableObject)
    {
        if (singleObjectPanel == null)
        {
            return;
        }

        HideAllPanels();
        singleObjectPanel.SetActive(true);

        // TODO: this is a code smell, every time I add a new selectable thing I have to add to 
        // this if statement
        if (selectableObject.gameObject.TryGetComponent(out BaseUnit unit))
        {
            displayNameText.text = unit.DisplayName;
        }
        else if (selectableObject.gameObject.TryGetComponent(out ResourceNode resource))
        {
            displayNameText.text = resource.DisplayName;
        }
        else
        {
            displayNameText.text = "something";
        }
    }


    public void HandleSelectionChanged(SelectionChangedEventArgs args)
    {
        Debug.Log("Selection changed event handled", this);

        if (args.SelectedObjects.Count == 0)
        {
            HideAllPanels();
        }
        else if (args.SelectedObjects.Count == 1)
        {
            DisplaySelectableObject(args.SelectedObjects.First());
        }
        else
        {
            // TODO multiselect
        }
    }
}
