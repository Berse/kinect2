using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;
using System.ComponentModel;

namespace FaceNSkinWPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// Constant for clamping Z values of camera space points from being negative
        private const float InferredZPositionClamp = 0.1f;

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
            bool _datareceived = false;

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
                    _datareceived = true;
                } // if
                if (_datareceived) DrawSkeletalBodies();
            } // using
        }

        private void KinectSensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine( "KinectSensor_IsAvailableChanged: e.IsAvailable=" + e.IsAvailable.ToString() );
            TextBlock_StatusBar.Text = "e.IsAvailable: " + e.IsAvailable.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

        private void DrawSkeletalBodies()
        {
                using (DrawingContext _dc = this.m_drawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    _dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.m_DisplayWidth, this.m_DisplayHeight));

                    int penIndex = 0;
                    foreach (Body _body in this.m_bodies)
                    {
                        Pen drawPen = BodyColors.GetColorAt(penIndex++);
                        if (_body.IsTracked)
                        {
                            BodyDrawHelper.DrawClippedEdges(_body, _dc, this.m_DisplayHeight, this.m_DisplayWidth );

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

                                DepthSpacePoint depthSpacePoint = this.m_coordinateMapper.MapCameraPointToDepthSpace(position);
                                _jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            }

                            BodyDrawHelper.DrawBody(joints, _jointPoints, _dc, drawPen);

                            BodyDrawHelper.DrawHand(_body.HandLeftState, _jointPoints[JointType.HandLeft], _dc);
                            BodyDrawHelper.DrawHand(_body.HandRightState, _jointPoints[JointType.HandRight], _dc);
                        }
                    }

                    // prevent drawing outside of our render area
                    this.m_drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, m_DisplayWidth, m_DisplayHeight));
                }
            } // DrawBodies function

            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets the bitmap to display
            /// </summary>
            public ImageSource ImageSource {get { return this.m_imageSource; }}
    }
}
