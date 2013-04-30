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
                    res.Add(new Property(Convert.ToInt32(dtr[0]), dtr[1].ToString(), dtr[2].ToString(), dtr[3].ToString(), Convert.ToInt32(dtr[4]), Convert.ToInt32(dtr[5]), Convert.ToInt32(dtr[6])));
                }
                catch (Exception crap)
                {
                    LogFile.Log("GetPropertiesList - foreach", String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}",
                        dtr[0].ToString(), dtr[1].ToString(), dtr[2].ToString(), dtr[3].ToString(), dtr[4].ToString(), dtr[5].ToString(), dtr[6].ToString()));
                }
            }
            return res.ToArray();
        }

        private Property(int ID, string Name, string Location, string Flags, int Staff, int StaffCap, int Cost)
        {
            this.ID = ID;
            this.Name = Name;
            string[] location_array = Location.Split(',');
            this.Location = new Vector3(Convert.ToSingle(location_array[0]), Convert.ToSingle(location_array[1]), Convert.ToSingle(location_array[2]));
            this.Ownable = (Flags[0] == '1') ? true : false;
            this.Owned = (Flags[1] == '1') ? true : false;
            this.HasContextMission = (Flags[2] == '1') ? true : false;
            this.StaffCount = Staff;
            this.StaffCap = StaffCap;
            this.Cost = Cost;
            LogFile.Log("DB_select_flags", Flags);
        }

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
        /// 
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
        /// Writes instance flags to correspondent database field
        /// </summary>
        public void UpdateFlags()
        {
            Dictionary<string, string> data = new Dictionary<string,string>();
            string data_s = String.Format("{0}{1}{2}", ((Ownable) ? "1" : "0"), ((Owned) ? "1" : "0"), ((HasContextMission) ? "1" : "0"));
            data.Add("Flags", data_s);
            LogFile.Log("UpdateFlags", data_s); 
            Database.Update("Properties", data, String.Format("ID={0}", this.ID));
        }
    }
}
