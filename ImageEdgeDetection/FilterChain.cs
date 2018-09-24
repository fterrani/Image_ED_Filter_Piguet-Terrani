using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    class FilterChain : IBitmapFilter
    {
        private string name = null;
        private List<IBitmapFilter> filters = new List<IBitmapFilter>();

        public string Name
        {
            get
            {
                if (name != null)
                    return name;

                string compoundName = "";
                string sep = " of ";

                for (int i = filters.Count - 1; i >= 0; i--)
                {
                    compoundName += filters[i].Name;
                    compoundName += (i > 0 ? sep : "");
                }

                return compoundName;
            }

            set
            {
                name = value;
            }
        }

        public List<IBitmapFilter> Filters {
            get {
                return filters;
            }
            set {
                filters = (value ?? new List<IBitmapFilter>());
            }
        }

        public FilterChain(params IBitmapFilter[] _filters)
        {
            Filters = new List<IBitmapFilter>(_filters);
        }

        public FilterChain(string _name, params IBitmapFilter[] _filters)
        {
            Filters = new List<IBitmapFilter>(_filters);
            Name = _name;
        }

        // Sequentially applies a list of filters on bmp
        public Bitmap Apply(Bitmap bmp)
        {
            foreach( IBitmapFilter f in filters)
            {
                bmp = f.Apply(bmp);
            }

            return bmp;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
