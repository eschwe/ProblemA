using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModelService
{
    class DifferenceNode
    {
        protected string PropertyName { get; }
        protected Tuple<object, object> PropertyValues { get; }
        DifferenceNode NextSibling { get; set; }
        DifferenceNode FirstChild { get; set; }

        // Disallow default construction
        private DifferenceNode() { }

        // Allow parameterized construction of nodes, but only by classes in this assembly
        // Value types will be boxed into objects, which might be a performance drain
        protected DifferenceNode(string propertyName, object object1, object object2)
        {
            PropertyName = propertyName;
            PropertyValues = new Tuple<object, object>(object1, object2);
        }

        public bool Compare(Delegate comparer)
        {
            // Since nothing was checked in the constructor:
            // PropertyName must not be null or empty
            // Both or either of the pair of values may be null, but if they are not they
            // must be of the same type.
            if (string.IsNullOrWhiteSpace(PropertyName))
            {
                throw new ArgumentNullException("PropertyName must not be null or empty");
            }

            if (PropertyValues.Item1 == null && PropertyValues.Item2 == null)
            {
                return true;
            }
            else if (PropertyValues.Item1 == null || PropertyValues.Item2 == null)
            {
                return false;
            }

            if (PropertyValues.Item1.GetType() != PropertyValues.Item2.GetType())
            {
                throw new ArgumentException("The differands must be of the same type");
            }

            // Nothing is null or amiss, so we can get to business
            return (bool)comparer.DynamicInvoke(PropertyValues.Item1, PropertyValues.Item2);
        }
    }
}
