﻿/**
* Digital Voice Modem - Fixed Network Equipment
* AGPLv3 Open Source. Use is subject to license terms.
* DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
*
* @package DVM / Fixed Network Equipment
*
*/
//
// Based on code from the MMDVMHost project. (https://github.com/g4klx/MMDVMHost)
// Licensed under the GPLv2 License (https://opensource.org/licenses/GPL-2.0)
//
/*
*   Copyright (C) 2022 by Bryan Biedenkapp N2PLL
*
*   This program is free software: you can redistribute it and/or modify
*   it under the terms of the GNU Affero General Public License as published by
*   the Free Software Foundation, either version 3 of the License, or
*   (at your option) any later version.
*
*   This program is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU Affero General Public License for more details.
*/

using System;

namespace fnecore.DMR
{
    /// <summary>
    /// Represents DMR link control data.
    /// </summary>
    public class LC
    {
        private bool R;

        /// <summary>
        /// Flag indicating whether link protection is enabled.
        /// </summary>
        public bool PF;

        /// <summary>
        /// Full-link control opcode.
        /// </summary>
        public byte FLCO;

        /// <summary>
        /// Feature ID.
        /// </summary>
        public byte FID;

        /// <summary>
        /// Source ID.
        /// </summary>
        public uint SrcId;

        /// <summary>
        /// Destination ID.
        /// </summary>
        public uint DstId;

        /** Service Options */
        /// <summary>
        /// Flag indicating the emergency bits are set.
        /// </summary>
        public bool Emergency;

        /// <summary>
        /// Flag indicating that encryption is enabled.
        /// </summary>
        public bool Encrypted;

        /// <summary>
        /// Flag indicating broadcast operation.
        /// </summary>
        public bool Broadcast;

        /// <summary>
        /// Flag indicating OVCM operation.
        /// </summary>
        public bool OVCM;

        /// <summary>
        /// Priority level for the traffic.
        /// </summary>
        public byte Priority;

        /*
        ** Methods
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="LC"/> class.
        /// </summary>
        public LC()
        {
            PF = false;

            FLCO = 0;
            FID = 0;

            SrcId = 0;
            DstId = 0;

            Emergency = false;
            Encrypted = false;
            Broadcast = false;
            OVCM = false;
            Priority = 2;
            
            R = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LC"/> class.
        /// </summary>
        /// <param name="bytes"></param>
        public LC(byte[] bytes)
        {
            PF = (bytes[0U] & 0x80U) == 0x80U;
            R = (bytes[0U] & 0x40U) == 0x40U;

            FLCO = (byte)(bytes[0U] & 0x3FU);

            FID = bytes[1U];

            Emergency = (bytes[2U] & 0x80U) == 0x80U;                                 // Emergency Flag
            Encrypted = (bytes[2U] & 0x40U) == 0x40U;                                 // Encryption Flag
            Broadcast = (bytes[2U] & 0x08U) == 0x08U;                                 // Broadcast Flag
            OVCM = (bytes[2U] & 0x04U) == 0x04U;                                      // OVCM Flag
            Priority = (byte)(bytes[2U] & 0x03U);                                     // Priority

            DstId = (uint)(bytes[3U] << 16 | bytes[4U] << 8 | bytes[5U]);             // Destination Address
            SrcId = (uint)(bytes[6U] << 16 | bytes[7U] << 8 | bytes[8U]);             // Source Address
        }

        /// <summary>
        /// Gets LC data as bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] lcData = new byte[12];
            GetData(ref lcData);

            return lcData;
        }

        /// <summary>
        /// Gets LC data as bytes.
        /// </summary>
        /// <param name="bytes"></param>
        public void GetData(ref byte[] bytes)
        {
            if (bytes == null)
                throw new NullReferenceException("bytes");

            bytes[0U] = FLCO;

            if (PF)
                bytes[0U] |= (byte)0x80U;

            if (R)
                bytes[0U] |= (byte)0x40U;

            bytes[1U] = FID;

            bytes[2U] = (byte)((Emergency ? 0x80U : 0x00U) +                          // Emergency Flag
                (Encrypted ? 0x40U : 0x00U) +                                         // Encrypted Flag
                (Broadcast ? 0x08U : 0x00U) +                                         // Broadcast Flag
                (OVCM ? 0x04U : 0x00U) +                                              // OVCM Flag
                (Priority & 0x03U));                                                  // Priority

            bytes[3U] = (byte)(DstId >> 16);                                          // Destination Address
            bytes[4U] = (byte)(DstId >> 8);                                           // ..
            bytes[5U] = (byte)(DstId >> 0);                                           // ..

            bytes[6U] = (byte)(SrcId >> 16);                                          // Source Address
            bytes[7U] = (byte)(SrcId >> 8);                                           // ..
            bytes[8U] = (byte)(SrcId >> 0);                                           // ..
        }
    } // public class LC
} // namespace fnecore.DMR
