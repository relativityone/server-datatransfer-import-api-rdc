﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Transfer;

	public interface IProgressHandler
	{
		void Subscribe(ITapiBridge tapiBridge);
		void Unsubscribe(ITapiBridge tapiBridge);
	}
}