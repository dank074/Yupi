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

namespace Yupi.Emulator.Game.Browser.Models
{
    /// <summary>
    ///     Struct NavigatorHeader
    /// </summary>
     struct NavigatorHeader
    {
        /// <summary>
        ///     The room identifier
        /// </summary>
         uint RoomId;

        /// <summary>
        ///     The caption
        /// </summary>
         string Caption;

        /// <summary>
        ///     The image
        /// </summary>
         string Image;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NavigatorHeader" /> struct.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="image">The image.</param>
         NavigatorHeader(uint roomId, string caption, string image)
        {
            RoomId = roomId;
            Caption = caption;
            Image = image;
        }
    }
}