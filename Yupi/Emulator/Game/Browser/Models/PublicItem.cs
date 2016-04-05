/**
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

using Yupi.Emulator.Game.Browser.Enums;
using Yupi.Emulator.Game.Rooms.Data;
using Yupi.Emulator.Game.Rooms.Data.Models;
using Yupi.Emulator.Messages.Buffers;

namespace Yupi.Emulator.Game.Browser.Models
{
    /// <summary>
    ///     Class PublicItem.
    /// </summary>
     class PublicItem
    {
        /// <summary>
        ///     The caption
        /// </summary>
         string Caption;

        /// <summary>
        ///     The category identifier
        /// </summary>
         int CategoryId;

        /// <summary>
        ///     The description
        /// </summary>
         string Description;

        /// <summary>
        ///     The image
        /// </summary>
         string Image;

        /// <summary>
        ///     The image type
        /// </summary>
         PublicImageType ImageType;

        /// <summary>
        ///     The item type
        /// </summary>
         PublicItemType ItemType;

        /// <summary>
        ///     The parent identifier
        /// </summary>
         int ParentId;

        /// <summary>
        ///     The recommended
        /// </summary>
         bool Recommended;

        /// <summary>
        ///     The room identifier
        /// </summary>
         uint RoomId;

        /// <summary>
        ///     The tags to search
        /// </summary>
         string TagsToSearch = string.Empty;

        /// <summary>
        ///     The type
        /// </summary>
         int Type;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PublicItem" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="desc">The desc.</param>
        /// <param name="image">The image.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <param name="recommand">if set to <c>true</c> [recommand].</param>
        /// <param name="typeOfData">The type of data.</param>
         PublicItem(uint id, int type, string caption, string desc, string image, PublicImageType imageType, uint roomId, int categoryId, int parentId, bool recommand, int typeOfData)
        {
            Id = id;
            Type = type;
            Caption = caption;
            Description = desc;
            Image = image;
            ImageType = imageType;
            RoomId = roomId;
            CategoryId = categoryId;
            ParentId = parentId;
            Recommended = recommand;

            switch (typeOfData)
            {
                case 1:
                    ItemType = PublicItemType.Tag;
                    break;
                case 2:
                    ItemType = PublicItemType.Flat;
                    break;
                case 3:
                    ItemType = PublicItemType.PublicFlat;
                    break;
                case 4:
                    ItemType = PublicItemType.Category;
                    break;
                default:
                    ItemType = PublicItemType.None;
                    break;
            }
        }

        /// <summary>
        ///     Gets the room information.
        /// </summary>
        /// <value>The room information.</value>
         RoomData RoomInfo => RoomId > 0u ? Yupi.GetGame().GetRoomManager().GenerateRoomData(RoomId) : null;

        /// <summary>
        ///     Serializes the specified messageBuffer.
        /// </summary>
        /// <param name="messageBuffer">The messageBuffer.</param>
         void Serialize(SimpleServerMessageBuffer messageBuffer)
        {
            messageBuffer.AppendInteger(Id);
            messageBuffer.AppendString(Caption);
            messageBuffer.AppendString(Description);
            messageBuffer.AppendInteger(Type);
            messageBuffer.AppendString(Caption);
            messageBuffer.AppendString(Image);
            messageBuffer.AppendInteger(ParentId);
            messageBuffer.AppendInteger(RoomInfo?.UsersNow ?? 0);
            messageBuffer.AppendInteger(ItemType == PublicItemType.None ? 0 : (ItemType == PublicItemType.Tag ? 1 : (ItemType == PublicItemType.Flat ? 2 : (ItemType == PublicItemType.PublicFlat ? 2 : (ItemType != PublicItemType.Category ? 0 : 4)))));

            switch (ItemType)
            {
                case PublicItemType.Tag:
                    messageBuffer.AppendString(TagsToSearch);
                    break;
                case PublicItemType.Category:
                    messageBuffer.AppendBool(false);
                    break;
                case PublicItemType.Flat:
                    RoomInfo?.Serialize(messageBuffer);
                    break;
                case PublicItemType.PublicFlat:
                    RoomInfo?.Serialize(messageBuffer);
                    break;
            }
        }

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
         uint Id { get; set; }

        /// <summary>
        ///     Gets the room data.
        /// </summary>
        /// <value>The room data.</value>
        /// <exception cref="System.NullReferenceException"></exception>
         RoomData GetPublicRoomData => RoomId == 0u ? new RoomData() : Yupi.GetGame().GetRoomManager().GenerateRoomData(RoomId);
    }
}