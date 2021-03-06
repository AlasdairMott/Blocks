using Blocks.Common.Generators;
using Blocks.Common.Parameters;
using Blocks.Viewer.Data;
using System;
using System.Windows.Forms;

namespace Blocks.Viewer.Dialogs
{
    public class GraphParameters : Form
    {
        private Button button_Apply;
        private Button button_Run;
        private Label label_SpringConstant;
        private Label label_RestLength;
        private NumericUpDown step_RestLength;
        private Label label_Repulsion;
        private NumericUpDown step_Repulsion;
        private Label label_RepulsionRadius;
        private NumericUpDown step_RepulsionRadius;
        private Label label_Iterations;
        private NumericUpDown step_Iterations;
        private Label label_CoolingFactor;
        private NumericUpDown step_CoolingFactor;
        private NumericUpDown step_SpringConstant;

        public GraphParameters()
        {
            InitializeComponent();
            SetDefaults();
        }

        private void InitializeComponent()
        {
            this.step_SpringConstant = new System.Windows.Forms.NumericUpDown();
            this.button_Apply = new System.Windows.Forms.Button();
            this.button_Run = new System.Windows.Forms.Button();
            this.label_SpringConstant = new System.Windows.Forms.Label();
            this.label_RestLength = new System.Windows.Forms.Label();
            this.step_RestLength = new System.Windows.Forms.NumericUpDown();
            this.label_Repulsion = new System.Windows.Forms.Label();
            this.step_Repulsion = new System.Windows.Forms.NumericUpDown();
            this.label_RepulsionRadius = new System.Windows.Forms.Label();
            this.step_RepulsionRadius = new System.Windows.Forms.NumericUpDown();
            this.label_Iterations = new System.Windows.Forms.Label();
            this.step_Iterations = new System.Windows.Forms.NumericUpDown();
            this.label_CoolingFactor = new System.Windows.Forms.Label();
            this.step_CoolingFactor = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.step_SpringConstant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_RestLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_Repulsion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_RepulsionRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_Iterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_CoolingFactor)).BeginInit();
            this.SuspendLayout();
            // 
            // step_SpringConstant
            // 
            this.step_SpringConstant.DecimalPlaces = 3;
            this.step_SpringConstant.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.step_SpringConstant.Location = new System.Drawing.Point(220, 36);
            this.step_SpringConstant.Name = "step_SpringConstant";
            this.step_SpringConstant.Size = new System.Drawing.Size(115, 31);
            this.step_SpringConstant.TabIndex = 0;
            // 
            // button_Apply
            // 
            this.button_Apply.Location = new System.Drawing.Point(220, 341);
            this.button_Apply.Name = "button_Apply";
            this.button_Apply.Size = new System.Drawing.Size(115, 38);
            this.button_Apply.TabIndex = 1;
            this.button_Apply.Text = "Apply";
            this.button_Apply.UseVisualStyleBackColor = true;
            this.button_Apply.Click += new System.EventHandler(this.OnApply_Click);
            // 
            // button_Run
            // 
            this.button_Run.Location = new System.Drawing.Point(36, 341);
            this.button_Run.Name = "button_Run";
            this.button_Run.Size = new System.Drawing.Size(115, 38);
            this.button_Run.TabIndex = 2;
            this.button_Run.Text = "Run";
            this.button_Run.UseVisualStyleBackColor = true;
            this.button_Run.Click += new System.EventHandler(this.OnRunClick);
            // 
            // label_SpringConstant
            // 
            this.label_SpringConstant.AutoSize = true;
            this.label_SpringConstant.Location = new System.Drawing.Point(35, 42);
            this.label_SpringConstant.Name = "label_SpringConstant";
            this.label_SpringConstant.Size = new System.Drawing.Size(166, 25);
            this.label_SpringConstant.TabIndex = 3;
            this.label_SpringConstant.Text = "Spring Constant";
            // 
            // label_RestLength
            // 
            this.label_RestLength.AutoSize = true;
            this.label_RestLength.Location = new System.Drawing.Point(35, 91);
            this.label_RestLength.Name = "label_RestLength";
            this.label_RestLength.Size = new System.Drawing.Size(128, 25);
            this.label_RestLength.TabIndex = 5;
            this.label_RestLength.Text = "Rest Length";
            // 
            // step_RestLength
            // 
            this.step_RestLength.DecimalPlaces = 3;
            this.step_RestLength.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.step_RestLength.Location = new System.Drawing.Point(220, 85);
            this.step_RestLength.Name = "step_RestLength";
            this.step_RestLength.Size = new System.Drawing.Size(115, 31);
            this.step_RestLength.TabIndex = 4;
            // 
            // label_Repulsion
            // 
            this.label_Repulsion.AutoSize = true;
            this.label_Repulsion.Location = new System.Drawing.Point(35, 140);
            this.label_Repulsion.Name = "label_Repulsion";
            this.label_Repulsion.Size = new System.Drawing.Size(108, 25);
            this.label_Repulsion.TabIndex = 7;
            this.label_Repulsion.Text = "Repulsion";
            // 
            // step_Repulsion
            // 
            this.step_Repulsion.DecimalPlaces = 3;
            this.step_Repulsion.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.step_Repulsion.Location = new System.Drawing.Point(220, 134);
            this.step_Repulsion.Name = "step_Repulsion";
            this.step_Repulsion.Size = new System.Drawing.Size(115, 31);
            this.step_Repulsion.TabIndex = 6;
            // 
            // label_RepulsionRadius
            // 
            this.label_RepulsionRadius.AutoSize = true;
            this.label_RepulsionRadius.Location = new System.Drawing.Point(35, 189);
            this.label_RepulsionRadius.Name = "label_RepulsionRadius";
            this.label_RepulsionRadius.Size = new System.Drawing.Size(181, 25);
            this.label_RepulsionRadius.TabIndex = 9;
            this.label_RepulsionRadius.Text = "Repulsion Radius";
            // 
            // step_RepulsionRadius
            // 
            this.step_RepulsionRadius.DecimalPlaces = 3;
            this.step_RepulsionRadius.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.step_RepulsionRadius.Location = new System.Drawing.Point(220, 183);
            this.step_RepulsionRadius.Name = "step_RepulsionRadius";
            this.step_RepulsionRadius.Size = new System.Drawing.Size(115, 31);
            this.step_RepulsionRadius.TabIndex = 8;
            // 
            // label_Iterations
            // 
            this.label_Iterations.AutoSize = true;
            this.label_Iterations.Location = new System.Drawing.Point(35, 287);
            this.label_Iterations.Name = "label_Iterations";
            this.label_Iterations.Size = new System.Drawing.Size(100, 25);
            this.label_Iterations.TabIndex = 11;
            this.label_Iterations.Text = "Iterations";
            // 
            // step_Iterations
            // 
            this.step_Iterations.BackColor = System.Drawing.SystemColors.Window;
            this.step_Iterations.Location = new System.Drawing.Point(220, 281);
            this.step_Iterations.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.step_Iterations.Name = "step_Iterations";
            this.step_Iterations.Size = new System.Drawing.Size(115, 31);
            this.step_Iterations.TabIndex = 10;
            // 
            // label_CoolingFactor
            // 
            this.label_CoolingFactor.AutoSize = true;
            this.label_CoolingFactor.Location = new System.Drawing.Point(35, 238);
            this.label_CoolingFactor.Name = "label_CoolingFactor";
            this.label_CoolingFactor.Size = new System.Drawing.Size(152, 25);
            this.label_CoolingFactor.TabIndex = 13;
            this.label_CoolingFactor.Text = "Cooling Factor";
            // 
            // step_CoolingFactor
            // 
            this.step_CoolingFactor.DecimalPlaces = 3;
            this.step_CoolingFactor.Increment = (decimal)0.001;
            this.step_CoolingFactor.Location = new System.Drawing.Point(220, 232);
            this.step_CoolingFactor.Name = "step_CoolingFactor";
            this.step_CoolingFactor.Size = new System.Drawing.Size(115, 31);
            this.step_CoolingFactor.TabIndex = 12;
            // 
            // GraphParameters
            // 
            this.ClientSize = new System.Drawing.Size(369, 411);
            this.Controls.Add(this.label_CoolingFactor);
            this.Controls.Add(this.step_CoolingFactor);
            this.Controls.Add(this.label_Iterations);
            this.Controls.Add(this.step_Iterations);
            this.Controls.Add(this.label_RepulsionRadius);
            this.Controls.Add(this.step_RepulsionRadius);
            this.Controls.Add(this.label_Repulsion);
            this.Controls.Add(this.step_Repulsion);
            this.Controls.Add(this.label_RestLength);
            this.Controls.Add(this.step_RestLength);
            this.Controls.Add(this.label_SpringConstant);
            this.Controls.Add(this.button_Run);
            this.Controls.Add(this.button_Apply);
            this.Controls.Add(this.step_SpringConstant);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GraphParameters";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GraphParameters_Load);
            ((System.ComponentModel.ISupportInitialize)(this.step_SpringConstant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_RestLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_Repulsion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_RepulsionRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_Iterations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.step_CoolingFactor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void SetDefaults()
        {
            var parameters = Preferences.GraphGeneratorParameters;

            step_SpringConstant.Value = (decimal)parameters.SpringConstant;
            step_RestLength.Value = (decimal)parameters.RestingLength;
            step_Repulsion.Value = (decimal)parameters.RepulsionFactor;
            step_RepulsionRadius.Value = (decimal)parameters.RepsulionRadius;
            step_CoolingFactor.Value = (decimal)parameters.CoolingFactor;
            step_Iterations.Value = parameters.MaxIterations;
        }

        public GraphGeneratorParameters Get()
        {
            return new GraphGeneratorParameters(
                (double)step_SpringConstant.Value, 
                (double)step_RestLength.Value,
                (double)step_Repulsion.Value,
                (double)step_RepulsionRadius.Value,
                (double)step_CoolingFactor.Value,
                (int)step_Iterations.Value
            );
        }
        private void OnRunClick(object sender, EventArgs e)
        {
            MainForm.Reference?.ComputeGraph(Get());
            MainForm.Generated?.ComputeGraph(Get());
            MainForm.RefreshViewports();
        }

        private void OnApply_Click(object sender, System.EventArgs e)
        {
            var parameters = Preferences.GraphGeneratorParameters;

            parameters.SpringConstant = (double)step_SpringConstant.Value;
            parameters.RestingLength = (double)step_RestLength.Value;
            parameters.RepulsionFactor = (double)step_Repulsion.Value;
            parameters.RepsulionRadius = (double)step_RepulsionRadius.Value;
            parameters.MaxIterations = (int)step_Iterations.Value;
        }

        private void GraphParameters_Load(object sender, System.EventArgs e)
        {

        }
    }
}
