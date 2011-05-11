using System;
using System.DirectoryServices;

namespace SimpleAD
{
    public class DirectoryEntrySearchResult
    {
        public Guid ObjectId { get; set; }
        public string Path { get; set; }
        public string Dn { get; set; }
        public DirectoryEntry Entry { get; set; }
    }
}