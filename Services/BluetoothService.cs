using System.Text;
using System.Text.Json;
using Android.Bluetooth;
using Android.Content;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Models;
using BackpackControllerApp.Services.Interfaces;

namespace BackpackControllerApp.Services;

public class BluetoothService : IBluetoothService
{
    private readonly ILoggingService _loggingService;
    private readonly ISettingsService _settingsService;

    private readonly BluetoothAdapter? _bluetoothAdapter;
    private BluetoothSocket? _bluetoothSocket;
    public string IsConnected { get; set; }

    public BluetoothService(ILoggingService loggingService, ISettingsService settingsService)
    {
        _loggingService = loggingService;
        _settingsService = settingsService;

        IsConnected = "starting";

        var status = Permissions.CheckStatusAsync<Permissions.Bluetooth>().GetAwaiter().GetResult();

        if (status != PermissionStatus.Granted)
        {
            _loggingService.Log(LogLevel.Error, "Bluetooth permission not granted", "BluetoothController");

            Permissions.RequestAsync<Permissions.Bluetooth>().GetAwaiter().GetResult();
            return;
        }

        var bluetoothManager =
            Android.App.Application.Context.GetSystemService(Context.BluetoothService) as
                BluetoothManager;

        _bluetoothAdapter = bluetoothManager?.Adapter;

        if (_bluetoothAdapter == null)
        {
            _loggingService.Log(LogLevel.Error, "Bluetooth adapter not found", "BluetoothController");
            return;
        }

        if (!CheckBluetoothConnected())
        {
            _loggingService.Log(LogLevel.Warning, "Bluetooth adapter not enabled", "BluetoothController");
        }

        _loggingService.Log(LogLevel.Info, "BluetoothController initialized", "BluetoothController");
        IsConnected = "started";
        Connect();
    }

    public void Connect()
    {
        _loggingService.Log(LogLevel.Info, "Connecting to Bluetooth device", "BluetoothController");
        IsConnected = "connecting";
        if (!CheckBluetoothConnected())
        {
            _loggingService.Log(LogLevel.Warning, "no Bluetooth", "BluetoothController");
            return;
        }

        var device = GetBluetoothDevice();

        if (device == null)
        {
            _loggingService.Log(LogLevel.Error, "No Bluetooth device found", "BluetoothController");
            return;
        }

        IsConnected = "found device";
        ConnectToDevice(device);
    }

    public async Task SendFile(FileBluetoothPacket packet)
    {
        IsConnected = "sending file";
        _loggingService.Log(LogLevel.Info, "Sending file", "BluetoothController");

        if (_bluetoothSocket is not { IsConnected: true })
        {
            _loggingService.Log(LogLevel.Error, "Bluetooth socket not connected", "BluetoothController");
            return;
        }

        try
        {
            var outStream = _bluetoothSocket?.OutputStream;

            var serializedPacket = JsonSerializer.Serialize(packet);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(serializedPacket);
            await stream.CopyToAsync(outStream);
        }
        catch (Exception e)
        {
            _loggingService.Log(LogLevel.Error, "Could not send file", e.Message);
        }
    }

    public async Task SendCommand(CommandBluetoothPacket packet)
    {
        IsConnected = "sending command";
        _loggingService.Log(LogLevel.Info, "Sending command", "BluetoothController");


        if (_bluetoothSocket is not { IsConnected: true })
        {
            _loggingService.Log(LogLevel.Error, "Bluetooth socket not connected", "BluetoothController");
            return;
        }

        try
        {
            var outStream = _bluetoothSocket?.OutputStream;

            var serializedPacket = JsonSerializer.Serialize(packet);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(serializedPacket);
            await stream.CopyToAsync(outStream);
        }
        catch (Exception e)
        {
            _loggingService.Log(LogLevel.Error, "Could not send file", e.Message);
        }
    }

