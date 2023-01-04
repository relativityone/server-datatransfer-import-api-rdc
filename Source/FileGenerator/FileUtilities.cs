using System;
using System.IO;
using System.Text;

namespace FileGenerator
{
	public class FileUtilities
	{
		private readonly string _tempString;
		private readonly int _tempStringSize;

		public FileUtilities()
		{
			_tempString = $@"{Guid.NewGuid()}Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla arcu turpis, dapibus in convallis nec, laoreet eu sem. Proin id leo turpis. Vestibulum dapibus lectus et justo efficitur lobortis. Pellentesque facilisis, felis vitae scelerisque blandit, enim augue condimentum neque, a posuere enim lacus ut orci. Aenean interdum sagittis nulla, id congue urna. Integer dui nulla, pellentesque in orci vitae, luctus posuere tellus. Vestibulum quam urna, gravida et eleifend ut, posuere ac risus. Sed auctor ultricies imperdiet. Sed a nisl finibus, luctus arcu ac, convallis ipsum. Donec fringilla elit nisl, vel ullamcorper dolor egestas id. Mauris ac mi et augue vehicula gravida. Integer laoreet libero non condimentum fringilla. Suspendisse convallis feugiat neque at pellentesque.
Sed diam ipsum, ultrices nec enim id, lacinia posuere mi. Integer mattis, mauris id condimentum laoreet, massa urna hendrerit mauris, vel mattis lectus ligula eget massa. Integer gravida quam sit amet iaculis elementum. In tempus purus nec metus aliquet luctus. Nam id pretium urna. Aenean id laoreet dui. Praesent dolor velit, vestibulum sed tortor quis, elementum ornare neque. Aliquam erat volutpat. Nullam a mi sem. Etiam ac nulla aliquam, rhoncus mi sit amet, iaculis sem. Praesent volutpat sagittis nisl, eu tempus sem iaculis eget. Cras ullamcorper enim ornare lorem consectetur tincidunt. Pellentesque a risus tempus, dignissim velit et, lacinia lacus. Integer eu volutpat mauris, quis gravida erat. Sed elementum condimentum risus, sed semper augue tempus quis. Suspendisse quis faucibus metus, non venenatis lacus.
Nam tempor risus non ante consequat, eu suscipit enim tristique. Donec sapien velit, porttitor at accumsan vitae, sollicitudin vitae justo. Morbi facilisis odio vel est sollicitudin molestie. Donec pulvinar ut lorem id molestie. Nulla gravida neque id leo volutpat sodales. Vivamus at mauris condimentum, consectetur tortor vitae, condimentum elit. Nam eget magna magna. Donec est lorem, blandit in neque ac, elementum molestie massa.
In rhoncus massa non lorem aliquet, sed laoreet nibh tincidunt. Donec et est pellentesque risus condimentum aliquet aliquam at quam. Morbi blandit leo nec dui aliquam, eget molestie nibh sollicitudin. Integer mi diam, consectetur eu cursus in, egestas at mauris. Aliquam congue dictum ex eu dignissim. Duis ac porttitor enim. Ut eu lacus quis mauris ornare finibus quis et diam. Nulla mauris tellus, hendrerit in suscipit at, facilisis vel lorem.
Nunc dictum mi sed neque accumsan rhoncus. Donec laoreet elit vel tortor commodo ultricies. Suspendisse cursus et nulla nec posuere. Sed nulla erat, tristique ac felis in, viverra consequat neque. Duis consectetur vehicula dui a faucibus. Quisque vel nibh velit. Sed eleifend magna sed ultrices mollis. Nulla vulputate commodo massa at mattis. Ut hendrerit sapien velit, a elementum lectus luctus in. Cras feugiat ex nulla, sit amet convallis lacus iaculis ac. Suspendisse consectetur urna ligula, a scelerisque tortor porta eu. Pellentesque in ante ut diam vulputate vestibulum. Pellentesque at ultrices nulla.
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla arcu turpis, dapibus in convallis nec, laoreet eu sem. Proin id leo turpis. Vestibulum dapibus lectus et justo efficitur lobortis. Pellentesque facilisis, felis vitae scelerisque blandit, enim augue condimentum neque, a posuere enim lacus ut orci. Aenean interdum sagittis nulla, id congue urna. Integer dui nulla, pellentesque in orci vitae, luctus posuere tellus. Vestibulum quam urna, gravida et eleifend ut, posuere ac risus. Sed auctor ultricies imperdiet. Sed a nisl finibus, luctus arcu ac, convallis ipsum. Donec fringilla elit nisl, vel ullamcorper dolor egestas id. Mauris ac mi et augue vehicula gravida. Integer laoreet libero non condimentum fringilla. Suspendisse convallis feugiat neque at pellentesque.
Sed diam ipsum, ultrices nec enim id, lacinia posuere mi. Integer mattis, mauris id condimentum laoreet, massa urna hendrerit mauris, vel mattis lectus ligula eget massa. Integer gravida quam sit amet iaculis elementum. In tempus purus nec metus aliquet luctus. Nam id pretium urna. Aenean id laoreet dui. Praesent dolor velit, vestibulum sed tortor quis, elementum ornare neque. Aliquam erat volutpat. Nullam a mi sem. Etiam ac nulla aliquam, rhoncus mi sit amet, iaculis sem. Praesent volutpat sagittis nisl, eu tempus sem iaculis eget. Cras ullamcorper enim ornare lorem consectetur tincidunt. Pellentesque a risus tempus, dignissim velit et, lacinia lacus. Integer eu volutpat mauris, quis gravida erat. Sed elementum condimentum risus, sed semper augue tempus quis. Suspendisse quis faucibus metus, non venenatis lacus.
Nam tempor risus non ante consequat, eu suscipit enim tristique. Donec sapien velit, porttitor at accumsan vitae, sollicitudin vitae justo. Morbi facilisis odio vel est sollicitudin molestie. Donec pulvinar ut lorem id molestie. Nulla gravida neque id leo volutpat sodales. Vivamus at mauris condimentum, consectetur tortor vitae, condimentum elit. Nam eget magna magna. Donec est lorem, blandit in neque ac, elementum molestie massa.
In rhoncus massa non lorem aliquet, sed laoreet nibh tincidunt. Donec et est pellentesque risus condimentum aliquet aliquam at quam. Morbi blandit leo nec dui aliquam, eget molestie nibh sollicitudin. Integer mi diam, consectetur eu cursus in, egestas at mauris. Aliquam congue dictum ex eu dignissim. Duis ac porttitor enim. Ut eu lacus quis mauris ornare finibus quis et diam. Nulla mauris tellus, hendrerit in suscipit at, facilisis vel lorem.
Nunc dictum mi sed neque accumsan rhoncus. Donec laoreet elit vel tortor commodo ultricies. Suspendisse cursus et nulla nec posuere. Sed nulla erat, tristique ac felis in, viverra consequat neque. Duis consectetur vehicula dui a faucibus. Quisque vel nibh velit. Sed eleifend magna sed ultrices mollis. Nulla vulputate commodo massa at mattis. Ut hendrerit sapien velit, a elementum lectus luctus in. Cras feugiat ex nulla, sit amet convallis lacus iaculis ac. Suspendisse consectetur urna ligula, a scelerisque tortor porta eu. Pellentesque in ante ut diam vulputate vestibulum. Pellentesque at ultrices nulla.
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla arcu turpis, dapibus in convallis nec, laoreet eu sem. Proin id leo turpis. Vestibulum dapibus lectus et justo efficitur lobortis. Pellentesque facilisis, felis vitae scelerisque blandit, enim augue condimentum neque, a posuere enim lacus ut orci. Aenean interdum sagittis nulla, id congue urna. Integer dui nulla, pellentesque in orci vitae, luctus posuere tellus. Vestibulum quam urna, gravida et eleifend ut, posuere ac risus. Sed auctor ultricies imperdiet. Sed a nisl finibus, luctus arcu ac, convallis ipsum. Donec fringilla elit nisl, vel ullamcorper dolor egestas id. Mauris ac mi et augue vehicula gravida. Integer laoreet libero non condimentum fringilla. Suspendisse convallis feugiat neque at pellentesque.
Sed diam ipsum, ultrices nec enim id, lacinia posuere mi. Integer mattis, mauris id condimentum laoreet, massa urna hendrerit mauris, vel mattis lectus ligula eget massa. Integer gravida quam sit amet iaculis elementum. In tempus purus nec metus aliquet luctus. Nam id pretium urna. Aenean id laoreet dui. Praesent dolor velit, vestibulum sed tortor quis, elementum ornare neque. Aliquam erat volutpat. Nullam a mi sem. Etiam ac nulla aliquam, rhoncus mi sit amet, iaculis sem. Praesent volutpat sagittis nisl, eu tempus sem iaculis eget. Cras ullamcorper enim ornare lorem consectetur tincidunt. Pellentesque a risus tempus, dignissim velit et, lacinia lacus. Integer eu volutpat mauris, quis gravida erat. Sed elementum condimentum risus, sed semper augue tempus quis. Suspendisse quis faucibus metus, non venenatis lacus.
Nam tempor risus non ante consequat, eu suscipit enim tristique. Donec sapien velit, porttitor at accumsan vitae, sollicitudin vitae justo. Morbi facilisis odio vel est sollicitudin molestie. Donec pulvinar ut lorem id molestie. Nulla gravida neque id leo volutpat sodales. Vivamus at mauris condimentum, consectetur tortor vitae, condimentum elit. Nam eget magna magna. Donec est lorem, blandit in neque ac, elementum molestie massa.
In rhoncus massa non lorem aliquet, sed laoreet nibh tincidunt. Donec et est pellentesque risus condimentum aliquet aliquam at quam. Morbi blandit leo nec dui aliquam, eget molestie nibh sollicitudin. Integer mi diam, consectetur eu cursus in, egestas at mauris. Aliquam congue dictum ex eu dignissim. Duis ac porttitor enim. Ut eu lacus quis mauris ornare finibus quis et diam. Nulla mauris tellus, hendrerit in suscipit at, facilisis vel lorem.
Nunc dictum mi sed neque accumsan rhoncus. Donec laoreet elit vel tortor commodo ultricies. Suspendisse cursus et nulla nec posuere. Sed nulla erat, tristique ac felis in, viverra consequat neque. Duis consectetur vehicula dui a faucibus. Quisque vel nibh velit. Sed eleifend magna sed ultrices mollis. Nulla vulputate commodo massa at mattis. Ut hendrerit sapien velit, a elementum lectus luctus in. Cras feugiat ex nulla, sit amet convallis lacus iaculis ac. Suspendisse consectetur urna ligula, a scelerisque tortor porta eu. Pellentesque in ante ut diam vulputate vestibulum. Pellentesque at ultrices nulla.
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla arcu turpis, dapibus in convallis nec, laoreet eu sem. Proin id leo turpis. Vestibulum dapibus lectus et justo efficitur lobortis. Pellentesque facilisis, felis vitae scelerisque blandit, enim augue condimentum neque, a posuere enim lacus ut orci. Aenean interdum sagittis nulla, id congue urna. Integer dui nulla, pellentesque in orci vitae, luctus posuere tellus. Vestibulum quam urna, gravida et eleifend ut, posuere ac risus. Sed auctor ultricies imperdiet. Sed a nisl finibus, luctus arcu ac, convallis ipsum. Donec fringilla elit nisl, vel ullamcorper dolor egestas id. Mauris ac mi et augue vehicula gravida. Int
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla arcu turpis, dapibus in convallis nec, laoreet eu sem. Proin id leo turpis. Vestibulum dapibus lectus et justo efficitur lobortis. Pellente
{new string('a',10000000)}";


			_tempStringSize = _tempString.Length;
		}

		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}

