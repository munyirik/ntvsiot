﻿//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************//

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualStudio.PlatformUI;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Interaction logic for WaitForCompleteAnalysisDialog.xaml
    /// </summary>
    partial class TaskProgressBar : DialogWindowVersioningWorkaround {
        private readonly Task _task;
        private readonly DispatcherTimer _timer;
        private readonly CancellationTokenSource _cancelSource;

        public TaskProgressBar(Task task, CancellationTokenSource cancelSource, string message) {
            _task = task;
            InitializeComponent();
            _waitLabel.Text = message;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
            _timer.Tick += TimerTick;
            _cancelSource = cancelSource;
        }

        void TimerTick(object sender, EventArgs e) {
            _progress.Value = (_progress.Value + 1) % 100;
        }

        protected override void OnInitialized(System.EventArgs e) {
            // when the task completes we post back onto our UI thread to close the dialog box.
            // Capture the UI scheduler, and setup a continuation to do the close.
            var curScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _task.ContinueWith(new CloseDialog(curScheduler, this).Close);

            base.OnInitialized(e);
        }

        class CloseDialog {
            private readonly TaskScheduler _ui;
            private readonly TaskProgressBar _progressBar;

            public CloseDialog(TaskScheduler uiScheduler, TaskProgressBar progressBar) {
                _ui = uiScheduler;
                _progressBar = progressBar;
            }

            public void Close(Task task) {
                var newTask = new Task(CloseWorker);
                newTask.Start(_ui);
                newTask.Wait();
            }

            private void CloseWorker() {
                _progressBar.DialogResult = true;
                _progressBar.Close();
            }
        }

        private void _cancelButton_Click(object sender, RoutedEventArgs e) {
            _cancelSource.Cancel();
            this.DialogResult = false;
            this.Close();
        }
    }
}