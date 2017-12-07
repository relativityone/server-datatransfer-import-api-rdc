using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchState : IBatchState
	{
		private readonly IList<IStateful> _statefulComponents;

		public BatchState(IList<IStateful> statefulComponents)
		{
			_statefulComponents = statefulComponents;
		}

		public void SaveState()
		{
			foreach (var statefulComponent in _statefulComponents)
			{
				statefulComponent.SaveState();
			}
		}

		public void RestoreState()
		{
			foreach (var statefulComponent in _statefulComponents)
			{
				statefulComponent.RestoreLastState();
			}
		}
	}
}