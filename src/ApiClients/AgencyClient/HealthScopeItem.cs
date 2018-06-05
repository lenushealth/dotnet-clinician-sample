namespace Clinician.ApiClients.AgencyClient
{
    public class HealthScopeItem
    {
        public string Name { get; }
        public string FriendlyName => this.Name;

        public HealthScopeItem(string name)
        {
            
            Name = name;
        }
    }
}