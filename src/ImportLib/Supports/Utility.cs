
#region Copyright notice
//===================================================================================
//
//===================================================================================
#endregion

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace ImportLib.Supports
{
    /// <summary>
    ///     Constans some utility methods
    /// </summary>
    public class Utility
    {
        /// <summary>
        ///     Convert a string into the Hexadecimal representation and then encode
        /// it for xml complaince
        /// </summary>
        /// <param name="originalValue">The original value</param>
        /// <returns>Formatted value</returns>
        public static string ToXmlComplaintHexString(string originalValue)
        {
            char[] characters = originalValue.ToCharArray();
            StringBuilder formattedValue = new StringBuilder();
            foreach (char ch in characters)
            {
                Byte bt = Convert.ToByte(ch);
                string xmlComplaint = string.Format("_x{0}_", bt.ToString("X2").PadLeft(4,'0'));
                formattedValue.Append(xmlComplaint);
            }
            return formattedValue.ToString();
        }
    }
}
