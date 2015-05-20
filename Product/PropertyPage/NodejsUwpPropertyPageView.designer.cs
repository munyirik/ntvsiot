/*
    Copyright(c) Microsoft Open Technologies, Inc. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions :

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

namespace Microsoft.NodejsUwp
{
    partial class NodejsUwpPropertyPageView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._debuggerMachineNameLabel = new System.Windows.Forms.Label();
            this._debuggerMachineName = new System.Windows.Forms.TextBox();
            this._nodeArgumentsLabel = new System.Windows.Forms.Label();
            this._nodeArguments = new System.Windows.Forms.TextBox();
            this._scriptArgumentsLabel = new System.Windows.Forms.Label();
            this._scriptArguments = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _debuggerMachineNameLabel
            // 
            this._debuggerMachineNameLabel.AutoSize = true;
            this._debuggerMachineNameLabel.Location = new System.Drawing.Point(17, 57);
            this._debuggerMachineNameLabel.Name = "_debuggerMachineNameLabel";
            this._debuggerMachineNameLabel.Size = new System.Drawing.Size(91, 13);
            this._debuggerMachineNameLabel.TabIndex = 0;
            this._debuggerMachineNameLabel.Text = "Remote Machine:";
            // 
            // _debuggerMachineName
            // 
            this._debuggerMachineName.Location = new System.Drawing.Point(111, 54);
            this._debuggerMachineName.Name = "_debuggerMachineName";
            this._debuggerMachineName.Size = new System.Drawing.Size(508, 20);
            this._debuggerMachineName.TabIndex = 1;
            // 
            // _nodeArgumentsLabel
            // 
            this._nodeArgumentsLabel.AutoSize = true;
            this._nodeArgumentsLabel.Location = new System.Drawing.Point(17, 90);
            this._nodeArgumentsLabel.Name = "_nodeArgumentsLabel";
            this._nodeArgumentsLabel.Size = new System.Drawing.Size(88, 13);
            this._nodeArgumentsLabel.TabIndex = 2;
            this._nodeArgumentsLabel.Text = "Node arguments:";
            // 
            // _nodeArguments
            // 
            this._nodeArguments.Location = new System.Drawing.Point(111, 87);
            this._nodeArguments.Name = "_nodeArguments";
            this._nodeArguments.Size = new System.Drawing.Size(508, 20);
            this._nodeArguments.TabIndex = 3;
            // 
            // _scriptArgumentsLabel
            // 
            this._scriptArgumentsLabel.AutoSize = true;
            this._scriptArgumentsLabel.Location = new System.Drawing.Point(17, 123);
            this._scriptArgumentsLabel.Name = "_scriptArgumentsLabel";
            this._scriptArgumentsLabel.Size = new System.Drawing.Size(89, 13);
            this._scriptArgumentsLabel.TabIndex = 4;
            this._scriptArgumentsLabel.Text = "Script arguments:";
            // 
            // _scriptArguments
            // 
            this._scriptArguments.Location = new System.Drawing.Point(112, 120);
            this._scriptArguments.Name = "_scriptArguments";
            this._scriptArguments.Size = new System.Drawing.Size(508, 20);
            this._scriptArguments.TabIndex = 5;
            // 
            // NodejsUwpPropertyPageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._scriptArguments);
            this.Controls.Add(this._scriptArgumentsLabel);
            this.Controls.Add(this._nodeArguments);
            this.Controls.Add(this._nodeArgumentsLabel);
            this.Controls.Add(this._debuggerMachineName);
            this.Controls.Add(this._debuggerMachineNameLabel);
            this.Name = "NodejsUwpPropertyPageControl";
            this.Size = new System.Drawing.Size(647, 210);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _debuggerMachineNameLabel;
        private System.Windows.Forms.TextBox _debuggerMachineName;
        private System.Windows.Forms.Label _nodeArgumentsLabel;
        private System.Windows.Forms.TextBox _nodeArguments;
        private System.Windows.Forms.Label _scriptArgumentsLabel;
        private System.Windows.Forms.TextBox _scriptArguments;
    }
}
