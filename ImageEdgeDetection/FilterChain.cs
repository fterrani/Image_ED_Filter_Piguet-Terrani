using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageEDFilter
{
    // This class defines a filter consisting in a set of filters applied sequentially
    class FilterChain : IBitmapFilter
    {
        private string name = null;
        private List<IBitmapFilter> filters = new List<IBitmapFilter>();

        public string Name
        {
            get
            {
                // Returning the name of the filter if one was defined
                if (name != null)
                    return name;

                string compoundName = "";
                string sep = " of ";

                // If no name was defined, we compose the filter's name from the composing filters' names
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

        // When converted to a string, the name of the filter is returned
        public override string ToString()
        {
            return Name;
        }
    }
}
