using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DACS_1.Model
{
    public class UserModel
    {
        public string UId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActived { get; set; }

        private string _remainingTime;
        public string RemainingTime
        {
            get => _remainingTime;
            set
            {
                _remainingTime = value;
                OnPropertyChanged(nameof(RemainingTime));
            }
        }
        public TimeSpan RemainingTimeSpan
        {
            get => TimeSpan.TryParse(RemainingTime, out var result) ? result : TimeSpan.Zero;
            set => RemainingTime = value.ToString(@"hh\:mm\:ss");
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }

}
