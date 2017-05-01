
namespace kCura.WinEDDS.Core.Model
{
	/// <summary>
	/// This class stores field artifact Id that will be used to create native file name during export process. 
	/// Can be extended to store metadata information about string formats
	/// </summary>
	/// <typeparam name="T">field artifact id</typeparam>
	public class FieldDescriptorPart : DescriptorPart<int>
	{
		public FieldDescriptorPart(int data) : base(data)
		{
		}
	}
}
