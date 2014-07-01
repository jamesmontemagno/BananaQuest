using System;
using System.Collections.Generic;

namespace BananaQuest.Shared
{
	public class Phase
	{
		public List<Banana> HiddenBananas { get; set;}
		public string UUID {get;set;}
		public int Major {get;set;}
		public Clue Clue {get;set;}
		public Prize Prize {get;set;}
	}
}

