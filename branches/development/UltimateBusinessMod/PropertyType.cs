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

namespace UltimateBusinessMod
{
    public class PropertyType
    {
        #region properties
        /// <summary>
        /// Database ID
        /// </summary>
        internal int ID
        { get; private set; }
        /// <summary>
        /// Property type description name
        /// </summary>
        public string Name
        { get; private set; }
        /// <summary>
        ///  Maximun income per staff member
        /// </summary>
        public int IncomeMax
        { get; private set; }
        /// <summary>
        /// Minimum income per staff member
        /// </summary>
        public int IncomeMin
        { get; private set; }
        /// <summary>
        /// Per update cost on each staff member
        /// </summary>
        public int StaffCost
        { get; set; }
        #endregion

        private PropertyType(int ID, string Name, int IncMax, int IncMin, int StaffCost)
        {
            this.ID = ID;
            this.Name = Name;
            this.IncomeMax = IncMax;
            this.IncomeMin = IncMin;
            this.StaffCost = StaffCost;
        }
        
        /// <summary>
        /// Returns an array of PropertyType instances reflecting database data
        /// </summary>
        /// <returns></returns>
        public static PropertyType[] GetTypesList()
        {
            List<PropertyType> res = new List<PropertyType>();
            DataTable dt = Database.GetDataTable("SELECT * FROM PropertyTypes ORDER BY ID ASC");
            foreach (DataRow dtr in dt.Rows)
                res.Add(new PropertyType(Convert.ToInt32(dtr[0]), (string)dtr[1], Convert.ToInt32(dtr[2]), Convert.ToInt32(dtr[3]), Convert.ToInt32(dtr[3])));
            return res.ToArray();
        }
    }
}
