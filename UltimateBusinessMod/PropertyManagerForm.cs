/// Copyright (c) 2013, Pedro Rodrigues <prodrigues1990@gmail.com>
/// All rights reserved.
/// 
/// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
/// 
/// 	-Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
/// 	-Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
/// 	-Neither the name of the author nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
                lock (PP)
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
            lock (PP)
            {
                if (Player.Money - PP.StaffPay >= 0)
                {
                    UltimateBusinessMod.ProximityProperty.AddStaff();
                    Player.Money -= PP.StaffPay;
                }
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
            lock (PP)
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
}
