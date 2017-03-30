namespace iDeal.Directory
{
    public class Issuer
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public string CountryNames { get; private set; }

        public Issuer(string id, string name, string countryNames)
        {
            Id = id;
            Name = name;
            CountryNames = countryNames;
        }
    }
}