		public void CreateFile(string path, string fileName, FileSize size)
		{
			string pathString = Path.Combine(path, fileName);
			var utf8Encoding = new UTF8Encoding();

			using (FileStream fs = File.Create(pathString))
			{
				if (size.ByteCount < _tempStringSize)
				{
					fs.Write(utf8Encoding.GetBytes(_tempString.ToCharArray(), 0, (int)size.ByteCount), 0, (int)size.ByteCount);
				}
				else
				{
					long blockSize = size.ByteCount / _tempStringSize;
					for (int i = 0; i < blockSize; i++)
					{
						fs.Write(utf8Encoding.GetBytes(_tempString), 0, utf8Encoding.GetByteCount(_tempString));
					}
					fs.Write(utf8Encoding.GetBytes(_tempString.ToCharArray(), 0, (int)size.ByteCount% _tempStringSize), 0, (int)size.ByteCount % _tempStringSize);
				}
			}
		}

		public FileStream CreateLoadFile(string path, string fileName)
		{
			CreateDirectory(path);
			string pathString = Path.Combine(path, fileName);
			return File.Create(pathString);
		}

		public void CloseStream(FileStream fs)
		{
			fs.Dispose();
		}

		public void WriteToLoadFile(FileStream fs, string line)
		{
			var utf8Encoding = new UTF8Encoding();
			fs.Write(utf8Encoding.GetBytes(line),0, line.Length);
		}

		public readonly struct FileSize
		{
			public int Size { get; }
			public Unit Unit { get; }

			public FileSize(int size, Unit unit)
			{
				Size = size;
				Unit = unit;
			}

			public long ByteCount => Size * (long)Unit;
		}

		public enum Unit
		{
			B = 1,
			KB = 1024,
			MB = 1024 * 1024,
			GB = 1024 * 1024 * 1024
		}
	}
}