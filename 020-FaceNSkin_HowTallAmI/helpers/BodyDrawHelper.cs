// CONTACT
// Joe Healy // josephehealy@hotmail.com
//
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associateddocumentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY
//
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace FaceNSkinWPF
{
    public static class BodyDrawHelper
    {
        public static double ClipBoundsThickness = 10;
        public static double HandSize = 30;
        public static Pen InferredBonePen = new Pen(Brushes.Gray, 1);
        /// Constant for clamping Z values of camera space points from being negative
        public static float InferredZPositionClamp = 0.1f;
        public static double JointThickness = 3;

        /// Brush used for drawing hands that are currently tracked as closed
        public static Brush HandClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
        /// Brush used for drawing hands that are currently tracked as opened
        public static Brush HandOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        public static Brush HandLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));
        /// Brush used for drawing joints that are currently tracked
        public static Brush TrackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        /// Brush used for drawing joints that are currently inferred       
        public static Brush InferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        public static void DrawClippedEdges(Body body, DrawingContext drawingContext, int displayHeight, int displayWidth )
        {
            FrameEdges _clippedEdges = body.ClippedEdges;

            if (_clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, displayHeight - ClipBoundsThickness, displayWidth, ClipBoundsThickness));
            }

            if (_clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, displayWidth, ClipBoundsThickness));
            }

            if (_clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, displayHeight));
            }

            if (_clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, displayHeight));
            }
        } // DrawClippedEdges

          /// <summary>
          /// Draws a body
          /// </summary>
          /// <param name="joints">joints to draw</param>
          /// <param name="jointPoints">translated positions of joints to draw</param>
          /// <param name="drawingContext">drawing context to draw to</param>
          /// <param name="drawingPen">specifies color to draw a specific body</param>
        public static void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, 
            IDictionary<JointType, Point> jointPoints, 
            DrawingContext drawingContext, Pen drawingPen )
        {
            // Draw the bones
            foreach (var bone in Bones.Collection)
            {
                DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = TrackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = InferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            } // FOREACH
        } // DRAWBODY

          /// <summary>
          /// Draws one bone of a body (joint to joint)
          /// </summary>
          /// <param name="joints">joints to draw</param>
          /// <param name="jointPoints">translated positions of joints to draw</param>
          /// <param name="jointType0">first joint of bone to draw</param>
          /// <param name="jointType1">second joint of bone to draw</param>
          /// <param name="drawingContext">drawing context to draw to</param>
          /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private static void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint _joint0 = joints[jointType0];
            Joint _joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (_joint0.TrackingState == TrackingState.NotTracked ||
                _joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = InferredBonePen;
            if ((_joint0.TrackingState == TrackingState.Tracked) && (_joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }//drawbone

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        public static void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse( HandClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse( HandOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse( HandLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            } //switch
        }//drawhand

        // could have a DrawSkeletalBodiesParamsStruct to bundle these
        public static void DrawSkeletalBodies( 
            DrawingGroup dg , Body[] bodies, int DisplayWidth, int DisplayHeight, CoordinateMapper coordmapper )
        {
            using (DrawingContext _dc = dg.Open())
            {
                // Draw a transparent background to set the render size
                _dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, DisplayWidth, DisplayHeight));

                int penIndex = 0;
                foreach (Body _body in bodies)
                {
                    Pen drawPen = BodyColors.GetColorAt(penIndex++);
                    if (_body.IsTracked)
                    {
                        BodyDrawHelper.DrawClippedEdges(_body, _dc, DisplayHeight, DisplayWidth);

                        IReadOnlyDictionary<JointType, Joint> joints = _body.Joints;

                        // convert the joint points to depth (display) space
                        Dictionary<JointType, Point> _jointPoints = new Dictionary<JointType, Point>();

                        foreach (JointType jointType in joints.Keys)
                        {
                            // sometimes the depth(Z) of an inferred joint may show as negative
                            // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                            CameraSpacePoint position = joints[jointType].Position;
                            if (position.Z < 0)
                            {
                                position.Z = InferredZPositionClamp;
                            }

                            DepthSpacePoint depthSpacePoint = coordmapper.MapCameraPointToDepthSpace(position);
                            _jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                        }

                        BodyDrawHelper.DrawBody(joints, _jointPoints, _dc, drawPen);

                        BodyDrawHelper.DrawHand(_body.HandLeftState, _jointPoints[JointType.HandLeft], _dc);
                        BodyDrawHelper.DrawHand(_body.HandRightState, _jointPoints[JointType.HandRight], _dc);
                    }
                }

                // prevent drawing outside of our render area
                dg.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, DisplayWidth, DisplayHeight));
            }
        } // DrawBodies function

    }
}
