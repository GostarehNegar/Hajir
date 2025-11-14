namespace GN.Library.AI.Agents
{
    public class CaptainSquadOptions
    {
        public bool Disabled { get;set; } = false;
        public int MaxCaptains { get; set; } = 5;
        public int MaxAgentsPerCaptain { get; set; } = 5;
        public CaptainSquadOptions Validate() => this;
    }
    public class AiAgentsOptions
    {
        public CaptainSquadOptions CaptainSquad { get; set; } = new CaptainSquadOptions();
        public AiAgentsOptions Validate()
        {
            this.CaptainSquad = (this.CaptainSquad ?? new CaptainSquadOptions()).Validate();
            return this;
        }
    }
}