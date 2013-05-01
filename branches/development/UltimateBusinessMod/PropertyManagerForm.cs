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
                this.InitializeComponent();
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
