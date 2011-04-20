using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;

namespace SimpleAD
{
    public interface ISimpleAdRepository
    {
        IEnumerable<DirectoryEntry> GetAll();
        IEnumerable<DirectoryEntry> FindAll(LdapQuery query);
        IEnumerable<DirectoryEntry> ResolveMembers(DirectoryEntry entry);
        IEnumerable<DirectoryEntry> ResolveMemberOf(DirectoryEntry entry);
        
        void AddToGroup(string groupDn, string userDn);
        void RemoveFromGroup(string groupDn, string userDn);

        DirectoryEntry CreateUser(string name, Dictionary<string, object> properties);
        DirectoryEntry CreateGroup(string name, Dictionary<string, object> properties);
        DirectoryEntry CreateOrganizationalUnit(string name, Dictionary<string, object> properties);

        void RemoveUser(string rootDn, string removeDn);
        void RemoveGroup(string rootDn, string removeDn);
        void RemoveOrganizationalUnit(string rootDn, string removeDn);
    }

    public class SimpleAdRepository<T> : ISimpleAdRepository where T : ISimpleAdContainer
    {
        private readonly ISimpleAdContainer container;

        public SimpleAdRepository(T container)
        {
            this.container = container;
        }

        private string GetDirectoryPath()
        {
            return GetDirectoryPath(container.Container);
        }
        private string GetDirectoryPath(string path)
        {
            return string.Format(@"LDAP://{0}/{1}", container.Domain, path);
        }

        public IEnumerable<DirectoryEntry> GetAll()
        {
            return FindAll(null);
        }
        public IEnumerable<DirectoryEntry> FindAll(LdapQuery query) {
            using (var root = new DirectoryEntry(GetDirectoryPath())) {
                using (var searcher = new DirectorySearcher(root)) {
                    searcher.SizeLimit = 1000;
                    if (query != null) {
                        searcher.Filter = query.ToString();
                    }
                    foreach (SearchResult de in searcher.FindAll())
                    {
                        yield return de.GetDirectoryEntry();
                    }
                }
            }
        }

        public IEnumerable<DirectoryEntry> ResolveMembers(DirectoryEntry entry)
        {
            object members = entry.Invoke("Members", null);
            foreach (object member in (IEnumerable) members)
            {
                var de = new DirectoryEntry(member);
                yield return de;
            }
        }
        public IEnumerable<DirectoryEntry> ResolveMemberOf(DirectoryEntry entry)
        {
            object members = entry.Invoke("Groups", null);
            foreach (object member in (IEnumerable) members)
            {
                var de = new DirectoryEntry(member);
                yield return de;
            }
        }

        public void AddToGroup(string groupDn, string userDn)
        {
            using(var root = new DirectoryEntry(GetDirectoryPath(groupDn)))
            {
                root.Properties["member"].Add(userDn);
                root.CommitChanges();
            }
        }
        public void RemoveFromGroup(string groupDn, string userDn)
        {
            using (var root = new DirectoryEntry(GetDirectoryPath(groupDn))) {
                root.Properties["member"].Remove(userDn);
                root.CommitChanges();
            }
        }
        
        public void RemoveUser(string rootDn, string removeDn)
        {
            RemoveEntry(rootDn, removeDn, "user");
        }
        public void RemoveGroup(string rootDn, string removeDn) {
            RemoveEntry(rootDn, removeDn, "group");
        }
        public void RemoveOrganizationalUnit(string rootDn, string removeDn) {
            RemoveEntry(rootDn, removeDn, "organizationalunit");
        }
        private void RemoveEntry(string rootDn, string removeDn, string schemaClass)
        {
            using (var root = new DirectoryEntry(GetDirectoryPath(rootDn)))
            {
                var de = root.Children.Find(removeDn, schemaClass);
                root.Children.Remove(de);
            }
        }

        public DirectoryEntry CreateUser(string name, Dictionary<string, object> properties) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("User must have a name!");

            if (!name.Trim().ToLower().StartsWith("cn=")) {
                name = "cn=" + name;
            }

            var de = CreateObject("user", name, properties);
            return de;
        }
        public DirectoryEntry CreateGroup(string name, Dictionary<string, object> properties) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Group must have a name!");

            if (!name.Trim().ToLower().StartsWith("cn=")) {
                name = "cn=" + name;
            }

            var de = CreateObject("group", name, properties);
            return de;
        }
        public DirectoryEntry CreateOrganizationalUnit(string name, Dictionary<string, object> properties)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("OrganizationalUnit must have a name!");

            if (!name.Trim().ToLower().StartsWith("ou="))
            {
                name = "ou=" + name;
            }

            var de = CreateObject("organizationalunit", name, properties);
            return de;
        }
        private DirectoryEntry CreateObject(string schemaClass, string name, Dictionary<string, object> properties)
        {
            using (var root = new DirectoryEntry(GetDirectoryPath()))
            {
                var de = root.Children.Add(name, schemaClass);
                if (properties != null)
                {
                    foreach (var prop in properties.Keys)
                    {
                        de.Properties[prop].Value = properties[prop];
                    }
                }
                de.CommitChanges();

                return de;
            }
        }
    }

}
