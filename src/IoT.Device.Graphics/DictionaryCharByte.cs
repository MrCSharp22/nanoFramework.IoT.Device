﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// This is a class added (and should be autogenerated) specifically for nanoFramework.

using System;
using System.Collections;

namespace Iot.Device.Graphics
{
    /// <summary>
    /// A dictonary of CharByte
    /// </summary>
    public class DictionaryCharByte
    {
        ArrayList _array = new ArrayList();
        /// <summary>
        /// DictionaryCharByte
        /// </summary>
        public DictionaryCharByte()
        { }

        /// <summary>
        /// Adds a CharByte
        /// </summary>
        /// <param name="cb"></param>
        public void Add(CharByte cb)
        {
            _array.Add(cb);
        }

        /// <summary>
        /// Adds a CharByte
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="bt"></param>
        public void Add(char cr, byte bt)
        {
            _array.Add(new CharByte(cr, bt));
        }

        /// <summary>
        /// Tries to add a CharByte
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="bt"></param>
        /// <returns></returns>
        public bool TryAdd(char cr, byte bt)
        {
            foreach (CharByte cb in _array)
            {
                if (cb.Cr == cr)
                {
                    return false;
                }
            }

            _array.Add(new CharByte(cr, bt));
            return true;
        }

        /// <summary>
        /// Tries to get the value of CharByte
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryGetValue(char cr, out byte val)
        {
            foreach (CharByte cb in _array)
            {
                if (cb.Cr == cr)
                {
                    val = cb.Bt;
                    return true;
                }
            }

            val = 0;
            return false;
        }

        /// <summary>
        /// Checks if CharByte contains a key
        /// </summary>
        /// <param name="cr"></param>
        /// <returns></returns>
        public bool ContainsKey(char cr)
        {
            foreach (CharByte cb in _array)
            {
                if (cb.Cr == cr)
                {
                    return true;
                }
            }

            return false;
        }

        ///// <summary>
        ///// Removes the first item with a key equal to <paramref name="cr"/> if found.
        ///// </summary>
        ///// <param name="cr">The character (key) to look for.</param>
        //public void Remove(char cr)
        //{
        //    var index = -1;
        //    for (var i = 0; i < _array.Count; i++)
        //    {
        //        var cb = (CharByte)_array[i];
        //        if (cb.Cr == cr)
        //        {
        //            index = i;
        //            break;
        //        }
        //    }

        //    if (index > -1)
        //        _array.RemoveAt(index);
        //}
    }
}
