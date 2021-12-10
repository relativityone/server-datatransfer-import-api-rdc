using System;
using System.Collections.Generic;
using System.Linq;

namespace FileGenerator
{
	public class RandomGen
	{
		private readonly Random _gen;
		private readonly Random _gen2;
		private readonly Dictionary<int, int> _fileSizeDistribution;
		private readonly List<int> _availableKeys;

		public RandomGen(Dictionary<string, int> fileSizeDistribution, int seed1, int seed2)
		{
			_fileSizeDistribution = fileSizeDistribution.ToDictionary(pair => int.Parse(pair.Key), pair => pair.Value);
			_availableKeys = _fileSizeDistribution.Keys.ToList();
			_gen = new Random(seed1);
			_gen2 = new Random(seed2);
		}

		public int GenerateSize()
		{
			var index = 0;

			if (_availableKeys.Count == 0)
			{
				return 0;
			}

			if (_availableKeys.Count > 1)
			{
				index = _gen.Next(0, _availableKeys.Count);
			}


			var key = _availableKeys[index];
			int remainingFileSizesForSizeRange = _fileSizeDistribution[key];

			if(remainingFileSizesForSizeRange == 1)
			{
				_fileSizeDistribution.Remove(key);
				_availableKeys.RemoveAt(index);
			}
			else
			{
				_fileSizeDistribution[_availableKeys[index]]--;
			}

			int size = _gen2.Next(key, key + key/2);
			return size;
		}

	}
	
}
