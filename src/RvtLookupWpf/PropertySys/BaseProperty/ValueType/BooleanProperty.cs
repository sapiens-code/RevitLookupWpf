﻿namespace RvtLookupWpf.PropertySys
{
    public class BooleanProperty : PropertyBase<bool>
    {
        public BooleanProperty(string name, bool value) : base(name)
        {
            Value = value;
        }
    }
}
