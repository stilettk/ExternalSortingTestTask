using System.Threading.Tasks;

namespace Generation.Generator
{
    public interface IGenerator
    {
        Task GenerateAsync(string filePath, int rowCount);
    }
}