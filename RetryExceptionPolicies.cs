// --------------------------------------------------------------------------------------------------------------------

// <copyright file="RetryExceptionPolicies.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
// Defines commonly used static function properties that decide whether an exception should be retried.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    /// <summary>
    /// Defines commonly used static function properties that decide whether an exception should be retried.
    /// </summary>
    public static class RetryExceptionPolicies
	{
		/// <summary>
		/// Defines a standard exception retry function that returns <see langword="true" /> for <see cref="FileInfoInvalidPathException"/> and all 
		/// <see cref="System.IO.IOException"/> and derived exception types except <see cref="System.IO.FileNotFoundException"/>, 
		/// <see cref="System.IO.PathTooLongException"/>, and disk full errors.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2211:NonConstantFieldsShouldNotBeVisible",
			Justification = "This is acceptable for defining predicates.")]
		public static Func<Exception, bool> IoStandardPolicy = exception =>
	    {
		    // Skip to preserve existing IoReporter behavior.
		    if (exception is FileInfoInvalidPathException)
		    {
			    return false;
		    }

		    // Skip to preserve existing IAPI/RDC behavior.
		    if (exception is System.IO.FileNotFoundException)
		    {
			    return false;
		    }

		    // Skip until the code base fully supports long paths.
		    if (exception is System.IO.PathTooLongException)
		    {
			    return false;
		    }

		    // Exclude all non I/O errors.
		    bool ioException = exception is System.IO.IOException;
		    if (!ioException)
		    {
			    return false;
		    }

		    // Retry all other I/O errors except the disk-full scenario.
		    const int HandleDiskFull = unchecked((int)0x80070027);
		    const int DiskFullHResult = unchecked((int)0x80070070);
		    return exception.HResult != HandleDiskFull && exception.HResult != DiskFullHResult;
	    };
    }
}