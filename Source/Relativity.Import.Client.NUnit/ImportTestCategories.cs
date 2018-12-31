// ----------------------------------------------------------------------------
// <copyright file="TransferTestCategories.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents all import API test categories.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class TransferTestCategories
    {
        /// <summary>
        /// The integration test category.
        /// </summary>
        public const string Integration = "Integration";

        /// <summary>
        /// The nightly test category.
        /// </summary>
        public const string Nightly = "Nightly";

        /// <summary>
        /// The unit test category.
        /// </summary>
        public const string UnitTest = "UnitTest";
    }
}