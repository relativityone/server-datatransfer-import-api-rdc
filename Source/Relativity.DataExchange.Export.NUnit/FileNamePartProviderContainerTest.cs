// -----------------------------------------------------------------------------------------------------
// <copyright file="FileNamePartProviderContainerTest.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

	using FileNaming.CustomFileNaming;

	using global::NUnit.Framework;

	using kCura.WinEDDS.FileNaming.CustomFileNaming;

	public class FileNamePartProviderContainerTest
	{
		private FileNamePartProviderContainer _subjectUnderTest;

		[Test]
		public void ItShouldThrowExceptionForNotSupportedDescriptorType()
		{
			this._subjectUnderTest = new FileNamePartProviderContainer();

			Assert.Throws<ArgumentOutOfRangeException>(() => this._subjectUnderTest.GetProvider(new HelperDescriptorBasePart()));
		}

		[Test]
		public void ItShouldReturnFieldDescriptorPartProvider()
		{
			var part = new FieldDescriptorPart(1);

			this._subjectUnderTest = new FileNamePartProviderContainer();

			IFileNamePartProvider retProvider = this._subjectUnderTest.GetProvider(part);
			Assert.That(retProvider, Is.TypeOf<FieldFileNamePartProvider>());
		}

		[Test]
		public void ItShouldReturnSeparatorDescriptorPartProvider()
		{
			var part = new SeparatorDescriptorPart(string.Empty);

			this._subjectUnderTest = new FileNamePartProviderContainer();

			IFileNamePartProvider retProvider = this._subjectUnderTest.GetProvider(part);
			Assert.That(retProvider, Is.TypeOf<SeparatorFileNamePartProvider>());
		}

		[Test]
		public void ItShouldReturnCustomTextDescriptorPartProvider()
		{
			var part = new CustomTextDescriptorPart("Custom Text");

			this._subjectUnderTest = new FileNamePartProviderContainer();

			IFileNamePartProvider retProvider = this._subjectUnderTest.GetProvider(part);
			Assert.That(retProvider, Is.TypeOf<CustomTextFileNamePartProvider>());
		}

		private class HelperDescriptorBasePart : DescriptorPart
		{
		}
	}
}
