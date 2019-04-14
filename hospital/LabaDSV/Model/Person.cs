using LabaDSV.Helpers;
using LabaDSV.Interface;

namespace LabaDSV.Model
{
    public abstract class Person:ViewModelBase, IPerson
    {
        #region Properties

        public string Name
        {
            get { return _name; }
            set { UpdateValue(value, ref _name);}
        }

        public string Surname
        {
            get { return _surname; }
            set { UpdateValue(value, ref _surname);}
        }

        public string Gender
        {
            get { return _gender; }
            set { UpdateValue(value, ref _gender);}
        }

        #endregion

        #region Fields

        private string _name;
        private string _surname;
        private string _gender;

        #endregion
    }
}
