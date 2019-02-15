// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
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
	    /// Gets the data transfer test parameters for all tests within the current assembly.
	    /// </summary>
	    /// <value>
	    /// The <see cref="DtxTestParameters"/> instance.
	    /// </value>
	    public static DtxTestParameters GlobalDtxTestParameters
	    {
		    get;
		    private set;
	    }

		/// <summary>
		/// The main setup method.
		/// </summary>
		[OneTimeSetUp]
        public void Setup()
        {
            GlobalDtxTestParameters = AssemblySetupHelper.Setup();
        }

        /// <summary>
        /// The main teardown method.
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            AssemblySetupHelper.TearDown(GlobalDtxTestParameters);
        }
    }
}