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

			var phaseJson = JsonConvert.SerializeObject (phase);
			return phase;
		}

		/// <summary>
		/// Checks the banana.
		/// </summary>
		/// <returns><c>true</c>, if banana was checked, <c>false</c> otherwise.</returns>
		/// <param name="major">Major number of the beacon.</param>
		public bool CheckBanana(int major, int minor)
		{
			if (phase == null)
				return false;

			if (major != Phase.Major)
				return false;
				
			if (phase.HiddenBanana.Found)
				return true;

			if (phase.HiddenBanana.Minor == minor) {
				phase.HiddenBanana.Found = true;

				if (PhaseComplete)
					OnPropertyChanged ("PhaseComplete");

				return true;
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

				return phase.HiddenBanana.Found;
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


		/*
		 * phase = new Phase { UUID = "B9407F30-F5F8-466E-AFF9-25556B57FE6D", Clue = new Clue {
					Image = "http://www.refractored.com/Tiburon.jpg", Message = "This town reaches south into the San Francisco Bay and also the Microsoft Office"
				},
				HiddenBanana = new Banana {
					Found = false,
					Minor = 1
				},
				Major = 255,
				Prize = new Prize{
					Image = "http://blog.xamarin.com/wp-content/uploads/2014/04/monkey-300x300.png",
					Message = "You won an amazing prize! Go claim it now!"
				}
			};*/
	}
}

