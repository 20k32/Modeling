using Modeling.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modeling.Core.Extensions
{
    public static class ObjectTreeExtensions
    {
        public static IEnumerable<IObjectTree> TraverseFromParent(this IObjectTree parent)
        {
            if (parent is null)
            {
                return Enumerable.Empty<IObjectTree>();
            }

            var visitedNodes = new HashSet<IObjectTree>();
            var familyStack = new Stack<IObjectTree>();

            while (parent is not null && !visitedNodes.Contains(parent))
            {
                familyStack.Push(parent);

                parent = parent.Parent;
            }

            return familyStack;
        }

        public static IEnumerable<IObjectTree> TraverseToParent(this IObjectTree child)
        {
            var visitedNodes = new HashSet<IObjectTree>();

            while (child is not null && !visitedNodes.Contains(child))
            {
                yield return child;

                child = child.Parent;
            }
        }
    }
}
