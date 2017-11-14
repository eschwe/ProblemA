using System;
using System.Collections.Generic;
using System.Text;

namespace ModelService
{
    interface IModelService
    {
        DifferenceTree Compare(object object1, object object2, Delegate comparer);
    }
}
