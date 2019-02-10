// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.Samples.NUnit
{
    using global::NUnit.Framework;

    using Relativity.ImportExport.UnitTestFramework;

    /// <summary>
    /// Represents a global assembly-wide setup routine that's guaranteed to be executed before ANY NUnit test.
    /// </summary>
    [SetUpFixture]
    public class AssemblySetup
    {
        /// <summary>
        /// The main setup method.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            AssemblySetupHelper.Setup();
        }

        /// <summary>
        /// The main teardown method.
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            AssemblySetupHelper.TearDown();
        }
    }
}