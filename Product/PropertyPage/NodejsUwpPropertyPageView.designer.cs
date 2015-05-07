/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

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
