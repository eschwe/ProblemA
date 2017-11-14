using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace ModelService
{
    public class ModelServiceProvider : IModelService
    {
        public StringBuilder PrintObject(object obj)
        {
            object sb = new StringBuilder();
            ObjectModelNode root = new ObjectModelNode("(root)", null, null);
            ObjectModelNode.BuildObjectModel(obj, root);
            ObjectModelNode.Traverse(root, null, PrintObjectProcessor, ref sb, -1);
            return (StringBuilder)sb;
        }

        public List<Difference> GetDifferingProperties(object object1, object object2)
        {
            object diffList = new List<Difference>();
            ObjectModelNode root1 = new ObjectModelNode("(root)", null, null);
            ObjectModelNode.BuildObjectModel(object1, root1);
            ObjectModelNode root2 = new ObjectModelNode("(root)", null, null);
            ObjectModelNode.BuildObjectModel(object2, root2);
            ObjectModelNode.Traverse(root1, root2, GetDiffereringPropertiesProcessor, ref diffList, -1);
            return (List<Difference>) diffList;
        }

        public List<string> GetEqualProperties(object object1, object object2)
        {
            object equalList = new List<string>();
            ObjectModelNode root1 = new ObjectModelNode("(root)", null, null);
            ObjectModelNode.BuildObjectModel(object1, root1);
            ObjectModelNode root2 = new ObjectModelNode("(root)", null, null);
            ObjectModelNode.BuildObjectModel(object1, root2);
            ObjectModelNode.Traverse(root1, root2, GetEqualPropertiesProcessor, ref equalList, -1);
            return (List<string>)equalList;
        }

        public string ComputeHash(object obj, HashAlgorithm hashAlgorithm)
        {
            object objectBytes = new byte[0];
            ObjectModelNode root = new ObjectModelNode("(root)", null, null);
            ObjectModelNode.BuildObjectModel(obj, root);
            ObjectModelNode.Traverse(root, null, ComputeHashProcessor, ref objectBytes, -1);
            return Convert.ToBase64String(hashAlgorithm.ComputeHash((byte[])objectBytes));
        }

        public static void PrintObjectProcessor(ObjectModelNode node1, ObjectModelNode node2, ref object accumulator, int level)
        {
            StringBuilder sb = (StringBuilder)accumulator;
            string s = "";

            for (int i = 0; i < level; i++)
            {
                s += "  ";
            }

            s += string.Format("{0} : {1}\n", node1.FullPropertyName, node1.PropertyValue);

            sb.Append(s);
        }

        public static void GetDiffereringPropertiesProcessor(ObjectModelNode node1, ObjectModelNode node2, ref object accumulator, int level)
        {
            List<Difference> diffs = (List<Difference>)accumulator;

            if (node1 == null)
            {
                diffs.Add(new Difference(node2.FullPropertyName, "[absent]", (node2.PropertyValue != null ? node2.PropertyValue : "null")));
                return;
            }
            if (node2 == null)
            {
                diffs.Add(new Difference(node1.FullPropertyName, (node1.PropertyValue != null ? node1.PropertyValue : "null"), "[absent]"));
                return;
            }
            if (node1.PropertyValue is IEnumerable)
            {
                return;
            }

            if (node1.PropertyValue == null)
            {
                if (node2.PropertyValue == null)
                {
                    return;
                }
                diffs.Add(new Difference(node2.FullPropertyName, "null", node2.PropertyValue));
                return;
            }
            if (node2.PropertyValue == null)
            {
                diffs.Add(new Difference(node1.FullPropertyName, node1.PropertyValue, "null"));
                return;
            }

            DataContractSerializer dcs = new DataContractSerializer(node1.PropertyType);
            byte[] bytes1 = null;
            byte[] bytes2 = null;


            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    dcs.WriteObject(ms, node1.PropertyValue);
                    bytes1 = ms.ToArray();
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    dcs.WriteObject(ms, node2.PropertyValue);
                    bytes2 = ms.ToArray();
                }
            }
            catch (SerializationException)
            {
                return;
            }

            if (!bytes1.SequenceEqual(bytes2))
            {
                diffs.Add(new Difference(node1.FullPropertyName, node1.PropertyValue, node2.PropertyValue));
            }
        }

        public static void GetEqualPropertiesProcessor(ObjectModelNode node1, ObjectModelNode node2, ref object accumulator, int level)
        {
            List<string> equals = (List<string>)accumulator;

            if (node1 == null)
            {
                if (node2 == null)
                {
                    equals.Add(node1.FullPropertyName);
                }
                return;
            }
            if (node2 == null)
            {
                return;
            }

            if (node1.PropertyValue == null)
            {
                if (node2.PropertyValue == null)
                {
                    equals.Add(node1.FullPropertyName);
                    return;
                }
                return;
            }
            if (node2.PropertyValue == null)
            {
                return;
            }

            DataContractSerializer dcs = new DataContractSerializer(node1.PropertyType);
            byte[] bytes1 = null;
            byte[] bytes2 = null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    dcs.WriteObject(ms, node1.PropertyValue);
                    bytes1 = ms.ToArray();
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    dcs.WriteObject(ms, node2.PropertyValue);
                    bytes2 = ms.ToArray();
                }
            }
            catch (SerializationException)
            {
                return;
            }

            if (bytes1.SequenceEqual(bytes2))
            {
                equals.Add(node1.FullPropertyName);
            }
        }

        public static void ComputeHashProcessor(ObjectModelNode node1, ObjectModelNode node2, ref object accumulator, int level)
        {
            byte[] bytes = (byte[])accumulator;

            if (node1.PropertyType == null) { return; }

            DataContractSerializer dcs = new DataContractSerializer(node1.PropertyType);
            byte[] objectBytes = null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    dcs.WriteObject(ms, node1.PropertyValue);
                    objectBytes = ms.ToArray();
                }
            }
            catch (SerializationException)
            {
                return;
            }

            byte[] newBytes = new byte[bytes.Length + objectBytes.Length];

            bytes.CopyTo(newBytes, 0);
            objectBytes.CopyTo(newBytes, bytes.Length);

            accumulator = (object)newBytes;
        }
    }
}
