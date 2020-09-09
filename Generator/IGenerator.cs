using System.Threading.Tasks;

namespace Generator
{
    public interface IGenerator
    {
        Task Generate(string filePath, int rowCount);
    }
}