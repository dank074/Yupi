﻿#region Header

// ---------------------------------------------------------------------------------
// <copyright file="HotelLandingPromos.cs" company="https://github.com/sant0ro/Yupi">
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

#endregion Header

namespace Yupi.Model.Domain
{
    using System;

    using Yupi.Model.Domain.Components;

    public class HotelLandingPromos
    {
        #region Properties

        [Required]
        public virtual string Body
        {
            get; set;
        }

        [Required]
        public virtual string Button
        {
            get; set;
        }

        [Required]
        public virtual DateTime CreatedAt {
            get; set;
        } = DateTime.Now;

        [Key]
        public virtual int Id
        {
            get; protected set;
        }

        [Required]
        public virtual string Image
        {
            get; set;
        }

        [Required]
        public virtual PromoLink Link
        {
            get; set;
        }

        [Required]
        public virtual string Title
        {
            get; set;
        }

        #endregion Properties
    }
}