using System;
namespace Gametest
{
	public class Semantic
	{
		public uint Offset
		{
			get
			{
				return _offset;
			}
		}		
		public string SourceName
		{
			get 
			{
				return _sourceName;
			}
		}
		uint _offset;
		string _sourceName;
		public Semantic(string pSourceName, uint pOffset)
		{
			_offset = pOffset;
			_sourceName = pSourceName;
		}
	}
}
