namespace ModelService
{
    public class Difference
    {
        public string PropertyName { get; set; }
        public object Object1 { get; set; }
        public object Object2 { get; set; }

        public Difference(string propertyName, object object1, object object2)
        {
            PropertyName = propertyName;
            Object1 = object1;
            Object2 = object2;
        }
    }
}
