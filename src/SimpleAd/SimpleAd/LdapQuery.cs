using System.DirectoryServices;
using System.Text;
namespace SimpleAD
{
    public class WildCardLdapQuery : LdapQuery
    {
        public WildCardLdapQuery():base(){}
        public WildCardLdapQuery(string attribue, object value) 
            :this(attribue,"=",value)
        {
        }
        public WildCardLdapQuery(string attribute, params LdapQuery[] value){
            Add(attribute, value);
        }
        public WildCardLdapQuery(string attribue, string operation, object value)
        {
            if (!(value is LdapQuery))
            {
                Add(attribue, operation + Ldap.EncodeFilter(value.ToString(), false));
            }
            else {
                Add(attribue, value);
            }
        }
    }

    public class LdapQuery
    {
        private StringBuilder queryBuilder = new StringBuilder();

        public override string ToString() {
            return queryBuilder.ToString();
        }

        public LdapQuery() { }
        public LdapQuery(string attribue, object value) 
            : this(attribue, "=", value) { }
        public LdapQuery(string attribute, params LdapQuery[] value) {
            Add(attribute, value);
        }

        public LdapQuery(string attribue, string operation, object value) {
            if (!(value is LdapQuery))
            {
                Add(attribue, operation + Ldap.EncodeFilter(value.ToString()));
            }
            else {
                Add(attribue, value);
            }
        }

        public virtual void Add(string attribute, params object[] value) {
            var subs = new StringBuilder();
            foreach (var q in value) {
                subs.Append(q);
            }
            queryBuilder.AppendFormat("({0}{1})", attribute, subs);
        }
    }
}