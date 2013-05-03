using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace UltimateBusinessMod
{
    public partial class PropertyManagerForm : GTA.Forms.Form
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
        

        

        #endregion

        public PropertyManagerForm(Size ScreenSize, GTA.Player Player)
            : base()
        {
            try
            {
                this.Player = Player;
                this.Size = ScreenSize;
                LogFile.Log("SetFormSize", String.Format("{0},{1}", Size.Width, Size.Height));
                this.InitializeComponent();
            }
            catch (Exception crap) { LogFile.Log("PropertyManagerForm", crap.Message); }
        }
        /// <summary>
        /// Force clear of all tasks to prevent player air kick when exiting manager window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PropertyManagerFrm_Closed(object sender, EventArgs e)
        {
            Player.Character.Task.ClearAllImmediately();
        }

        public void RefreshGUI()
        {
            try
            {
                // Workers Label
                if (PP.StaffPay == 0)
                    WorkersDisplayLabel.Text = String.Format("{0} out of {1} employees.\nSalary : {2:C}/hr", PP.StaffCount, PP.StaffCap, PP.StaffSal);
                else
                    WorkersDisplayLabel.Text = String.Format("{0} out of {1} employees.\nSalary : {2:C}/hr.\n{3:C} up front payment.", PP.StaffCount, PP.StaffCap, PP.StaffSal, PP.StaffPay);
                if (PP.Income > 100)
                {
                    IncomeLabel.Text = String.Format("There's {0:C} to collect.", PP.Income);
                    IncomeCollectBtn.Visible = true;
                }
                else
                {
                    IncomeLabel.Text = String.Format("There's no income to collect.");
                    IncomeCollectBtn.Visible = false;
                }
            }
            catch (Exception crap) { LogFile.Log("RefreshGUI", crap.Message + "" + PP.Income); }
        }
        public new void Show()
        {
            try
            {
                this.Name = UltimateBusinessMod.ProximityProperty.Name + "'s Manager";
                RefreshGUI();
                base.ShowDialog();
            }
            catch (Exception crap) { LogFile.Log("Show", crap.Message); }
        }

        /// <summary>
        /// Remove worker button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RemoveWorker_Click(object sender, GTA.MouseEventArgs e)
        {
            UltimateBusinessMod.ProximityProperty.RemoveStaff();
            RefreshGUI();
        }
        /// <summary>
        /// Add worker button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Income collect button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IncomeCollectBtn_Click(object sender, GTA.MouseEventArgs e)
        {
            // Collect the money and reset the income value
            int income = PP.Income;
            PP.Income = 0;
            Player.Money += income;
            PP.UpdateIncome(0);
            RefreshGUI();
        }
    }
}
