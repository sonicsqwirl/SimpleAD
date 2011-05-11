#region License
/*
The MIT License

Copyright (c) 2008 Sky Morey

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;

namespace SimpleAD
{
    public static class DirectoryEntryExtensions
    {
        public static void SetValue<T>(this DirectoryEntry entry, string propertyName, T value) {
            if (entry == null)
                throw new ArgumentNullException("entry");
            var properties = entry.Properties;
            if (!properties.Contains(propertyName))
                return;
            var values = properties[propertyName];
            // if string and empty then clear.
            if ((typeof(T) == typeof(string)) && string.IsNullOrEmpty((string)(object)value)) {
                if (values.Count > 0)
                    values.Clear();
                return;
            }
            values.Value = value;
        }

        public static T GetValue<T>(this DirectoryEntry entry, string property)
        {
            if(entry.Properties.Contains(property))
            {
                return (T)entry.Properties[property].Value;
            }
            return default(T);
        }

        public static T GetFirstValue<T>(this DirectoryEntry entry, string propertyName) {
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            var properties = entry.Properties;
            PropertyValueCollection values;
            return (properties.Contains(propertyName) && ((values = properties[propertyName]).Count == 1) ? (T)values[0] : default(T));
        }

        //public static string GetString(this DirectoryEntry entry, string property) {
        //    return entry.Properties[property] == null ? string.Empty : entry.Properties[property].Value.ToString();
        //}

        //public static byte[] GetBytes(this DirectoryEntry entry, string property) {
        //    return entry.Properties[property] == null ? null : (byte[])entry.Properties[property].Value;
        //}

        public static SecurityIdentifier GetSid(this DirectoryEntry directoryEntry)
        {
            var objectSid = (byte[])directoryEntry.Properties["objectSid"].Value;
            return (objectSid != null ? new SecurityIdentifier(objectSid, 0) : null);
        }

        public static IEnumerable<DirectoryEntrySearchResult> ToSearchResult(this IEnumerable<DirectoryEntry> entries)
        {
           foreach(var result in entries)
           {
               yield return new DirectoryEntrySearchResult()
                                {
                                    Entry = result,
                                    ObjectId = new Guid(result.GetValue<byte[]>(PropertyHelper.objectGUID)),
                                    Path = result.Path,
                                    Dn = result.GetValue<string>(PropertyHelper.distinguishedName)
                                };
           }
        }

        public static DirectoryEntrySearchResult ToSearchResult(this DirectoryEntry entry)
        {
            return new DirectoryEntrySearchResult()
                       {
                           Entry = entry,
                           ObjectId = new Guid(entry.GetValue<byte[]>(PropertyHelper.objectGUID)),
                           Path = entry.Path,
                           Dn = entry.GetValue<string>(PropertyHelper.distinguishedName)
                       };

        }
    }
}
