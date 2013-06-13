using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimateBusinessMod
{
    partial class PropertyManagerForm
    {
        private void InitializeComponent()
        {
            this.TitleBackColor = System.Drawing.Color.FromArgb(200, 0, 0, 0);
            this.BackColor = System.Drawing.Color.FromArgb(150, 0, 0, 0);
            this.Location = new System.Drawing.Point(0, 0);
            this.Closed += new EventHandler(UltimateBusinessMod.PropertyManagerFrm_Closed);
            this.Closed += new EventHandler(PropertyManagerFrm_Closed);

            this.RemoveWorker = new GTA.Forms.Button();
            this.AddWorker = new GTA.Forms.Button();
            this.WorkersDisplayLabel = new GTA.Forms.Label();
            this.IncomeLabel = new GTA.Forms.Label();
            this.IncomeCollectBtn = new GTA.Forms.Button();
            
            //
            // Workers label
            //
            this.WorkersDisplayLabel.Location = new System.Drawing.Point(30, 30);
            this.WorkersDisplayLabel.Size = new System.Drawing.Size(275, 50);
            this.Controls.Add(WorkersDisplayLabel);
            //
            // Add Worker Button
            //
            this.AddWorker.Size = new System.Drawing.Size(100, 20);
            this.AddWorker.Location = new System.Drawing.Point(300, 30);
            this.AddWorker.Text = "Hire worker";
            this.AddWorker.Click += new GTA.MouseEventHandler(AddWorker_Click);
            this.Controls.Add(AddWorker);
            //
            // Remove Worker Button
            //
            this.RemoveWorker.Size = new System.Drawing.Size(100, 20);
            this.RemoveWorker.Location = new System.Drawing.Point(300, 55);
            this.RemoveWorker.Text = "Fire worker";
            this.RemoveWorker.Click += new GTA.MouseEventHandler(RemoveWorker_Click);
            this.Controls.Add(RemoveWorker);
            //
            // Income display label
            //
            this.IncomeLabel.Location = new System.Drawing.Point(30, 100);
            this.IncomeLabel.Size = new System.Drawing.Size(275, 50);
            this.Controls.Add(IncomeLabel);
            //
            // Income Collect Button
            //
            this.IncomeCollectBtn.Size = new System.Drawing.Size(100, 20);
            this.IncomeCollectBtn.Location = new System.Drawing.Point(300, 100);
            this.IncomeCollectBtn.Text = "Collect";
            this.IncomeCollectBtn.Click += new GTA.MouseEventHandler(IncomeCollectBtn_Click);
            this.Controls.Add(IncomeCollectBtn);
        }

        // Worker management stuff
        private GTA.Forms.Label WorkersDisplayLabel;
        private GTA.Forms.Button AddWorker;
        private GTA.Forms.Button RemoveWorker;

        // Income display and collecting
        private GTA.Forms.Label IncomeLabel;
        private GTA.Forms.Button IncomeCollectBtn;
    }
}
