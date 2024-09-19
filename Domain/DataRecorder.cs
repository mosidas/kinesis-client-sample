
namespace Proto.Domain;

public interface IDataRecorder
{
  public void Record(IEnumerable<string> data);

}
public class DataRecorder : IDataRecorder
{
  public void Record(IEnumerable<string> data)
  {
    Console.WriteLine(data);
  }
}
