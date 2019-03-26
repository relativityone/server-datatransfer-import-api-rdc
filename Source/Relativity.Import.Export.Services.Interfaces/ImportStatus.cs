﻿namespace Relativity.Import.Export.Services
{
	using System;

	[Flags]
	public enum ImportStatus : long
	{

		Pending = 0,

		ErrorOverwrite = 2,

		ErrorAppend = 4,

		ErrorRedaction = 8,

		ErrorBates = 16,

		ErrorImageCountMismatch = 32,

		ErrorDocumentInProduction = 64,

		NoImageSpecifiedOnLine = 128,

		FileSpecifiedDne = 256,

		InvalidImageFormat = 512,

		ColumnMismatch = 1024,

		EmptyFile = 2048,

		EmptyIdentifier = 4096,

		IdentifierOverlap = 8192,

		SecurityUpdate = 16384,

		SecurityAdd = 32768,

		ErrorOriginalInProduction = 65536,

		ErrorAppendNoParent = 131072,

		ErrorDuplicateAssociatedObject = 262144,

		SecurityAddAssociatedObject = 524288,

		ErrorAssociatedObjectIsChild = 1048576,

		ErrorAssociatedObjectIsDocument = 2097152,

		ErrorOverwriteMultipleKey = 4194304,

		ErrorTags = 8388608,

		ErrorAssociatedObjectIsMissing = 16777216,

		DataGridInvalidDocumentIDError = 33554432,

		DataGridFieldMaxSizeExceeded = 67108864,

		DataGridInvalidFieldNameError = 134217728,

		DataGridExceptionOccurred = 268435456,
	}
}