
using System;
using kCura.WinEDDS.Importers;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportFoldersTask : IImportFoldersTask
	{
		private readonly ImportContext _importContext;
		private readonly IFolderCache _folderCache;

		public ImportFoldersTask(ImportContext importContext, IFolderCache folderCache)
		{
			_importContext = importContext;
			_folderCache = folderCache;
		}

		public void Execute()
		{
			try
			{
				_importContext.FolderPath = string.Empty;
				_importContext.ParentFolderId = -1;  //GetFolderId();
			}
			catch (Exception ex)
			{
				//TODO:
			}
		}

		private int GetFolderId()
		{
			return _importContext.Settings.LoadFile.CreateFolderStructure ? CreateFolderStructure() : GetDafaultFolderId();
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
