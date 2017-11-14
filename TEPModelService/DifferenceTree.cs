using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModelService
{
    class DifferenceTree
    {
        public DifferenceNode RootNode;
        public object Object1;
        public object Object2;
        private CompareProperty Compare;

        public DifferenceTree(object object1, object object2, CompareProperty compare)
        {
            Object1 = object1;
            Object2 = object2;
            Compare = compare;
        }

        public void Run()
        {
            Type t1, t2;
            PropertyInfo[] pinfo1, pinfo2;

            if (Object1 == null)
            {
                throw new ArgumentNullException("Object1 must not be null");
            }

            typeof(string).GetPro
            t1 = Object1.GetType();
            pinfo1 = t1.GetProperty

            TraverseObjectModel(Object1, Object2, compare)
        }

        private TraverseObjectModel()

    }

    delegate bool CompareProperty(object object1, object object2);
}
