using System.Collections;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Exporters;
using Polly;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class ArtifactEnumerator : IEnumerator<ObjectExportInfo>
	{
		private readonly Context _context;
		private readonly IEnumerator<ObjectExportInfo> _enumerator;

		public ArtifactEnumerator(ObjectExportInfo[] artifacts, Context context)
		{
			_context = context;
			_enumerator = artifacts.Cast<ObjectExportInfo>().GetEnumerator();
		}

		public void Dispose()
		{
			_enumerator.Dispose();
		}

		public bool MoveNext()
		{
			bool moveNext = _enumerator.MoveNext();
			if (moveNext)
			{
				UpdateContext();
			}
			return moveNext;
		}

		public void Reset()
		{
			_enumerator.Reset();
		}

		private void UpdateContext()
		{
			if (Current != null)
			{
				_context[WritersRetryPolicy.CONTEXT_LAST_ARTIFACT_ID_KEY] = Current.ArtifactID;
			}
		}

		public ObjectExportInfo Current => _enumerator.Current;

		object IEnumerator.Current => Current;
	}
}