﻿/**
     Because i love chocolat...                                      
                                    88 88  
                                    "" 88  
                                       88  
8b       d8 88       88 8b,dPPYba,  88 88  
`8b     d8' 88       88 88P'    "8a 88 88  
 `8b   d8'  88       88 88       d8 88 ""  
  `8b,d8'   "8a,   ,a88 88b,   ,a8" 88 aa  
    Y88'     `"YbbdP'Y8 88`YbbdP"'  88 88  
    d8'                 88                 
   d8'                  88     
   
   Private Habbo Hotel Emulating System
   @author Claudio A. Santoro W.
   @author Kessiler R.
   @version dev-beta
   @license MIT
   @copyright Sulake Corporation Oy
   @observation All Rights of Habbo, Habbo Hotel, and all Habbo contents and it's names, is copyright from Sulake
   Corporation Oy. Yupi! has nothing linked with Sulake. 
   This Emulator is Only for DEVELOPMENT uses. If you're selling this you're violating Sulakes Copyright.
*/

using System.Collections.Specialized;
using System.Data;
using Yupi.Emulator.Core.Io.Logger;
using Yupi.Emulator.Data.Base.Adapters.Interfaces;

namespace Yupi.Emulator.Core.Settings
{
    /// <summary>
    ///     Class Languages.
    /// </summary>
     public class ServerLanguageSettings
    {
        /// <summary>
        ///     The texts
        /// </summary>
     public HybridDictionary Texts;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerLanguageSettings" /> class.
        /// </summary>
        /// <param name="language">The language.</param>
     public ServerLanguageSettings(string language)
        {
            Texts = new HybridDictionary();

            using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery($"SELECT * FROM server_langs WHERE lang = '{language}' ORDER BY id DESC");

                DataTable table = queryReactor.GetTable();

                if (table == null)
                    return;

                foreach (DataRow dataRow in table.Rows)
                {
                    string name = dataRow["name"].ToString();
                    string text = dataRow["text"].ToString();
                    Texts.Add(name, text);
                }
            }
        }

        /// <summary>
        ///     Gets the variable.
        /// </summary>
        /// <param name="var">The variable.</param>
        /// <returns>System.String.</returns>
     public string GetVar(string var)
        {
            if (Texts.Contains(var))
                return Texts[var].ToString();

            YupiWriterManager.WriteLine("Variable not found: " + var, "Yupi.Languages");

            return "Language variable not Found: " + var;
        }

        /// <summary>
        ///     Counts this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
     public int Count() => Texts.Count;
    }
}