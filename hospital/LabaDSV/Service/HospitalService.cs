using System;
using LabaDSV.Model;

namespace LabaDSV.Service
{
    public class HospitalService
    {
        #region Methods

        public Client GetClient()
        {
            var random = new Random();

            var gender = random.Next(0, 1000) % 2 == 0 ? "Man" : "Woman";

            var isSick = (random.Next(0, 1000) % 2 == 0);

            var name = GetRandomName(gender);
            var surname = GetRandomSurname(gender);

            return new Client(name, surname, gender, isSick);
        }

        public Doctor GetDoctor()
        {
            return new Doctor();
        }

        private string GetRandomName(string gender)
        {
            var random = new Random();

            if (gender == "Man")
                return BoyNames[random.Next(0, BoyNames.Length - 1)];

            return GirlNames[random.Next(0, GirlNames.Length - 1)];
        }

        private string GetRandomSurname(string gender)
        {
            var random = new Random();

            if (gender == "Man")
                return BoySunames[random.Next(0, BoySunames.Length - 1)];

            return GirlSunames[random.Next(0, GirlSunames.Length - 1)];
        }

        #endregion

        #region Fields

        private static readonly string[] GirlNames = {"Ксения", "Лена", "Алена", "Мария", "Айгюнь", "Арзу", "Айсель"};
        private static readonly string[] GirlSunames = { "Новикова", "Сидрова", "Пидрова", "Петрова", "Антова", "Ашотова", "Смирнова" };
        private static readonly string[] BoyNames = { "Миша", "Саша", "Никита", "Орхан", "Женя", "Костя", "Паша " };
        private static readonly string[] BoySunames = { "Кондратьев", "Новиков", "Алексеев", "Гулян", "Юдаев", "Петров", "Ашотов" };

        #endregion


    }
}
