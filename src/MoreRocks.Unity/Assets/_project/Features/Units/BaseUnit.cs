using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets._project.Features.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class BaseUnit : MonoBehaviour, ISelectable
    {
        [SerializeField] protected bool isSelected;
        [SerializeField] protected bool isMultiSelectEnabled = false;
        [SerializeField] protected GameObject highlight;
        [SerializeField] protected string displayName;

        public string DisplayName => displayName;

        protected NavMeshAgent navMeshAgent;
        protected virtual void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Perform the default interaction for the provided target.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="target"></param>
        public virtual void Interact(GameObject target)
        {
            var destination = GetClosestPoint(target);
            Move(destination);
        }

        public Vector3 GetClosestPoint(GameObject target)
        {
            if (target.TryGetComponent(out Collider collider))
            {
                var destination = collider.ClosestPoint(this.transform.position);
                return destination;
            }
            else
            {
                Debug.LogWarning("Provided game object does not have a collider attached, using transform location", this);
                return target.transform.position;
            }
        }

        /// <summary>
        /// Move this unit to the provided location 
        /// </summary>
        /// <param name="location"></param>
        public virtual void Move(Vector3 location)
        {
            navMeshAgent.destination = location;
        }

        /// <summary>
        /// Move this unit to the provided location
        /// </summary>
        /// <param name="location"></param>
        public virtual void GuardMove(Vector3 location)
        {
            // not implemented
            Move(location);
        }

        public virtual void Idle()
        {

        }

        #region ISelectable
        public bool IsMultiSelectEnabled => isMultiSelectEnabled;

        public bool IsSelected => isSelected;

        public void Deselect()
        {
            isSelected = false;
            TurnOffSelector();
        }

        public void Select()
        {
            isSelected = true;
            TurnOnSelector();
        }

        public void TurnOffSelector()
        {
            if (highlight)
            {
                highlight.SetActive(false);
            }
            else
            {
                Debug.LogWarning("No highlight item has been assigned to this object", this);
            }
        }

        //Turns on the sprite renderer
        public void TurnOnSelector()
        {
            if (highlight)
            {
                highlight.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No highlight item has been assigned to this object", this);
            }
        }

        #endregion
    }
}
