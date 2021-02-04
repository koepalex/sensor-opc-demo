
namespace sensor_opc_server.Interfaces
{
    using System.Threading.Tasks;
    public interface ICommandLineArgumentParser
    {
        /// <summary>
        /// Analyze the command line arguments and fillup settings
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <returns>true if command line arguments are valid; otherwise true</returns>
        Task<bool> Parse(string[] args);
    }
}