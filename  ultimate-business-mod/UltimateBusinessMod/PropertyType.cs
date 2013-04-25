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
        #endregion

        private PropertyType(int ID, string Name, int IncMax, int IncMin)
        {
            this.ID = ID;
            this.Name = Name;
            this.IncomeMax = IncMax;
            this.IncomeMin = IncMin;
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
                res.Add(new PropertyType((int)dtr[0], (string)dtr[1], (int)dtr[2], (int)dtr[3]));
            return res.ToArray();
        }
    }
}
