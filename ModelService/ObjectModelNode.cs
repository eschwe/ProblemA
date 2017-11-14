using System;
using System.Collections;
using System.Reflection;

namespace ModelService
{
    public class ObjectModelNode
    {
        public string PropertyName { get; set; }
        public string FullPropertyName
        {
            get
            {
                string fullName = "";
                ObjectModelNode node = this;
                do
                {
                    fullName = node.PropertyName + "." + fullName;
                    node = node.Parent;
                }
                while (node != null);
                return fullName.TrimEnd('.');
            }
        }
        public object PropertyValue { get; set; }
        public Type PropertyType { get; set; }
        public ObjectModelNode NextSibling { get; set; }
        public ObjectModelNode FirstChild { get; set; }
        public ObjectModelNode Parent { get; set; }
        private const int MAX_RECURSION_LEVEL = 64;

        public ObjectModelNode() { }

        public ObjectModelNode(string propertyName, object propertyValue, Type propertyType)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            PropertyType = propertyType;
        }

        public static ObjectModelNode BuildObjectModel(object o, ObjectModelNode parent, int level = -1)
        {
            level++;
            if (level == MAX_RECURSION_LEVEL)
            {
                throw new Exception("Reached MAX_RECURSION_LEVEL");
            }

            ObjectModelNode node = new ObjectModelNode();
            node.Parent = parent;
            Type type = o.GetType();
            node.PropertyType = type;

            IEnumerable ie = o as IEnumerable;
            if (ie != null)
            {
                node.PropertyName = "(enumerator)";
                IEnumerator enumerator = (IEnumerator)o.GetType().GetMethod("GetEnumerator", new Type[0]).Invoke(o, null);
                Enumerate(enumerator, node, level);
                AppendSibling(parent, node);
                node.Parent = parent;
            }

            PropertyInfo[] pinfos = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pinfo in pinfos)
            {
                if (pinfo.GetIndexParameters().Length != 0)
                {
                    continue;
                }

                string propertyName = pinfo.Name;
                object obj = pinfo.GetValue(o);
                type = pinfo.PropertyType;

                node = new ObjectModelNode(propertyName, obj, type);
                node.Parent = parent;

                if (!(obj is ValueType))
                {
                    BuildObjectModel(obj, node, level);
                }

                AppendSibling(parent, node);
            }

            level--;
            return node;
        }

        private static void Enumerate(IEnumerator enumerator, ObjectModelNode parent, int level)
        {
            ObjectModelNode itemNode = null;
            int count = 0;

            while (enumerator.MoveNext())
            {
                object item = enumerator.Current;
                string itemname = string.Format("(enumerator)[{0}]", count);

                if (item is ValueType)
                {
                    itemNode = new ObjectModelNode(itemname, item, item.GetType());
                    itemNode.Parent = parent;
                    AppendSibling(parent, itemNode);
                }
                else if (item != null && item is IEnumerable)
                {
                    itemNode = new ObjectModelNode(itemname, item, item.GetType());
                    IEnumerator ie = (IEnumerator)item.GetType().GetMethod("GetEnumerator", new Type[0]).Invoke(item, null);
                    Enumerate(ie, itemNode, level);
                    itemNode.Parent = parent;
                    AppendSibling(parent, itemNode);
                }
                else
                {
                    AppendSibling(parent, (item != null ? itemNode = BuildObjectModel(item, parent, level) : new ObjectModelNode(itemname, null, null)));
                }
                count++;
            }
        }

        private static void AppendSibling(ObjectModelNode parent, ObjectModelNode child)
        {
            ObjectModelNode curNode = null;

            if (parent.FirstChild != null)
            {
                curNode = parent.FirstChild;
                while (curNode.NextSibling != null)
                {
                    curNode = curNode.NextSibling;
                }
                curNode.NextSibling = child;
            }
            else
            {
                parent.FirstChild = child;
            }
        }

        public static void Traverse(ObjectModelNode root1, ObjectModelNode root2, ObjectModelProcessor processor, ref object accumulator, int level)
        {
            level++;

            while (root1 != null || root2 != null)
            {
                processor(root1, root2, ref accumulator, level);

                if ((root1 != null && root1.FirstChild != null) ||
                    (root2 != null && root2.FirstChild != null))
                {
                    Traverse((root1 != null ? root1.FirstChild : null), (root2 != null ? root2.FirstChild : null), processor, ref accumulator, level);
                }

                if (root1 != null || root2 != null)
                {
                    root1 = (root1 != null ? root1.NextSibling : null);
                    root2 = (root2 != null ? root2.NextSibling : null);
                }
            }

            level--;
        }
    }

    public delegate void ObjectModelProcessor(ObjectModelNode node1, ObjectModelNode node2, ref object accumulator, int level);
}
