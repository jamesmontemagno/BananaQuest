using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace BananaQuest.Shared
{
	public class PhaseViewModel : INotifyPropertyChanged
	{

		const string PhaseUrl = "http://refractored.com/phase{0}.json";
		public async Task<Phase> GetPhaseAsync()
		{
			var url = string.Format (PhaseUrl, phaseNumber);
			var client = new HttpClient ();

			var result = await client.GetStringAsync (url);

			Phase =  JsonConvert.DeserializeObject<Phase> (result);
			return phase;
		}

		/// <summary>
		/// Checks the banana.
		/// </summary>
		/// <returns><c>true</c>, if banana was checked, <c>false</c> otherwise.</returns>
		/// <param name="major">Major number of the beacon.</param>
		/// <param name="minor">Minor number of the beacon.</param>
		public bool CheckBanana(int major, int minor)
		{
			if (phase == null)
				return false;

			if (major != Phase.Major)
				return false;
				
			foreach (Banana banana in Phase.HiddenBananas) {
				if (banana.Found)
					continue;

				if (banana.Minor == minor) {
					banana.Found = true;
					if (PhaseComplete)
						OnPropertyChanged ("PhaseComplete");
					return true;
				}
			}

			return false;
		}

		private Phase phase;
		public Phase Phase
		{
			get { return phase; }
			set {
				phase = value;
				OnPropertyChanged ("Phase");
			}
		}


		private int phaseNumber = 0;
		public int PhaseNumber
		{
			get { return phaseNumber; }
			set {
				phaseNumber = value;
				OnPropertyChanged ("PhaseNumber");
			}
		}


		public bool PhaseComplete
		{
			get {
				if (Phase == null)
					return false;

				foreach (Banana banana in Phase.HiddenBananas) {
					if (!banana.Found)
						return false;
				}

				return true;
			}
		}



		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null)
				return;

			PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}
	}
}

