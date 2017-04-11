namespace kCura.WinEDDS.Core.Model
{
	public abstract class DescriptorPart
	{
	}

	/// <summary>
	/// This is a base class for the file name descriptors that are used to build native file name during the export process
	/// </summary>
	/// <typeparam name="TValueType">This a identifier value of the given file name part descriptor. eg: Field artifact Id or separator character</typeparam>
	public class DescriptorPart<TValueType> : DescriptorPart
	{
		public DescriptorPart(TValueType data)
		{
			Value = data;
		}

		public TValueType Value { get; protected set; }
	}
}
