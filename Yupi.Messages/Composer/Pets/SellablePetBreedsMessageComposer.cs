﻿// ---------------------------------------------------------------------------------
// <copyright file="SellablePetBreedsMessageComposer.cs" company="https://github.com/sant0ro/Yupi">
//   Copyright (c) 2016 Claudio Santoro, TheDoctor
// </copyright>
// <license>
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </license>
// ---------------------------------------------------------------------------------
namespace Yupi.Messages.Pets
{
    using System;
    using System.Collections.Generic;

    using Yupi.Protocol.Buffers;

    public class SellablePetBreedsMessageComposer : Yupi.Messages.Contracts.SellablePetBreedsMessageComposer
    {
        #region Methods

        public override void Compose(Yupi.Protocol.ISender session, string type)
        {
            // TODO Refactor?

            /*
            string petType = PetTypeManager.GetPetTypeByHabboPetType(type);

            uint petId = PetTypeManager.GetPetRaceByItemName(petType);

            List<PetRace> races = PetTypeManager.GetRacesByPetType(petType);

            using (ServerMessage message = Pool.GetMessageBuffer (Id)) {
                message.AppendString(type);
                message.AppendInteger(races.Count);

                foreach (PetRace current in races)
                {
                    message.AppendInteger(petId);
                    message.AppendInteger(current.ColorOne);
                    message.AppendInteger(current.ColorTwo);
                    message.AppendBool(current.HasColorOne);
                    message.AppendBool(current.HasColorTwo);
                }
                session.Send (message);
            }
            */
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}