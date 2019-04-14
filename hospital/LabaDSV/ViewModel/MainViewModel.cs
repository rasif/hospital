using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LabaDSV.Helpers;
using LabaDSV.Model;
using LabaDSV.Service;

namespace LabaDSV.ViewModel
{
    public sealed class MainViewModel:ViewModelBase
    {
        #region Constructors

        public MainViewModel(ICommandFactory commandFactory, HospitalService hospitalService)
        {
            if (commandFactory == null)
                return;

            if (hospitalService == null)
                return;

            _commandFactory = commandFactory;
            _hospitalService = hospitalService;

            ConfigureCommands();

            Clients = new ObservableCollection<Client>();
            ClientsQueue = new ObservableCollection<Client>();
            Doctors = new List<Doctor>();

            _isClientSick = -1; 

            CreateDoctorsAsync();

            var clientTimer = new DispatcherTimer();
            clientTimer.Interval = new TimeSpan(0, 0, 0, 3);
            clientTimer.Tick += AddClientInQueueTimer;
            clientTimer.Start();

            var clientTurnTimer = new DispatcherTimer();
            clientTurnTimer.Interval = new TimeSpan(0,0,0,0,3500);
            clientTurnTimer.Tick += AddClientToObservation;
            clientTurnTimer.Start();
        }

        #endregion

        #region Commands

        private void ConfigureCommands()
        {
            MoveWindowCommand = _commandFactory.CreateCommand<Window>(OnMoveWindowCommand);
            CloseWindowCommand = _commandFactory.CreateCommand<Window>(OnCloseWindowCommand);
            CollapseWindowCommand = _commandFactory.CreateCommand<Window>(OnCollapseWindowCommand);
        }

        public ICommand MoveWindowCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand CollapseWindowCommand { get; set; }

        #endregion

        #region Methods for commands

        private void OnMoveWindowCommand(Window window)
        {
            window.DragMove();
        }

        private void OnCloseWindowCommand(Window window)
        {
            window.Close();
        }

        private void OnCollapseWindowCommand(Window window)
        {
            window.WindowState = WindowState.Minimized;
        }

        #endregion

        #region Other methods

        private void AddClientInQueueTimer(object sender, EventArgs e) 
        {
            var client = _hospitalService.GetClient();

            if (client.IsSick)
                InfectOtherClientsAsync();

            Clients.Add(client);

            WriteInBookAboutClientAsync(client, "очередь");

            CheckClientAtDoctorAsync();

            AddClientToFreeDoctor();
        }

        private void AddClientToObservation(object sender, EventArgs e) 
        {
            if (ClientsQueue.Count >= 4 || Clients.Count == 0) 
                return;

            foreach (var client in Clients)
            {
                var isClientBannedEnter = client.IsSick && _isClientSick == 0 || 
                                         !client.IsSick && _isClientSick == 1;

                if (!isClientBannedEnter)
                {
                    ClientsQueue.Add(client);
                    CheckSickingInObservationAsync();
                    Clients.Remove(client);
                    WriteInBookAboutClientAsync(client, "смотровую");
                    break;
                }               
            }
        }

        private async void InfectOtherClientsAsync() 
        {
            await InfectOtherClients();
        }

        private Task InfectOtherClients() 
        {
            return Task.Run(() =>
            {
                Thread.Sleep(10000);

                var isSick = IsExistSickingClientInQueue();

                if (!isSick)
                    return;

                foreach (var client in Clients)
                    client.IsSick = true;

                _eventsInHospitalText += $"Все пациенты заразились в очереди\n";
            });
        }

        private bool IsExistSickingClientInQueue() 
        {
            return Clients.Any(client => client.IsSick);
        }

        private void AddClientToFreeDoctor() 
        {
            var isClientsNoExist = ClientsQueue.Count <= 0;
            if (isClientsNoExist)
                return;

            foreach (var doctor in Doctors)
            {
                if (doctor.IsFree && ClientsQueue.Count > 0)
                {
                    var client = ClientsQueue[0];
                    doctor.Client = client;
                    ClientsQueue.Remove(client);
                    doctor.IsFree = false;
                    CheckSickingInObservation();
                    _eventsInHospitalText += $"Врач принял клиента №{client.Number}\n";
                }
            }
        }

        private async void CheckClientAtDoctorAsync() 
        {
            await CheckClientAtDoctor();
        }

        private Task CheckClientAtDoctor() 
        {
            return Task.Run(() =>
            {
                foreach (var doctor in Doctors)
                {
                    if (doctor.IsFree == false)
                    {
                        var random = new Random();
                        Thread.Sleep(random.Next(500,10000));
                        doctor.IsFree = true;
                    }
                }
            }); 
        }

        private async void WriteInBookAboutClientAsync(Client client, string where) 
        {
            await WriteInBookAboutClient(client, where);
        }

        private Task WriteInBookAboutClient(Client client, string where) 
        {
            return Task.Run(() =>
            {
                if (client.IsSick)
                    EventsInHospitalText += $"Добавлен в {where} больной  №{client.Number}\n";
                else
                    EventsInHospitalText += $"Добавлен в {where} здоровый  №{client.Number}\n";
            });
        }

        private async void CheckSickingInObservationAsync() 
        {
            await CheckSickingInObservation();
        }

        private Task CheckSickingInObservation() 
        {
            return Task.Run(() =>
            {
                if (ClientsQueue.Count == 0)
                    _isClientSick = -1;
                else
                {
                    foreach (var client in ClientsQueue)
                    {
                        if (client.IsSick)
                        {
                            _isClientSick = 1;
                            break;
                        }

                        _isClientSick = 0;
                    }
                }
            });
        }

        private async void CreateDoctorsAsync() 
        {
            await CreateDoctors();
        }

        private Task CreateDoctors() 
        {
            return Task.Run(() =>
            {
                var random = new Random();

                _countOfDoctors = random.Next(1, 4);

                for (var i = 0; i < _countOfDoctors; i++)
                    Doctors.Add(_hospitalService.GetDoctor());
            });
        }

        #endregion

        #region Ovveride UpdateValue

        protected override void UpdateValue<TArg>(TArg value, ref TArg storage, [CallerMemberName] string propertyName = null)
        {
            base.UpdateValue(value, ref storage, propertyName);
        }

        #endregion

        #region Properties

        public ObservableCollection<Client> Clients { get; set; } 
        public ObservableCollection<Client> ClientsQueue { get; set; }
        public List<Doctor> Doctors { get; set; }

        public string EventsInHospitalText 
        {
            get { return _eventsInHospitalText; }
            set { UpdateValue(value, ref _eventsInHospitalText); }
        }

        #endregion

        #region Fields

        private readonly ICommandFactory _commandFactory;
        private readonly HospitalService _hospitalService;

        private string _eventsInHospitalText;
        private int _isClientSick;
        private int _countOfDoctors;

        #endregion
    }
}
