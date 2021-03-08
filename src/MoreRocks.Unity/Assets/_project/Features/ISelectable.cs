using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._project.Features
{
    public interface ISelectable
    {
        GameObject gameObject { get; }

        void Select();

        void Deselect();

        bool IsMultiSelectEnabled { get; }

        bool IsSelected { get; }
    }
}
