using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ModelService
{
    public interface IModelService
    {
        List<Difference> GetDifferingProperties(object object1, object object2);
        List<string> GetEqualProperties(object object1, object object2);
        string ComputeHash(object obj, HashAlgorithm hashAlgorithm);

        StringBuilder PrintObject(object obj);
    }
}
