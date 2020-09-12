using System.Threading.Tasks;

namespace Generation.Generator
{
    public interface IGenerator
    {
        Task Generate(string filePath, int rowCount);
    }
}