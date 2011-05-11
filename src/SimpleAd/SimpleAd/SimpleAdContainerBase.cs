using SimpleAD;

namespace SimpleAD
{
    public class SimpleAdContainerBase : ISimpleAdContainer
    {
        public string Domain { get; set; }
        public string Container { get; set; }

        public SimpleAdContainerBase(string domain, string container) {
            Domain = domain;
            Container = container;
        }

    }
}