using LabaDSV.Interface;

namespace LabaDSV.Model
{
    public sealed class Client:Person, IClient
    {
        #region Constructors

        static Client()
        {
            _count = 0;
        }

        public Client(string name, string surname, string gender, bool isSick)
        {
            _count++;

            Name = name;
            Surname = surname;
            Gender = gender;
            Number = _count;
            IsSick = isSick;
        }

        #endregion

        #region Properties

        public int Number { get; }

        public bool IsSick
        {
            get { return _isSick;}
            set { UpdateValue(value, ref _isSick);}
        }

        #endregion

        #region Fields

        private static int _count;
        private bool _isSick;

        #endregion
    }
}
