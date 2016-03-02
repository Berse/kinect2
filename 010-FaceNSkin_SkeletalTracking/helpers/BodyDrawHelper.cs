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



    }
}
