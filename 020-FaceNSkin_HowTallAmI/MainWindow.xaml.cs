// CONTACT
// Joe Healy // josephehealy@hotmail.com
//
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associateddocumentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. 
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY
//
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;
using System.ComponentModel;

namespace FaceNSkinWPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private KinectSensor m_kinectSensor = null;
        private BodyFrameReader m_bodyReader;
        private Body[] m_bodies = null;
        private DrawingGroup m_drawingGroup;
        private DrawingImage m_imageSource;
        int m_DisplayWidth, m_DisplayHeight;
        private CoordinateMapper m_coordinateMapper = null;

        public MainWindow()
        {
            this.m_drawingGroup = new DrawingGroup();
            this.m_imageSource = new DrawingImage(this.m_drawingGroup);

            // initialize kinect
            this.m_kinectSensor = KinectSensor.GetDefault();
            this.m_coordinateMapper = this.m_kinectSensor.CoordinateMapper;

            this.m_kinectSensor.IsAvailableChanged += KinectSensor_IsAvailableChanged;
            this.m_kinectSensor.Open();

            // hook the body stuff
            this.m_bodyReader = m_kinectSensor.BodyFrameSource.OpenReader();
            FrameDescription _frameDescription = this.m_kinectSensor.DepthFrameSource.FrameDescription;
            this.m_DisplayWidth = _frameDescription.Width;
            this.m_DisplayHeight = _frameDescription.Height;

            this.m_bodyReader.FrameArrived += BodyReader_FrameArrived;

            this.DataContext = this;
            InitializeComponent();
            Msg("MainWindow c'tor completed");
        }

        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame _bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (_bodyFrame != null)
                {
                    if (this.m_bodies == null)
                    {
                        this.m_bodies = new Body[_bodyFrame.BodyCount];
                    }
                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    _bodyFrame.GetAndRefreshBodyData(this.m_bodies);
                    Msg(String.Format("BodyReader_FrameArrived: {0}", e.FrameReference.RelativeTime.ToString()));
                    BodyDrawHelper.DrawSkeletalBodies(m_drawingGroup, m_bodies, this.m_DisplayHeight, this.m_DisplayWidth, m_coordinateMapper);
                    ShowBodyHeights();
                } // if
            } // using
        }

        private void KinectSensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine( "KinectSensor_IsAvailableChanged: e.IsAvailable=" + e.IsAvailable.ToString() );
            TextBlock_StatusBar.Text = "e.IsAvailable: " + e.IsAvailable.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // nothing
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.m_bodyReader != null)
            {
                // BodyFrameReader is IDisposable
                this.m_bodyReader.Dispose();
                this.m_bodyReader = null;
            }

            if (this.m_kinectSensor != null)
            {
                this.m_kinectSensor.Close();
                this.m_kinectSensor = null;
            }
        }

        private void Msg ( string s )
        {
            if (s.Length <= 0) s = "ready";
            TextBlock_Msg.Text = s;
        }

        private void Button_KStatus_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder _sb = new StringBuilder(1024);
            _sb.AppendFormat("raw: {0}", System.DateTime.Now.ToString("hh:mm:ss"));
            _sb.Append(" -- ");
            _sb.AppendFormat("UniqueKinectId: {0}", m_kinectSensor.UniqueKinectId);
            _sb.Append(" -- ");
            _sb.AppendFormat("IsOpen: {0}", m_kinectSensor.IsOpen);
            _sb.Append(" -- ");
            int _bcount = -1;
            // we should check for null but being lazy here in the interest of time
            try
            {
                _bcount = (from b in m_bodies where b.IsTracked == true select b).Count();
                _sb.AppendFormat("tracked m_bodies.Count: {0}", _bcount);
            }
            catch ( Exception ex )
            {
                _sb.Append("tracked m_bodies.Count: NONE" );
            }

            TextBlock_RawStatus.Text = _sb.ToString();
        }
        private void ShowBodyHeights()
        {
            double _h0 = m_bodies[0].Height();
            double _h1 = m_bodies[1].Height();
            double _h2 = m_bodies[2].Height();
            double _h3 = m_bodies[3].Height();
            double _h4 = m_bodies[4].Height();
            double _h5 = m_bodies[5].Height();

            // switch between meters and feet for display
            // BodyHeightExtension.MeasurementSystem = BodyHeightMeasurementSystem.Meters;
            BodyHeightExtension.MeasurementSystem = BodyHeightMeasurementSystem.Imperial;

            string _retval;
            if ( BodyHeightExtension.MeasurementSystem == BodyHeightMeasurementSystem.Meters )
            { 
                _retval = String.Format("body heights: #0[{0:0.0#}m] #1[{1:0.0#}m] #2[{2:0.0#}m] #3[{3:0.0#}m] #4[{4:0.0#}m] #5[{5:0.0#}m]",
                    _h0, _h1, _h2, _h3, _h4, _h5 );
            }
            else
            {
                _retval = String.Format("body heights: #0[{0:0.0#}f] #1[{1:0.0#}f] #2[{2:0.0#}f] #3[{3:0.0#}f] #4[{4:0.0f}f] #5[{5:0.0#}f]",
                    _h0, _h1, _h2, _h3, _h4, _h5);
            }
            TextBlock_BodyHeights.Text = _retval;
        }
        public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets the bitmap to display
            /// </summary>
            public ImageSource ImageSource {get { return this.m_imageSource; }}
    }
}
