using Assets._project.Features;
using Assets._project.Features.Units;
using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Features.Player
{
    /// <summary>
    /// The command & control centrepiece for a player.
    /// </summary>
    public class C2Manager : MonoBehaviour
    {
        [SerializeField] KeyCode multiselectKey = KeyCode.LeftControl;

        public SelectionChangedEvent onSelectionChanged;
        
        private Vector3 dragStartPosition;
        
        private List<ISelectable> selectedObjects = new List<ISelectable>();

        private bool isDragging = false;

        private Camera mainCamera;

        public IReadOnlyList<ISelectable> SelectedObjects => selectedObjects;
        public IReadOnlyList<BaseUnit> SelectedUnits => selectedObjects.OfType<BaseUnit>().ToList();

        private List<BaseUnit> _ownedUnits = new List<BaseUnit>();


        void Start()
        {
            mainCamera = Camera.main;
            _ownedUnits = FindObjectsOfType<BaseUnit>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
            // handle left click - selection concerns
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt))
            {
                dragStartPosition = Input.mousePosition;

                var camRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(camRay, out RaycastHit hit))
                {
                    if (hit.transform.TryGetComponent(out ISelectable selectable))
                    {
                        var coordinates = $"({hit.transform.position.x}, {hit.transform.position.y}, {hit.transform.position.z}";
                        Debug.Log($"Click hit a selectable object at {coordinates}", this);
                        Select(selectable, Input.GetKey(KeyCode.LeftControl));
                    }
                    else
                    {
                        isDragging = true;
                    }

                }
                else
                {
                    Debug.Log("Click hit nothing", this);
                }
            }

            if (isDragging && Input.GetMouseButtonUp(0))
            {
                // This responds to a new click & drag event, so if we already have something selected we want
                // to clear that selection unless the player is pressing the CTRL button
                if (!Input.GetKey(multiselectKey))
                {
                    ClearSelection();
                }

                // TODO investigate other ways to do this
                foreach (var unit in _ownedUnits)
                {
                    if (IsWithinSelectionBounds(unit.transform))
                    {
                        if (unit.TryGetComponent(out ISelectable selectable))
                        {
                            Select(selectable, Input.GetKey(KeyCode.LeftControl));
                        }
                    }
                }

                isDragging = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (SelectedUnits.Count > 0)
                {
                    var camRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(camRay, out RaycastHit hit))
                    {
                        if (hit.collider.CompareTag("Ground"))
                        {
                            var location = hit.point;
                            var coordinates = $"({location.x}, {location.y}, {location.z})";
                            Debug.Log($"Issuing a move command to point {coordinates}");
                            IssueMoveCommand(location);
                        }
                        else
                        {
                            IssueInteractCommand(hit.collider.gameObject);
                        }

                    }
                }
                else
                {
                    // TODO: provide feedback?
                    // no-op for now
                }
            }
        }

        void OnGUI()
        {
            if (isDragging)
            {
                var rect = ScreenHelper.GetScreenRect(dragStartPosition, Input.mousePosition);
                ScreenHelper.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.1f));
                ScreenHelper.DrawScreenRectBorder(rect, 1, Color.green);
            }
        }

        /// <summary>
        /// Tells all selected units to move to the given location.
        /// </summary>
        /// <param name="location"></param>
        private void IssueMoveCommand(Vector3 location)
        {
            // TODO: platoon logic
            foreach (var selectedObject in selectedObjects)
            {
                if (selectedObject is BaseUnit)
                {
                    (selectedObject as BaseUnit).Move(location);   
                }
            }
        }

        private void IssueInteractCommand(GameObject gameObject)
        {
            foreach (var unit in SelectedUnits)
            {
                unit.Interact(gameObject);
            }
        }

        private void IssueGuardMoveCommand(Vector3 location)
        {
            // todo
            IssueMoveCommand(location);
        }

        private bool IsWithinSelectionBounds(Transform transform)
        {
            if (!isDragging)
            {
                return false;
            }

            var camera = Camera.main;
            var viewportBounds = ScreenHelper.GetViewportBounds(camera, dragStartPosition, Input.mousePosition);
            return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
        }

        //public void SelectUnit(SelectableObject selectableObject, bool isMultiSelect = false)
        //{
        //    if (selectableObject == null)
        //    {
        //        Debug.LogWarning("Selectable object component on the provided game object is null");
        //        return;
        //    }

        //    if (!isMultiSelect)
        //    {
        //        // if we aren't doing a multiselect, we're clearing our current selection so that
        //        // the latest unit will be our focus
        //        ClearSelection();
        //    }
        //    else if (isMultiSelect && !selectableObject.IsMultiSelectEnabled)
        //    {
        //        // if we are multiselecting, and this unit does not have that feature enabled, skip it
        //        return;
        //    }

        //    selectedObjects.Add(selectableObject);
        //    selectableObject.IsSelected = true;

        //    if (onSelectionChanged)
        //    {
        //        onSelectionChanged.Raise(new SelectionChangedEventArgs(selectedObjects));
        //    }
        //    //else
        //    //{
        //    //    Debug.LogWarning("Selection has changed, but no event was assigned to raise");
        //    //}
        //}

        public void ClearSelection()
        {
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                var selectedUnit = (ISelectable)selectedObjects[i];
                selectedUnit.Deselect();
            }

            selectedObjects.Clear();

            if (onSelectionChanged != null)
            {
                onSelectionChanged.Raise(new SelectionChangedEventArgs());
            }
        }

        public void Select(ISelectable selectable, bool isMultiSelect = false)
        {
            if (isMultiSelect)
            {
                // TODO
            }
            else
            {
                ClearSelection();
                selectedObjects.Add(selectable);
                selectable.Select();

                if (onSelectionChanged != null)
                {
                    onSelectionChanged.Raise(new SelectionChangedEventArgs(selectedObjects));
                }
            }
            
        }
    }
}