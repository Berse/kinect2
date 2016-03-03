// CONTACT
// Joe Healy // josephehealy@hotmail.com
//
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associateddocumentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY
//
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
