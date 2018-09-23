using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    abstract class BmpFilterDecorator : IBitmapFilter, ICloneable
    {
        private string name = "<unnamed_filter>";
        protected string compoundName = null;

        private IBitmapFilter component = null;
        public string Name
        {
            get
            {
                // If a friendly name was given, we return it
                string retName = (compoundName != null ? compoundName : name);

                if ( component != null )
                    retName += " of " + component.Name;

                return retName;
            }

            set
            {
                name = value;
            }
        }

        public BmpFilterDecorator(string _name)
        {
            name = _name;
        }

        public void Decorate(IBitmapFilter filter)
        {
            component = filter;
        }

        public void Decorate(IBitmapFilter filter, string _compoundName)
        {
            Decorate(filter);
            compoundName = _compoundName;
        }

        public static BmpFilterDecorator Chain(BmpFilterDecorator decorator, IBitmapFilter component, string _compoundName = null )
        {
            BmpFilterDecorator inst = (BmpFilterDecorator) decorator.Clone();
            inst.compoundName = _compoundName;
            return inst;
        }

        public abstract Bitmap Apply(Bitmap bmp);

        public abstract object Clone();
    }
}
