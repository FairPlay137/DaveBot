namespace DaveBot.Services
{
    public interface IServerConfiguration
    {
        

        bool ReloadConfig(bool verbose);
        bool SaveConfig(bool verbose);
    }
}
