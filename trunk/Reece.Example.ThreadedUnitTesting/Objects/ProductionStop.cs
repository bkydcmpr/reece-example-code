using System;

namespace Reece.Example.ThreadedUnitTesting.Objects
{
    public class ProductionStop
    {
        public object StateData { get; set; }
        public Guid NextNode { get; set; }
    }
}