// CONTACT
// Joe Healy // josephehealy@hotmail.com
//
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associateddocumentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY
//
using Microsoft.Kinect;
using System;
using System.Collections.Generic;


namespace FaceNSkinWPF
{
    public static class Bones
    {
        private static List<Tuple<JointType, JointType>> m_bones;
        public static List<Tuple<JointType, JointType>> Collection
        {
            get { return m_bones;  }
        }
        static Bones()
        {
            // a bone defined as a line between two joints
            m_bones = new List<Tuple<JointType, JointType>>();

            // Torso
            m_bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            m_bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            m_bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            m_bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            m_bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            m_bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));
        }
    }
}
