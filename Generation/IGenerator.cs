using System.Threading.Tasks;

namespace Generation
{
    public interface IGenerator
    {
        Task Generate(string filePath, int rowCount);
    }
}