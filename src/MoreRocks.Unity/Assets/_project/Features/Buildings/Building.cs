using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._project.Features.Buildings
{
    public class Building : MonoBehaviour, ISelectable
    {
        [SerializeField] protected bool isSelected;
        [SerializeField] protected bool isMultiSelectEnabled = false;
        [SerializeField] protected GameObject highlight;
        
        public bool IsMultiSelectEnabled => isMultiSelectEnabled;

        public bool IsSelected => isSelected;

        public void Deselect()
        {
            isSelected = false;
        }

        public void Select()
        {
            isSelected = true;
        }
    }
}
