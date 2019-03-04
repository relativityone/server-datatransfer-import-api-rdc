﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="FileSystemWrapTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="FileSystemWrap"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using Castle.Components.DictionaryAdapter;

	using global::NUnit.Framework;

	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="FileSystemWrap"/> tests.
	/// </summary>
	[TestFixture]
    public class FileSystemWrapTests
    {
	    private readonly List<string> pathsForDeletion = new List<string>();
		private IFileSystem fileSystem;

		[SetUp]
	    public void Setup()
        {
            this.fileSystem = FileSystem.Instance.DeepCopy();
			this.pathsForDeletion.Clear();
		}

		[TearDown]
	    public void Teardown()
	    {
		    foreach (var path in this.pathsForDeletion)
		    {
			    try
			    {
				    if (System.IO.File.Exists(path))
				    {
					    System.IO.File.Delete(path);
				    }
				    else if (System.IO.Directory.Exists(path))
				    {
					    System.IO.Directory.Delete(path, true);
				    }
			    }
			    catch (IOException e)
			    {
				    Console.WriteLine("Failed to delete the {0} path. Exception: {1}", path, e);
			    }
		    }

		    this.pathsForDeletion.Clear();
	    }

	    [Test]
	    [Category(TestCategories.FileSystem)]
		public void ShouldGetTheNonNullDirectoryInstance()
	    {
		    Assert.That(this.fileSystem.Directory, Is.Not.Null);
	    }

	    [Test]
	    [Category(TestCategories.FileSystem)]
		public void ShouldGetTheNonNullFileInstance()
	    {
		    Assert.That(this.fileSystem.File, Is.Not.Null);
	    }

	    [Test]
	    [Category(TestCategories.FileSystem)]
		public void ShouldGetTheNonNullPathInstance()
	    {
			Assert.That(this.fileSystem.Path, Is.Not.Null);
	    }

	    [Test]
	    [Category(TestCategories.FileSystem)]
		public void ShouldCreateTheFileInfo()
		{
			string fileName = GenerateUniqueFileName();
			string file = this.GenerateUniqueFilePath(fileName);
		    IFileInfo info = this.fileSystem.CreateFileInfo(file);
			Assert.That(info, Is.Not.Null);
			Assert.That(info.Exists, Is.False);
			Assert.Throws<System.IO.FileNotFoundException>(() => Console.WriteLine(info.Length));
			Assert.That(info.FullName, Is.EqualTo(file));
			Assert.That(info.Name, Is.EqualTo(fileName));
			System.IO.File.WriteAllText(file, "Hello World");
			info.Refresh();
			Assert.That(info.Exists, Is.True);
			Assert.That(info.Length, Is.EqualTo(11));
			Assert.That(info.FullName, Is.EqualTo(file));
			Assert.That(info.Name, Is.EqualTo(fileName));
			Assert.That(this.fileSystem.File.Exists(file), Is.True);
			Assert.That(info.Directory, Is.Not.Null);
			Assert.That(info.Directory.Exists, Is.True);
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldCreateTheFile()
		{
			string fileName = GenerateUniqueFileName();
			string file = this.GenerateUniqueFilePath(fileName);
			using (this.fileSystem.File.Create(file))
			{
			}

			Assert.That(this.fileSystem.File.Exists(file), Is.True);
			Assert.That(file, Does.Exist);
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldCreateTheDirectoryInfo()
		{
			string folderName = GenerateUniqueFolderName();
			string directory = this.GenerateUniqueDirectoryPath(folderName);
			IDirectoryInfo info = this.fileSystem.CreateDirectoryInfo(directory);
			Assert.That(info, Is.Not.Null);
			Assert.That(info.Exists, Is.False);
			Assert.That(info.FullName, Is.EqualTo(directory));
			Assert.That(info.Name, Is.EqualTo(folderName));
			System.IO.Directory.CreateDirectory(directory);
			info.Refresh();
			Assert.That(info.Exists, Is.True);
			Assert.That(info.FullName, Is.EqualTo(directory));
			Assert.That(info.Name, Is.EqualTo(folderName));
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		[Category(TestCategories.FileSystem)]
		public void ShouldCreateTheDirectory(bool useDirectoryObj)
		{
			string folderName = GenerateUniqueFolderName();
			string directory = this.GenerateUniqueDirectoryPath(folderName);
			if (useDirectoryObj)
			{
				this.fileSystem.Directory.CreateDirectory(directory);
			}
			else
			{
				IDirectoryInfo info = this.fileSystem.CreateDirectoryInfo(directory);
				info.Create();
			}

			Assert.That(this.fileSystem.Directory.Exists(directory), Is.True);
			Assert.That(directory, Does.Exist);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheDirectoryParent(bool directoryExists)
		{
			string expected = GetUniqueDirectory();
			string directory = this.GenerateUniqueDirectoryPath();
			if (!directoryExists)
			{
				expected = System.IO.Path.Combine(directory, "dummy");
				directory = System.IO.Path.Combine(System.IO.Path.Combine(directory, "dummy"), "dummy");
			}

			IDirectoryInfo parent = this.fileSystem.Directory.GetParent(directory);
			Assert.That(parent, Is.Not.Null);
			expected = this.fileSystem.Path.TrimTrailingSlash(expected);
			Assert.That(parent.FullName, Is.EqualTo(expected));
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheFileSize()
		{
			string sourceFileName = GenerateUniqueFolderName();
			string sourceFile = this.GenerateUniqueFilePath(sourceFileName);
			string text = RandomHelper.NextString(100, 1000);
			System.IO.File.WriteAllText(sourceFile, text);
			long fileSize = this.fileSystem.File.GetFileSize(sourceFile);
			Assert.That(fileSize, Is.EqualTo(text.Length));
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldCountTheLinesInFile()
		{
			string sourceFileName = GenerateUniqueFolderName();
			string sourceFile = this.GenerateUniqueFilePath(sourceFileName);
			List<string> lines = new List<string>();
			int expectedLineCount = RandomHelper.NextInt(10, 1000);
			for (var i = 0; i < expectedLineCount; i++)
			{
				lines.Add(RandomHelper.NextString(5, 25).Trim());
			}

			System.IO.File.WriteAllLines(sourceFile, lines);
			int linesInFile = this.fileSystem.File.CountLinesInFile(sourceFile);
			Assert.That(linesInFile, Is.EqualTo(expectedLineCount));
		}

		[Test]
		[TestCase("\\")]
		[TestCase("/")]
		[TestCase("?")]
		[TestCase(":")]
		[TestCase("*")]
		[TestCase(">")]
		[TestCase("<")]
		[TestCase("|")]
		[TestCase("\"")]
		[Category(TestCategories.FileSystem)]
		public void ShouldConvertIllegalCharactersInFilename(string illegalCharacter)
		{
			string fileName = $"fil{illegalCharacter}ename.e{illegalCharacter}t";

			// Verify the conversion replaces using an underscore.
			string replacement1 = "_";
			string expectedWithDefault = $"fil{replacement1}ename.e{replacement1}t";
			string convertedWithDefault = this.fileSystem.Path.ConvertIllegalCharactersInFilename(fileName);
			Assert.That(convertedWithDefault, Is.EqualTo(expectedWithDefault));

			// Verify the conversion replaces using the specified .
			string replacement2 = "-";
			string expectedWithSuppliedChar = $"fil{replacement2}ename.e{replacement2}t";
			string convertedWithSuppliedChar = this.fileSystem.Path.ConvertIllegalCharactersInFilename(fileName, replacement2);
			Assert.That(convertedWithSuppliedChar, Is.EqualTo(expectedWithSuppliedChar));
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldAddAndTrimTheBackSlashes()
		{
			// UNC path
			const string OriginalUncPath = @"\\abc\def";
			string path = this.fileSystem.Path.AddTrailingBackSlash(OriginalUncPath);
			Assert.That(path, Is.EqualTo(@"\\abc\def\"));
			path = this.fileSystem.Path.AddTrailingBackSlash(OriginalUncPath);
			Assert.That(path, Is.EqualTo(@"\\abc\def\"));
			path = this.fileSystem.Path.TrimTrailingSlash(path);
			Assert.That(path, Is.EqualTo(OriginalUncPath));
			path = this.fileSystem.Path.TrimTrailingSlash(path);
			Assert.That(path, Is.EqualTo(OriginalUncPath));

			// Local path
			const string OriginalLocalPath = @"C:\Windows\System32";
			path = this.fileSystem.Path.AddTrailingBackSlash(OriginalLocalPath);
			Assert.That(path, Is.EqualTo(@"C:\Windows\System32\"));
			path = this.fileSystem.Path.AddTrailingBackSlash(OriginalLocalPath);
			Assert.That(path, Is.EqualTo(@"C:\Windows\System32\"));
			path = this.fileSystem.Path.TrimTrailingSlash(path);
			Assert.That(path, Is.EqualTo(OriginalLocalPath));
			path = this.fileSystem.Path.TrimTrailingSlash(path);
			Assert.That(path, Is.EqualTo(OriginalLocalPath));
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheDirectoryName()
		{
			// UNC path
			const string UncPath = @"\\abc\def\temp.txt";
			string name = this.fileSystem.Path.GetDirectoryName(UncPath);
			Assert.That(name, Is.EqualTo(@"\\abc\def"));

			// Local path
			const string LocalPath = @"C:\Windows\System32\temp.txt";
			name = this.fileSystem.Path.GetDirectoryName(LocalPath);
			Assert.That(name, Is.EqualTo(@"C:\Windows\System32"));
		}

		[Test]
		[TestCase(@"C:\Windows", @"C:\Windows")]
		[TestCase(@"\\kcura.corp\shares\Engineering", @"\\kcura.corp\shares\Engineering")]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheFullPath(string path, string expected)
		{
			string returnedPath = this.fileSystem.Path.GetFullPath(path);
			Assert.That(returnedPath, Is.Not.Null);
			Assert.That(returnedPath, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("https://kcura.relativity.one", "Relativity.Distributed")]
		[TestCase("https://kcura.relativity.one", "/Relativity.Distributed")]
		[TestCase("https://kcura.relativity.one", "/Relativity.Distributed/")]
		[TestCase("https://kcura.relativity.one/", "Relativity.Distributed")]
		[TestCase("https://kcura.relativity.one/", "/Relativity.Distributed")]
		[TestCase("https://kcura.relativity.one/", "/Relativity.Distributed/")]
		[TestCase("https://kcura.relativity.one/RelativityWebAPI/", "/Relativity.Distributed/")]
		[TestCase("https://kcura.relativity.one/RelativityWebAPI/", "Relativity.Distributed")]
		[TestCase("https://kcura.relativity.one/RelativityWebAPI/", "/Relativity.Distributed")]
		[TestCase("https://kcura.relativity.one/RelativityWebAPI/", "/Relativity.Distributed/")]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheFullyQualifiedPath(string basePath, string path)
		{
			string returnedPath = this.fileSystem.Path.GetFullyQualifiedPath(new Uri(basePath), path);
			Assert.That(returnedPath, Is.EqualTo("https://kcura.relativity.one/Relativity.Distributed/"));
		}

		[Test]
		[TestCase(@"C:\", "temp.txt", @"C:\temp.txt")]
		[TestCase(@"C:\Windows", "temp", @"C:\Windows\temp")]
		[TestCase(@"\\abc\def", "temp.txt", @"\\abc\def\temp.txt")]
		[TestCase(@"\\abc\def", "temp", @"\\abc\def\temp")]
		[Category(TestCategories.FileSystem)]
		public void ShouldCombineThePaths(string path1, string path2, string expected)
		{
			string path = this.fileSystem.Path.Combine(path1, path2);
			Assert.That(path, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(@"\\abc\temp.txt", @".txt")]
		[TestCase(@"\\abc\temp", "")]
		[TestCase(@"C:\Windows\temp.bin", @".bin")]
		[TestCase(@"C:\Windows\temp", "")]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheFileExtension(string path, string expected)
		{
			string extension = this.fileSystem.Path.GetExtension(path);
			Assert.That(extension, Is.EqualTo(expected));
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldChangeTheFileExtension()
		{
			string fileNameNoExt = "import-api-test";
			string oldFileName = fileNameNoExt + ".txt";
			string directory = @"C:\Windows\System32";
			string oldFile = System.IO.Path.Combine(directory, oldFileName);
			string changedFile = this.fileSystem.Path.ChangeExtension(oldFile, ".log");
			string expectedChangedFileName = fileNameNoExt + ".log";
			string expectedChangedFile = System.IO.Path.Combine(directory, expectedChangedFileName);
			Assert.That(changedFile, Is.EqualTo(expectedChangedFile));
		}

		[Test]
		[TestCase(@"C:\Windows\System32\dnsapi.dll", "dnsapi.dll")]
		[TestCase(@"C:\Windows\System32\", "")]
		[TestCase(@"C:\Windows\System32", "System32")]
		[TestCase(@"\\abc\def\dnsapi.dll", "dnsapi.dll")]
		[TestCase(@"\\abc\def\", "")]
		[TestCase(@"\\abc\def", "def")]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheFileName(string path, string expected)
		{
			string fileName = this.fileSystem.Path.GetFileName(path);
			Assert.That(fileName, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(@"http://kcura.relativity.one", true)]
		[TestCase(@"http://kcura.relativity.one/", true)]
		[TestCase(@"http://kcura.relativity.one/RelativityWebAPI", true)]
		[TestCase(@"https://kcura.relativity.one", true)]
		[TestCase(@"https://kcura.relativity.one/", true)]
		[TestCase(@"https://kcura.relativity.one/RelativityWebAPI", true)]
		[TestCase(@"\\kcura\shares\Engineering", false)]
		[TestCase(@"\\?\UNC\kcura\shares\Engineering", false)]
		[TestCase(@"C:", false)]
		[TestCase(@"C:\", false)]
		[TestCase(@"C:\Windows", false)]
		[TestCase(@"C:\Windows\", false)]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheIsPathFullyQualifiedValue(string path, bool expected)
		{
			bool fullyQualified = this.fileSystem.Path.IsPathFullyQualified(path);
			Assert.That(fullyQualified, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(@"dnsapi.dll", false, false)]
		[TestCase(@"C:", true, false)]
		[TestCase(@"C:\", true, false)]
		[TestCase(@"C:\dnsapi.dll", true, false)]
		[TestCase(@"\\abc\def", true, true)]
		[TestCase(@"\\abc\def\", true, true)]
		[TestCase(@"\\abc\def\dnsapi.dll", true, true)]
		[TestCase(@"\\?\UNC\def", true, true)]
		[TestCase(@"\\?\UNC\def\", true, true)]
		[TestCase(@"\\?\UNC\def\dnsapi.dll", true, true)]
		[Category(TestCategories.FileSystem)]
		public void ShouldGetTheIsPathRootedAndUncValues(string path, bool rootedExpected, bool uncExpected)
		{
			bool rooted = this.fileSystem.Path.IsPathRooted(path);
			Assert.That(rooted, Is.EqualTo(rootedExpected));
			bool unc = this.fileSystem.Path.IsPathUnc(path);
			Assert.That(unc, Is.EqualTo(uncExpected));
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldCopyTheFile()
		{
			string sourceFileName = GenerateUniqueFolderName();
			string sourceFile = this.GenerateUniqueFilePath(sourceFileName);
			string destinationFileName = GenerateUniqueFolderName();
			string destinationFile = System.IO.Path.Combine(GetUniqueDirectory(), destinationFileName);
			System.IO.File.WriteAllText(sourceFile, "@");
			Assert.That(sourceFile, Does.Exist);
			Assert.That(destinationFile, Does.Not.Exist);
			this.AddPathsForDeletion(destinationFile);
			this.fileSystem.File.Copy(sourceFile, destinationFile, true);
			Assert.That(sourceFile, Does.Exist);
			Assert.That(destinationFile, Does.Exist);
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldMakeReferenceDeepCopy()
		{
			IFileSystem source = this.fileSystem;
			IFileSystem deepCopy = source.DeepCopy();
			Assert.That(source, Is.Not.SameAs(deepCopy));
			Assert.That(source.Directory, Is.Not.SameAs(deepCopy.Directory));
			Assert.That(source.File, Is.Not.SameAs(deepCopy.File));
			Assert.That(source.Path, Is.Not.SameAs(deepCopy.Path));
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldDeleteTheFile()
		{
			string sourceFileName = GenerateUniqueFolderName();
			string sourceFile = this.GenerateUniqueFilePath(sourceFileName);
			System.IO.File.WriteAllText(sourceFile, "ABC123");
			Assert.That(sourceFile, Does.Exist);
			this.fileSystem.File.Delete(sourceFile);
			Assert.That(sourceFile, Does.Not.Exist);

			// Never fail when the file doesn't exist.
			this.fileSystem.File.Delete(sourceFile);
		}

		[Test]
		[Category(TestCategories.FileSystem)]
		public void ShouldCreateTheStreamWriter()
		{
			string sourceFileName = GenerateUniqueFolderName();
			string sourceFile = this.GenerateUniqueFilePath(sourceFileName);
			IStreamWriter writer = this.fileSystem.CreateStreamWriter(sourceFile, false, System.Text.Encoding.UTF8);
			Assert.That(writer, Is.Not.Null);
			Assert.That(writer.BaseStream, Is.Not.Null);
			writer.Write("Line1");
			writer.Close();
			Assert.That(writer.BaseStream, Is.Null);
			writer = this.fileSystem.CreateStreamWriter(sourceFile, true, System.Text.Encoding.UTF8);
			writer.Write(Environment.NewLine);
			writer.Write("Line2");
			writer.Close();
			Assert.That(writer.BaseStream, Is.Null);
			writer = this.fileSystem.CreateStreamWriter(sourceFile, true, System.Text.Encoding.UTF8);
			writer.Write(Environment.NewLine);
			writer.WriteLine("Line{0}", 3);
			writer.Close();
			Assert.That(writer.BaseStream, Is.Null);
			string[] lines = System.IO.File.ReadAllLines(sourceFile);
			Assert.That(lines.Length, Is.EqualTo(3));
			Assert.That(lines[0], Is.EqualTo("Line1"));
			Assert.That(lines[1], Is.EqualTo("Line2"));
			Assert.That(lines[2], Is.EqualTo("Line3"));
		}

		private static string GenerateUniqueFileName()
		{
			return Guid.NewGuid() + ".txt";
		}

		private static string GenerateUniqueFolderName()
		{
			return $"reldir-{Guid.NewGuid()}";
		}

		private static string GetUniqueDirectory()
		{
			return System.IO.Path.GetTempPath();
		}

		private void AddPathsForDeletion(string path)
		{
			this.pathsForDeletion.Add(path);
		}

		private string GenerateUniqueDirectoryPath()
		{
			return this.GenerateUniqueDirectoryPath(GenerateUniqueFolderName());
		}

		private string GenerateUniqueDirectoryPath(string folder)
		{
			string file = System.IO.Path.Combine(GetUniqueDirectory(), folder);
			this.AddPathsForDeletion(file);
			return file;
		}

		private string GenerateUniqueFilePath(string fileName)
		{
			string file = System.IO.Path.Combine(GetUniqueDirectory(), fileName);
			this.AddPathsForDeletion(file);
			return file;
		}
	}
}