

namespace kCura.WinEDDS.Core.Model
{
	/// <summary>
	/// This class stores separator that will be used to create native file name during export process. 
	/// </summary>
	/// <typeparam name="T">separator</typeparam>
	public class SeparatorDescriptorPart : DescriptorPart<string>
	{
		public SeparatorDescriptorPart(string value) : base(value)
		{
		}
	}
}
