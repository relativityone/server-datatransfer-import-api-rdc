
using System;
using kCura.WinEDDS.Importers;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportFoldersTask : IImportFoldersTask
	{
		private readonly ImportContext _importCentext;
		private readonly IFolderCache _folderCache;

		public ImportFoldersTask(ImportContext importCentext, IFolderCache folderCache)
		{
			_importCentext = importCentext;
			_folderCache = folderCache;
		}

		public void Execute()
		{
			try
			{
				_importCentext.ParentFolderId = GetFolderId();
			}
			catch (Exception ex)
			{
				//TODO:
			}
		}

		private int GetFolderId()
		{
			return _importCentext.Settings.LoadFile.CreateFolderStructure ? CreateFolderStructure() : GetDafaultFolderId();
		}

		private int GetDafaultFolderId()
		{
			throw new System.NotImplementedException();
		}

		private int CreateFolderStructure()
		{
			throw new System.NotImplementedException();
		}
	}
}
