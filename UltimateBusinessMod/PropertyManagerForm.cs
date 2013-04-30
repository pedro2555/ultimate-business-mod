using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace UltimateBusinessMod
{
    public class PropertyManagerForm : GTA.Forms.Form
    {
        private Property PP
        {
            get
            {
                return UltimateBusinessMod.ProximityProperty;
            }
        }
        private GTA.Player Player;

        #region Control declaration
        public GTA.Forms.Form PropertyManagerFrm;

        public GTA.Forms.Label WorkersDisplayLabel;

        public GTA.Forms.Button AddWorker;

        public GTA.Forms.Button RemoveWorker;

        

        #endregion

        public PropertyManagerForm(Size ScreenSize, GTA.Player Player)
            : base()
        {
            try
            {
                this.Player = Player;
                this.Size = ScreenSize;
                this.TitleBackColor = System.Drawing.Color.FromArgb(200, 0, 0, 0);
                this.BackColor = System.Drawing.Color.FromArgb(150, 0, 0, 0);
                this.Location = new System.Drawing.Point(0, 0);
                this.Closed += new EventHandler(UltimateBusinessMod.PropertyManagerFrm_Closed);
                // Workers label
                WorkersDisplayLabel = new GTA.Forms.Label();
                this.Controls.Add(WorkersDisplayLabel);
                WorkersDisplayLabel.Location = new System.Drawing.Point(30, 30);
                WorkersDisplayLabel.Size = new System.Drawing.Size(275, 50);
                // Add Worker Button
                AddWorker = new GTA.Forms.Button();
                AddWorker.Size = new System.Drawing.Size(100, 20);
                AddWorker.Location = new System.Drawing.Point(300, 30);
                AddWorker.Text = "Hire worker";
                AddWorker.Click += new GTA.MouseEventHandler(AddWorker_Click);
                this.Controls.Add(AddWorker);
                // Remove Worker Button
                RemoveWorker = new GTA.Forms.Button();
                RemoveWorker.Size = new System.Drawing.Size(100, 20);
                RemoveWorker.Location = new System.Drawing.Point(300, 55);
                RemoveWorker.Text = "Fire worker";
                RemoveWorker.Click += new GTA.MouseEventHandler(RemoveWorker_Click);
                this.Controls.Add(RemoveWorker);
            }
            catch (Exception crap) { LogFile.Log("PropertyManagerForm", crap.Message); }
        }


        public void RefreshGUI()
        {
            // Workers Label
            if (PP.StaffPay == 0)
                WorkersDisplayLabel.Text = String.Format("{0} out of {1} employees.\nSalary : {2:C}/hr", PP.StaffCount, PP.StaffCap, PP.StaffSal);
            else
                WorkersDisplayLabel.Text = String.Format("{0} out of {1} employees.\nSalary : {2:C}/hr.\n{3:C} up front payment.", PP.StaffCount, PP.StaffCap, PP.StaffSal, PP.StaffPay);
                
        }
        public new void Show()
        {
            try
            {
                this.Name = UltimateBusinessMod.ProximityProperty.Name + "'s Manager";
                RefreshGUI();
                base.Show();
            }
            catch (Exception crap) { LogFile.Log("Show", crap.Message); }
        }


        void RemoveWorker_Click(object sender, GTA.MouseEventArgs e)
        {
            UltimateBusinessMod.ProximityProperty.RemoveStaff();
            RefreshGUI();
        }

        void AddWorker_Click(object sender, GTA.MouseEventArgs e)
        {
            if (Player.Money - PP.StaffPay >= 0)
            {
                UltimateBusinessMod.ProximityProperty.AddStaff();
#if !DEBUG
                Player.Money -= PP.StaffPay;
#endif
            }
            RefreshGUI();
        }
    }
}
