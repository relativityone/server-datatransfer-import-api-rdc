// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSharePathManager.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines the core file transfer class object to support native files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a file share path manager class object.
    /// </summary>
    public class FileSharePathManager
    {
        /// <summary>
        /// The default volume size [1000].
        /// </summary>
        public const int DefaultVolumeSize = 1000;

        /// <summary>
        /// The maximum volume size backing.
        /// </summary>
        private readonly int maxVolumeSize;

        /// <summary>
        /// The current sub-directory backing.
        /// </summary>
        private string currentSubDirectory;

        /// <summary>
        /// The last sub-directory backing.
        /// </summary>
        private string lastSubdirectory;

        /// <summary>
        /// The current file number backing.
        /// </summary>
        private int currentFileNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSharePathManager"/> class.
        /// </summary>
        /// <param name="maximumVolumeSize">
        /// Maximum size of the volume.
        /// </param>
        public FileSharePathManager(int maximumVolumeSize)
        {
            this.currentSubDirectory = this.GetNewSubdirectory();
            this.lastSubdirectory = string.Copy(this.currentSubDirectory);
            this.currentFileNumber = 0;
            this.maxVolumeSize = Math.Max(DefaultVolumeSize, maximumVolumeSize);
        }

        /// <summary>
        /// Gets the name of the current target folder.
        /// </summary>
        /// <value>
        /// The folder name.
        /// </value>
        public string CurrentTargetFolderName => this.currentSubDirectory;

        /// <summary>
        /// Gets the maximum size of the volume.
        /// </summary>
        /// <value>
        /// The maximum size of the volume.
        /// </value>
        public int MaxVolumeSize => this.maxVolumeSize;

        /// <summary>
        /// Rollback the current file number and update the sub-directory value.
        /// </summary>
        public void Rollback()
        {
            if (this.currentFileNumber == 1)
            {
                this.currentFileNumber = this.maxVolumeSize;
                this.currentSubDirectory = string.Copy(this.lastSubdirectory);
            }
            else
            {
                this.currentFileNumber--;
            }
        }

        /// <summary>
        /// Gets the next target path.
        /// </summary>
        /// <param name="targetFolderName">
        /// The target folder name.
        /// </param>
        /// <returns>
        /// The target path.
        /// </returns>
        public string GetNextTargetPath(string targetFolderName)
        {
            this.currentFileNumber++;
            if (this.currentFileNumber > this.maxVolumeSize)
            {
                this.currentFileNumber = 1;
                this.lastSubdirectory = string.Copy(this.currentSubDirectory);
                this.currentSubDirectory = this.GetNewSubdirectory();
            }

            return Path.Combine(targetFolderName, this.currentSubDirectory) + "\\";
        }

        /// <summary>
        /// Gets the new sub-directory.
        /// </summary>
        /// <returns>
        /// The sub-directory.
        /// </returns>
        private string GetNewSubdirectory()
        {
            return "RV_" + Guid.NewGuid();
        }

        /// <summary>
        /// Gets the new sub-directory.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The new sub-directory.
        /// </returns>
        public string GetNewSubdirectory(string path)
        {
            return Path.Combine(path, this.GetNewSubdirectory()) + "\\";
        }
    }
}