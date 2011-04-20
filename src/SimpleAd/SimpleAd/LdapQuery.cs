using System.Text;

public class LdapQuery
{
    private StringBuilder queryBuilder = new StringBuilder();

    public override string ToString() {
        return queryBuilder.ToString();
    }

    public LdapQuery() { }
    public LdapQuery(string attribue, object value) : this(attribue, "=", value) { }
    public LdapQuery(string attribute, params LdapQuery[] value) {
        Add(attribute, value);
    }

    public LdapQuery(string attribue, string operation, object value) {
        if (!(value is LdapQuery)) {
            Add(attribue, operation + value);
        }
        else {
            Add(attribue, value);
        }
    }

    public void Add(string attribute, params object[] value) {
        var subs = new StringBuilder();
        foreach (var q in value) {
            subs.Append(q);
        }
        queryBuilder.AppendFormat("({0}{1})", attribute, subs);
    }
}
