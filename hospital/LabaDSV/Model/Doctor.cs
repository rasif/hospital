using LabaDSV.Interface;

namespace LabaDSV.Model
{
    public sealed class Doctor:Person, IDoctor
    {
        #region Constructors

        public Doctor()
        {
            IsFree = true;
        }

        #endregion

        #region Properties

        public bool IsFree
        {
            get { return _isFree; }
            set { UpdateValue(value, ref _isFree);}
        }

        public Client Client
        {
            get { return _client; }
            set { UpdateValue(value, ref _client);}
        }

        #endregion

        #region Fields

        private bool _isFree;
        private Client _client;

        #endregion
    }
}
