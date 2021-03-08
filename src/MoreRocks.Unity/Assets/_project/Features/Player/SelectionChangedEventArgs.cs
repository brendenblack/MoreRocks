using Assets._project.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Features.Player
{
    public class SelectionChangedEventArgs
    {
        public SelectionChangedEventArgs()
        {
            SelectedObjects = new List<ISelectable>();
        }

        public SelectionChangedEventArgs(List<ISelectable> selectedObjects)
        {
            SelectedObjects = selectedObjects;
        }

        public List<ISelectable> SelectedObjects { get; }
    }
}
