//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
//
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/Cognitive-Face-Windows
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;

using Microsoft.ProjectOxford.Face.Controls;

using SampleUserControlLibrary;

namespace Microsoft.ProjectOxford.Face
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ServicePointManager.DefaultConnectionLimit = 1000;

            // You can use the next line to insert your own subscription key, instead of using UI to set license key.
            this.ViewModel = new MainViewModel()
            {
                FaceIdentificationDescription = "Tell whom an input face belongs to given a tagged person database. Here we only handle tagged person database in following format: 1). One root folder. 2). Sub-folders are named as person's name. 3). Each person's images are put into their own sub-folder. Pick the root folder, then choose an image to identify, all faces will be shown on the image with the identified person's name.",
            };
            this.DataContext = this.ViewModel;
            this._scenariosControl.SampleScenarioList = new Scenario[]
            {
                new Scenario()
                {
                    PageClass = typeof(FaceIdentificationPage),
                    Title = "Face Training",
                }
            };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets view model instance for MainWindow
        /// </summary>
        public MainViewModel ViewModel
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Log message in main window log pane
        /// </summary>
        /// <param name="format">format string</param>
        /// <param name="args">format arguments</param>
        public static void Log(string format, params object[] args)
        {
            ((MainWindow)Application.Current.MainWindow)._scenariosControl.Log(string.Format(format, args));
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// View model for MainWindow, covers display image, text
        /// </summary>
        public class MainViewModel : INotifyPropertyChanged
        {
            #region Events

            /// <summary>
            /// Implements INotifyPropertyChanged interface
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion Events

            #region Properties

            /// <summary>
            /// Gets or sets description of face detection
            /// </summary>
            public string FaceDetectionDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of face verification
            /// </summary>
            public string FaceVerificationDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of face to face verification
            /// </summary>
            public string FaceFaceVerificationDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of face to person verification
            /// </summary>
            public string FacePersonVerificationDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of face grouping 
            /// </summary>
            public string FaceGroupingDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of find similar face
            /// </summary>
            public string FaceFindSimilarDescription
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets description of identification 
            /// </summary>
            public string FaceIdentificationDescription
            {
                get;
                set;
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Helper function for INotifyPropertyChanged interface 
            /// </summary>
            /// <typeparam name="T">Property type</typeparam>
            /// <param name="caller">Property name</param>
            private void OnPropertyChanged<T>([CallerMemberName]string caller = null)
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(caller));
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}