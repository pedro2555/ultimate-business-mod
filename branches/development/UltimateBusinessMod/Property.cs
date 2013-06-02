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
using System.Data;
using GTA;

namespace UltimateBusinessMod
{
    public class Property
    {
        /// <summary>
        /// Returns an array of Property instances reflecting database data
        /// </summary>
        /// <returns></returns>
        public static Property[] GetPropertiesList()
        {
            List<Property> res = new List<Property>();
            DataTable dt = Database.GetDataTable("SELECT * FROM Properties ORDER BY ID ASC");
            foreach (DataRow dtr in dt.Rows)
            {
                try
                {
                    res.Add(new Property(Convert.ToInt32(dtr[0]), dtr[1].ToString(), dtr[2].ToString(), dtr[3].ToString(), Convert.ToInt32(dtr[4]), Convert.ToInt32(dtr[5]), Convert.ToInt32(dtr[6]), Convert.ToInt32(dtr[7]), Convert.ToInt32(dtr[8]), Convert.ToInt32(dtr[9]), Convert.ToInt32(dtr[10]), Convert.ToInt32(dtr[11]), Convert.ToInt32(dtr[12])));
                }
                catch (Exception crap)
                {
                    LogFile.Log("GetPropertiesList", "foreach");
                }
            }
            return res.ToArray();
        }

        private Property(int ID, string Name, string Location, string Flags, int Staff, int StaffCap, int Cost, int IncomeMin, int IncomeMax, int StaffSal, int StaffPay, int Income, int TypeID)
        {
            this.ID = ID;
            this.Name = Name;
            string[] location_array = Location.Split(',');
            this.Location = new Vector3(Convert.ToSingle(location_array[0]), Convert.ToSingle(location_array[1]), Convert.ToSingle(location_array[2]));
            this.Ownable = (Flags[0] == '1') ? true : false;
            this.Owned = (Flags[1] == '1') ? true : false;
            this.HasContextMission = (Flags[2] == '1') ? true : false;
            this.StaffPayed = (Flags[3] == '1') ? true : false;
            this.StaffCount = Staff; 
            this.StaffCap = StaffCap;
            this.Cost = Cost;
            this.IncomeMin = IncomeMin;
            this.IncomeMax = IncomeMax;
            this.StaffSal = StaffSal;
            this.StaffPay = StaffPay;
            this.Income = Income;
            this.TypeID = TypeID;
            LogFile.Log("DB_select", Flags);
        }

        #region db fields
        /// <summary>
        /// Database ID
        /// </summary>
        public int ID
        { get; private set; }
        /// <summary>
        /// Game map blip
        /// </summary>
        public Blip blip
        { get; set; }
        /// <summary>
        /// Gets property name, used to reference the property in-game
        /// </summary>
        public string Name
        { get; private set; }
        /// <summary>
        /// Gets property center point, used to detect player proximity and blip location
        /// </summary>
        public Vector3 Location
        { get; private set; }
        /// <summary>
        /// Gets if player can buy the property.
        /// </summary>
        public bool Ownable
        { get; private set; }
        /// <summary>
        /// Gets if player owns the property
        /// </summary>
        public bool Owned
        { get; set; }
        /// <summary>
        /// Gets if property should load a context mission on key press
        /// </summary>
        public bool HasContextMission
        { get; private set; }
        /// <summary>
        /// Gets property total staff
        /// </summary>
        public int StaffCount
        { get; private set; }
        /// <summary>
        /// Gets property staff cap
        /// </summary>
        public int StaffCap
        { get; private set; }
        /// <summary>
        /// Gets property cost
        /// </summary>
        public int Cost
        { get; private set; }
        /// <summary>
        /// Per staff member income min
        /// </summary>
        public int IncomeMin
        { get; private set; }
        /// <summary>
        /// Per staff member income max
        /// </summary>
        public int IncomeMax
        { get; private set; }
        /// <summary>
        /// Salary per staff member
        /// </summary>
        public int StaffSal
        { get; private set; }
        /// <summary>
        /// Staff hire up front pay
        /// </summary>
        public int StaffPay
        { get; private set; }
        /// <summary>
        /// Total income to collect
        /// </summary>
        public int Income
        { get; set; }
        /// <summary>
        /// ID of PropertyType
        /// </summary>
        public int TypeID
        { get; private set; }
        /// <summary>
        /// Keeps track if staff has been payed
        /// </summary>
        public bool StaffPayed
        { get; set; }
        #endregion
        /// <summary>
        /// Writes instance flags to correspondent database field
        /// </summary>
        public void UpdateFlags()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string data_s;
            lock (this)
                data_s = String.Format("{0}{1}{2}{3}", ((Ownable) ? "1" : "0"), ((Owned) ? "1" : "0"), ((HasContextMission) ? "1" : "0"), ((StaffPayed) ? "1" : "0"));
            data.Add("Flags", data_s);
            LogFile.Log("UpdateFlags", data_s);
            Database.Update("Properties", data, String.Format("ID={0}", this.ID));

        }
        /// <summary>
        /// Adds one staff member if possible
        /// 
        /// returns true on successful insertion, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool AddStaff()
        {
            if (this.StaffCount >= this.StaffCap)
                return false;
            this.StaffCount++;
            Dictionary<string, string> data = new Dictionary<string, string>();
            string data_s = this.StaffCount.ToString();
            data.Add("Staff", data_s);
            LogFile.Log("AddStaff", data_s);
            Database.Update("Properties", data, String.Format("ID={0}", this.ID));

            return true;
        }
        /// <summary>
        /// Removes one staff member if possible
        /// 
        /// returns true on successful insertion, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool RemoveStaff()
        {
            if (this.StaffCount == 0)
                return false;
            this.StaffCount--;
            Dictionary<string, string> data = new Dictionary<string, string>();
            string data_s = this.StaffCount.ToString();
            data.Add("Staff", data_s);
            LogFile.Log("AddStaff", data_s);
            Database.Update("Properties", data, String.Format("ID={0}", this.ID));

            return true;
        }
        /// <summary>
        /// Updates income value to value specified in newValue
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool UpdateIncome(int newValue)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("Income", newValue.ToString());
                LogFile.Log("UpdateIncome", newValue.ToString());
                Database.Update("Properties", data, String.Format("ID={0}", this.ID));

                return true;
            }
            catch (Exception crap) { LogFile.Log("UpdateIncome - Error", newValue.ToString()); return false; }

        }
    }
}
