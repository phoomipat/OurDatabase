using System.Collections.Generic;

namespace OurDatabase
{
    public class GenericNode<T, TA>
    {
        public int NodePosition { get; set; }
        public int LeftPosition { get; set; }
        public int RightPosition { get; set; }
        public T Key { get; set; }
        public TA Value { get; set; }
        
        public GenericNode(T key, TA value, int nodePosition)
        {
            NodePosition = nodePosition;
            LeftPosition = -1;
            RightPosition = -1;
            Key = key;
            Value = value;
        }
        
        public GenericNode(T key, TA value, int nodePosition, int leftPosition, int rightPosition)
        {
            NodePosition = nodePosition;
            LeftPosition = leftPosition;
            RightPosition = rightPosition;
            Key = key;
            Value = value;
        }
        
        public GenericNode(int nodePosition, int leftPosition, int rightPosition, T key, TA value)
        {
            NodePosition = nodePosition;
            LeftPosition = leftPosition;
            RightPosition = rightPosition;
            Key = key;
            Value = value;
        }
    }
}