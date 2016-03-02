using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace FaceNSkinWPF
{
    static public class BodyColors
    {
        static private List<Pen> _penlist;
        static BodyColors()
        {
            // populate body colors, one for each BodyIndex
            _penlist = new List<Pen>();

            _penlist.Add(new Pen(Brushes.Red, 6));
            _penlist.Add(new Pen(Brushes.Orange, 6));
            _penlist.Add(new Pen(Brushes.Green, 6));
            _penlist.Add(new Pen(Brushes.Blue, 6));
            _penlist.Add(new Pen(Brushes.Indigo, 6));
            _penlist.Add(new Pen(Brushes.Violet, 6));
        }

        static public Pen GetColorAt( int ii )
        {
            if ((ii < 0) || (ii>_penlist.Count)) return null;
            return(_penlist[ii]);
        }
    }
}