    private BluetoothDevice? GetBluetoothDevice()
    {
        _loggingService.Log(LogLevel.Info, "Getting Bluetooth device", "BluetoothController");

        var devices = _bluetoothAdapter?.BondedDevices;

        if (devices != null)
        {
            foreach (var device in devices)
            {
                if (CheckDevice(device))
                {
                    return device;
                }
            }
        }

        DiscoverDevices();

        _loggingService.Log(LogLevel.Error, "No Bluetooth device found", "BluetoothController");

        return null;
    }

    private void DiscoverDevices()
    {
        _loggingService.Log(LogLevel.Info, "Discovering Bluetooth devices", "BluetoothController");

        if (!_bluetoothAdapter.StartDiscovery())
        {
            _loggingService.Log(LogLevel.Error, "Bluetooth adapter not started", "BluetoothController");
            return;
        }

        var filter = new IntentFilter(BluetoothDevice.ActionFound);

        var receiver = new CustomBluetoothReceiver();

        receiver.DeviceFound += (_, bluetoothDevice) =>
        {
            if (CheckDevice(bluetoothDevice))
            {
                _loggingService.Log(LogLevel.Info, "Found Bluetooth device", "BluetoothController");

                if (!_bluetoothAdapter.CancelDiscovery())
                {
                    _loggingService.Log(LogLevel.Error, "Bluetooth discovery cancel failed", "BluetoothController");
                }

                ConnectToDevice(bluetoothDevice);
            }
        };

        Android.App.Application.Context.RegisterReceiver(receiver, filter);
    }

    private void ConnectToDevice(BluetoothDevice device)
    {
        _loggingService.Log(LogLevel.Info, "Connecting to Bluetooth device", "BluetoothController");
        try
        {
            _bluetoothSocket = device.CreateRfcommSocketToServiceRecord(_settingsService.Uuid);

            _bluetoothSocket?.Connect();
        }
        catch (IOException e)
        {
            try
            {
                _bluetoothSocket?.Close();
            }
            catch (IOException)
            {
                _loggingService.Log(LogLevel.Error, "Could not close the client socket", e.Message);
            }

            _loggingService.Log(LogLevel.Error, e.Message);
        }
    }

    private bool CheckBluetoothConnected()
    {
        if (_bluetoothAdapter == null)
        {
            _loggingService.Log(LogLevel.Error, "Bluetooth adapter not initialized", "BluetoothController");
            return false;
        }

        if (_bluetoothAdapter.IsEnabled) return true;

        using var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);

        try
        {
            Android.App.Application.Context.StartActivity(enableBtIntent);
        }
        catch (ActivityNotFoundException e)
        {
            _loggingService.Log(LogLevel.Error, e.Message, "BluetoothController");
        }

        return _bluetoothAdapter.IsEnabled;
    }

    private bool CheckDevice(BluetoothDevice device)
    {
        _loggingService.Log(LogLevel.Info, $"Name: {device.Name}, Address: {device.Address}", "BluetoothController");

        var uuids = device.GetUuids()?.ToList();


        if (uuids == null)
        {
            _loggingService.Log(LogLevel.Error, "No UUIDs found");
            return false;
        }

        StringBuilder builder = new StringBuilder();

        uuids.ForEach(x => { builder.Append(x + "\n"); });

        _loggingService.Log(LogLevel.Info, builder.ToString(), "BluetoothController");

        if (uuids.All(x => x.Uuid != _settingsService.Uuid))
        {
            _loggingService.Log(LogLevel.Error, "No matching UUIDs found");
            return false;
        }

        return true;
    }
}

internal class CustomBluetoothReceiver : BroadcastReceiver
{
    public event EventHandler<BluetoothDevice>? DeviceFound;

    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context == null || intent == null) return;

        if (BluetoothDevice.ActionFound.Equals(intent.Action))
        {
            var device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;
            if (device != null) return;
            DeviceFound?.Invoke(this, device);
        }
    }
}