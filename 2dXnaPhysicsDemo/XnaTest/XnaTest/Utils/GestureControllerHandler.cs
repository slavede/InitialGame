using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizbin.Kinect.Gestures;
using Fizbin.Kinect.Gestures.Segments;
using System.Timers;
using System.ComponentModel;
using Microsoft.Kinect;
using XnaTest.CustomHandGestures;

namespace XnaTest.Utils
{
    /// <summary>
    /// Class will be responsible for registering gestures, recognizing them, handle events etc.
    /// It will fire event so frontend caller will now gesture happened, and what gesture
    /// </summary>
    public class GestureControllerHandler
    {
        public GestureController GestureController {get;set;}
        private Timer clearTimer;

        public GestureControllerHandler()
        {
            // initialize the gesture recognizer
            GestureController = new GestureController();
            GestureController.GestureRecognized += OnGestureRecognized;
            

            // add timer for clearing last detected gesture
            clearTimer = new Timer(2000);
            clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);

            // register the gestures for this demo
            //RegisterGestures();
        }

        /// <summary>
        /// Method registeres all available gestures for application
        /// </summary>
        private void RegisterGestures()
        {
            IRelativeGestureSegment[] joinedhandsSegments = new IRelativeGestureSegment[1];
            JoinedHandsAnywhere joinedhandsSegment = new JoinedHandsAnywhere();
            // only once is enough
            joinedhandsSegments[0] = joinedhandsSegment;

            GestureController.AddGesture("JoinedHandsAnywhere", joinedhandsSegments);

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Gesture event arguments.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            switch (e.GestureName)
            {
                case "JoinedHandsAnywhere":
                    Gesture = "Joined Hands Anywhere";
                    break;
                default:
                    break;
            }

            clearTimer.Start();
        }

        /// <summary>
        /// Event implementing INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Gets or sets the last recognized gesture.
        /// </summary>
        private string _gesture;
        public String Gesture
        {
            get { return _gesture; }
          
            set
            {
                if (_gesture == value)
                    return;

                _gesture = value;

                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
            }
        }

        /// <summary>
        /// Clear text after some time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Gesture = "";
            clearTimer.Stop();
        }

        public void UpdateAllGestures(Skeleton skeleton)
        {
            GestureController.UpdateAllGestures(skeleton);
        }
    }
}
