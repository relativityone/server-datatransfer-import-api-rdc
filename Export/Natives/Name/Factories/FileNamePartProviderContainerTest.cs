using System;
using kCura.WinEDDS.Core.Export.Natives.Name;
using kCura.WinEDDS.Core.Export.Natives.Name.Factories;
using kCura.WinEDDS.Core.Model;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name.Factories
{
	public class FileNamePartProviderContainerTest
	{
		private class HelperDescriptorBasePart : DescriptorPart
		{
		}

		private FileNamePartProviderContainer _subjectUnderTest;

		[Test]
		public void ItShouldThrowExceptionForNotSupportedDescriptorType()
		{
			_subjectUnderTest = new FileNamePartProviderContainer();

			Assert.Throws<Exception>(() => _subjectUnderTest.GetProvider(new HelperDescriptorBasePart()));
		}

		[Test]
		public void ItShouldReturnFieldDescriptorPartProvider()
		{
			var part = new FieldDescriptorPart(1);

			_subjectUnderTest = new FileNamePartProviderContainer();

			IFileNamePartProvider retProvider = _subjectUnderTest.GetProvider(part);
			Assert.That(retProvider, Is.TypeOf<FieldFileNamePartProvider>());
		}

		[Test]
		public void ItShouldReturnSeparatorDescriptorPartProvider()
		{
			var part = new SeparatorDescriptorPart("");

			_subjectUnderTest = new FileNamePartProviderContainer();

			IFileNamePartProvider retProvider = _subjectUnderTest.GetProvider(part);
			Assert.That(retProvider, Is.TypeOf<SeparatorFileNamePartProvider>());
		}
	}
}
