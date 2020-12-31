using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Microbit.RemoteControl
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private const string DeviceGuid = "00000000-0000-0000-0000-f004ed3a41ea";
        private const string UartServiceGuid = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
        private const string UartWriteCharacteristicGuid = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";
        private const string MoveCommand = "M";
        private const string BackCommand = "B";
        private const string StopCommand = "S";
        private const string NewLine = "\r\n";

        IBluetoothLE ble;
        ObservableCollection<IDevice> deviceList;
        IAdapter adapter;
        IDevice device;
        IService uartService;
        private bool _connected;
        
        public bool Connected
        {
            get => _connected;
            set
            {
                _connected = value;
                OnPropertyChanged(nameof(Connected));
            }
        }

        public MainPage()
        {
            InitializeComponent();
            Connected = false;
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            adapter.ScanMode = ScanMode.LowPower;
            deviceList = new ObservableCollection<IDevice>();
        }

        private async void OnConnectClicked(object sender, EventArgs e)
        {
            try
            {
                device = await adapter.ConnectToKnownDeviceAsync(Guid.Parse(DeviceGuid));
                Connected = true;
                adapter.DeviceConnectionLost += (s, er) => Connected = false;
                adapter.DeviceDisconnected += (d, ev) => Connected = false;
                uartService = await device.GetServiceAsync(Guid.Parse(UartServiceGuid));
                await DisplayAlert("Notice", "Microbit device connected", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Notice", ex.Message.ToString(), "OK");
            }
        }

        private void Adapter_DeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
            try
            {
                deviceList.Clear();
                adapter.DeviceDiscovered += (s, a) =>
                {
                    deviceList.Add(a.Device);
                };

                //We have to test if the device is scanning 
                if (!ble.Adapter.IsScanning)
                {
                    await adapter.StartScanningForDevicesAsync();

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Notice", ex.Message.ToString(), "Error !");
            }
        }

        private async void OnMoveClicked(object sender, EventArgs e)
        {
            await ExecuteCommand(MoveCommand);
        }

        private async Task ExecuteCommand(string command)
        {
            try
            {
                if (uartService != null)
                {
                    var characteristic = await uartService
                        .GetCharacteristicAsync(Guid.Parse(UartWriteCharacteristicGuid));
                    if (characteristic != null)
                    {
                        await characteristic
                            .WriteAsync(Encoding.ASCII.GetBytes(command + NewLine));
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Notice", e.Message.ToString(), "Error !");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await ExecuteCommand(BackCommand);
        }

        private async void OnStopClicked(object sender, EventArgs e)
        {
            await ExecuteCommand(StopCommand);
        }
    }
}
