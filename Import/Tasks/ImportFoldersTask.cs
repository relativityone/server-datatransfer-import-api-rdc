
using System;
using kCura.WinEDDS.Importers;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportFoldersTask : IImportFoldersTask
	{
		private readonly IFolderCache _folderCache;

		public ImportFoldersTask(IFolderCache folderCache)
		{
			_folderCache = folderCache;
		}

		public void Execute(ImportBatchContext importBatchContext)
		{
			try
			{
				importBatchContext.ImportContext.FolderPath = string.Empty;
				importBatchContext.ImportContext.ParentFolderId = -1;  //GetFolderId();
			}
			catch (Exception ex)
			{
				//TODO:
			}
		}

		private int GetFolderId(ImportContext importContext)
		{
			return importContext.Settings.LoadFile.CreateFolderStructure ? CreateFolderStructure() : GetDafaultFolderId();
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
