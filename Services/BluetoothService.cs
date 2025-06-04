using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Models;
using BackpackControllerApp.Services.Interfaces;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using ZstdSharp;

namespace BackpackControllerApp.Services;

[Service(Enabled = true, Exported = false)]
public class BluetoothService : Service, IBluetoothService
{
    private const string channelId = "BluetoothServiceChannel";
    private const string channelName = "Bluetooth Service";
    private const int serviceId = 1000;
    private const int reconnectIntervall = 5000;

    private readonly ILoggingService _loggingService;
    private readonly ISettingsService _settingsService;

    private BluetoothLeScanner? _bluetoothLeScanner;
    private BluetoothAdapter? _bluetoothAdapter;
    private BluetoothDevice? _device;

    private readonly Context _context;
    private readonly CancellationTokenSource? _cts = new();

    private bool _isReconnecting;
    private int _reconnectAttempts;
    private const int maxReconnectAttempts = 5;

    private CustomScanCallback? _customScanCallback;
    private CustomBluetoothGattCallback? _customBluetoothGattCallback;
    public string IsConnected { get; set; }

    public override IBinder? OnBind(Intent? intent)
    {
        _loggingService.Log(LogLevel.Info, "BluetoothService OnBind", "BluetoothController");
        return null;
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent? intent,
        [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        CreateNotificationChannel();
        CustomStartForegroundService();
        return StartCommandResult.Sticky;
    }

    public BluetoothService(ILoggingService loggingService, ISettingsService settingsService)
    {
        _loggingService = loggingService;
        _settingsService = settingsService;
        _loggingService.Log(LogLevel.Info, "BluetoothService constructor", "BluetoothController");

        _context = Android.App.Application.Context;

        InitializeBluetooth();
    }

    public BluetoothService()
    {
        _loggingService = App.Current.Windows[0].Page.Handler.GetService<ILoggingService>();
        _settingsService = App.Current.Windows[0].Page.Handler.GetService<ISettingsService>();


        _loggingService?.Log(LogLevel.Info, "BluetoothService constructor secondary", "BluetoothController");

        _context = Android.App.Application.Context;

        InitializeBluetooth();
    }

    private void CustomStartForegroundService()
    {
        _loggingService.Log(LogLevel.Info, "CustomStartForegroundService", "CustomStartForegroundService");
        var notification = new Notification.Builder(_context, channelId)
            .SetContentTitle("Bluetooth Service")
            .SetContentText("Scanning for devices...")
            .SetSmallIcon(Resource.Drawable.abc_ic_arrow_drop_right_black_24dp)
            .SetOngoing(true)
            .Build();
        StartForeground(serviceId, notification);
    }

    private void CreateNotificationChannel()
    {
        _loggingService.Log(LogLevel.Info, "CreateNotificationChannel", "CreateNotificationChannel");
        if (!OperatingSystem.IsAndroidVersionAtLeast(33)) return;
        var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default)
        {
            Description = "Bluetooth Service Channel"
        };
        var notificationManager = (NotificationManager)_context.GetSystemService(NotificationService)!;
        notificationManager.CreateNotificationChannel(channel);
    }

    private async Task<bool> CheckBluetoothPermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
        if (status != PermissionStatus.Granted)
        {
            _loggingService.Log(LogLevel.Error, "Bluetooth permission not granted", "BluetoothController");
            status = await Permissions.RequestAsync<Permissions.Bluetooth>();
            if (status != PermissionStatus.Granted)
            {
                _loggingService.Log(LogLevel.Info, "Bluetooth permission not granted by user", "BluetoothController");
                return false;
            }
        }

        return true;
    }

    private void InitializeBluetooth()
    {
        _loggingService.Log(LogLevel.Info, "Initializing Bluetooth", "BluetoothController");

        Task.Run(async () =>
        {
            if (!await CheckBluetoothPermission())
            {
                return;
            }

            if (Android.App.Application.Context.GetSystemService(Context.BluetoothService) is not BluetoothManager
                bluetoothManager)
            {
                _loggingService.Log(LogLevel.Error, "Bluetooth manager not found", "BluetoothController");
                return;
            }

            _bluetoothAdapter = bluetoothManager.Adapter;

            if (_bluetoothAdapter == null)
            {
                _loggingService.Log(LogLevel.Error, "Bluetooth adapter not found", "BluetoothController");
                return;
            }

            if (!_bluetoothAdapter.IsEnabled)
            {
                using var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                enableBtIntent.SetFlags(ActivityFlags.NewTask);
                try
                {
                    Android.App.Application.Context.StartActivity(enableBtIntent);
                }
                catch (ActivityNotFoundException e)
                {
                    _loggingService.Log(LogLevel.Error, e.Message, "BluetoothController");
                }
            }

            await DiscoverAndConnect();
        });
    }

    private async Task DiscoverAndConnect()
    {
        _loggingService.Log(LogLevel.Info, "Discovering Bluetooth devices", "BluetoothController");

        if (await CheckBondedDevices()) return;

        _bluetoothLeScanner = _bluetoothAdapter?.BluetoothLeScanner;

        if (_bluetoothLeScanner == null)
        {
            _loggingService.Log(LogLevel.Error, "Bluetooth socket not connected", "BluetoothController");
            return;
        }

        _customScanCallback =
            new CustomScanCallback(async void (device) =>
            {
                try
                {
                    await FoundDeviceCallbackAction(device);
                }
                catch (Exception e)
                {
                    _loggingService.Log(LogLevel.Error, e.Message, "BluetoothController");
                }
            }, _loggingService);

        _bluetoothLeScanner.StartScan(_customScanCallback);
    }

    private async Task<bool> CheckBondedDevices()
    {
        var bondedDevices = _bluetoothAdapter?.BondedDevices;

        if (!(bondedDevices?.Count > 0)) return false;

        foreach (var bluetoothDevice in bondedDevices)
        {
            if (CheckDevice(bluetoothDevice))
            {
                await ConnectAsync(bluetoothDevice);
                return true;
            }
        }

        return false;
    }

    private async Task FoundDeviceCallbackAction(BluetoothDevice? device)
    {
        if (device == null)
        {
            return;
        }

        _loggingService.Log(LogLevel.Info, $"Found device: {device.Name}", "BluetoothController");

        if (!CheckDevice(device))
        {
            return;
        }

        _bluetoothLeScanner?.StopScan(_customScanCallback);
        _customScanCallback = null;

        await ConnectAsync(device);
    }

    private async Task ConnectAsync(BluetoothDevice device)
    {
        _device = device;

        if (_device.BondState != Bond.Bonded)
        {
            _device.CreateBond();
        }

        if (_device == null)
        {
            _loggingService.Log(LogLevel.Warning, "Device is null, cannot connect.", "BluetoothService");
            return;
        }

        _customBluetoothGattCallback = new CustomBluetoothGattCallback(
            async void (gatt, status, newState) =>
            {
                try
                {
                    if (status == GattStatus.Success)
                    {
                        switch (newState)
                        {
                            case ProfileState.Connected:
                                _loggingService.Log(LogLevel.Info, $"Connected to device: {_device.Name}",
                                    "BluetoothService");
                                IsConnected = "True";
                                break;
                            case ProfileState.Disconnected:
                                _loggingService.Log(LogLevel.Warning, $"Disconnected from device: {_device.Name}",
                                    "BluetoothService");
                                IsConnected = "False";
                                await Reconnect();
                                break;
                        }
                    }
                    else
                    {
                        _loggingService.Log(LogLevel.Error,
                            $"GATT connection error: Status={status}, NewState={newState}",
                            "BluetoothService");
                        IsConnected = "False";
                        await Reconnect();
                    }
                }
                catch (Exception e)
                {
                    _loggingService.Log(LogLevel.Error, e.Message, "BluetoothController");
                }
            },
            _loggingService
        );
        try
        {
            await Task.Run(() => { _device.ConnectGatt(_context, false, _customBluetoothGattCallback); });
        }
        catch (Exception ex)
        {
            _loggingService.Log(LogLevel.Error, $"Error connecting to GATT: {ex.Message}", "BluetoothService");
            await Reconnect();
        }
    }

    private async Task Reconnect()
    {
        await Task.Delay(3000);

        if (_device != null)
        {
            _loggingService.Log(LogLevel.Error, "Device is null, cannot reconnect.", "BluetoothService");
            return;
        }

        if (_isReconnecting)
        {
            return;
        }

        if (_reconnectAttempts >= maxReconnectAttempts)
        {
            _loggingService.Log(LogLevel.Error, "Max reconnect attempts reached. Stopping service.",
                "BluetoothService");
            return;
        }

        _isReconnecting = true;
        _reconnectAttempts++;
        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                _loggingService.Log(LogLevel.Info, "Attempting to reconnect...", "BluetoothService");
                await DiscoverAndConnect();
                if (IsConnected == "True")
                {
                    _loggingService.Log(LogLevel.Info, "Reconnected successfully.", "BluetoothService");
                    _reconnectAttempts = 0;
                    break;
                }

                int delay = reconnectIntervall * _reconnectAttempts;
                await Task.Delay(delay, _cts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            _loggingService.Log(LogLevel.Info, "Reconnection task cancelled.", "BluetoothService");
        }
        finally
        {
            _isReconnecting = false;
        }
    }

    public async Task SendFile(FileBluetoothPacket packet, CancellationToken? token = null,
        IProgress<double>? progress = null)
    {
        if (_device == null)
        {
            _loggingService.Log(LogLevel.Error, "Device is null, cannot connect.", "BluetoothService");
            return;
        }

        await SendAsync(CommandType.File, packet.FileName, packet.Data, progress, token);
    }

    public async Task SendCommand(CommandBluetoothPacket packet)
    {
        if (_device == null)
        {
            _loggingService.Log(LogLevel.Error, "Device is null, cannot connect.", "BluetoothService");
            return;
        }

        await SendAsync(CommandType.Command, packet.Command);
    }


    private async Task SendAsync(CommandType type, string name,
        List<byte>? data = null,
        IProgress<double>? progress = null,
        CancellationToken? token = null)
    {
        try
        {
            using var socket = _device.CreateInsecureL2capChannel(0x0081);
            await socket.ConnectAsync();

            using var packetStream = new MemoryStream();
            await using var writer = new BinaryWriter(packetStream);

            writer.Write((byte)type);
            writer.Write(name);

            if (type == CommandType.File)
            {
                if (data == null)
                {
                    _loggingService.Log(LogLevel.Error, "Missing fileData", "BluetoothController");
                }

                using var compressor = new Compressor();
                var compressedData = compressor.Wrap(data.ToArray());
                writer.Write(compressedData);
            }


            var packetData = packetStream.ToArray();
            await using var outStream = socket.OutputStream;

            if (outStream is null)
            {
                _loggingService.Log(LogLevel.Error, "OutputStream is null", "BluetoothService");
                return;
            }

            var chunkSize = 480;
            var size = packetData.Length;
            int offset = 0;


            while (offset < size)
            {
                var toSend = Math.Min(chunkSize, size - offset);
                await outStream.WriteAsync(packetData.AsMemory(offset, toSend), token ?? CancellationToken.None);
                await outStream.FlushAsync();
                offset += toSend;
                progress?.Report((double)offset / size);
                await Task.Delay(5);
            }

            await Task.Delay(1000);
            socket.Close();
        }
        catch (IOException ex)
        {
            _loggingService.Log(LogLevel.Error, $"Error sending file: {ex.Message}", "BluetoothService");
            await Reconnect();
        }
        catch (Exception ex) when (ex is ObjectDisposedException or TaskCanceledException)
        {
            _loggingService.Log(LogLevel.Warning, $"Operation cancelled or disposed: {ex.Message}", "BluetoothService");
        }
        catch (Exception ex)
        {
            _loggingService.Log(LogLevel.Error, $"Error sending file: {ex.Message}", "BluetoothService");
        }
    }

    private bool CheckDevice(BluetoothDevice? device)
    {
        if (device == null)
        {
            _loggingService.Log(LogLevel.Warning, "Device is null, cannot check.", "BluetoothService");
            return false;
        }

        var name = device?.Alias;

        if (name == null || string.IsNullOrEmpty(name)) return false;

        _loggingService.Log(LogLevel.Info, $"Checking device alias: {name}", "BluetoothDevice");

        if (name == "ScreenControllerApp")
        {
            _loggingService.Log(LogLevel.Info, $"Found matching device: {device.Name}", "BluetoothService");
            return true;
        }

        return false;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _cts?.Cancel();
        _bluetoothLeScanner?.StopScan(_customScanCallback);
        _customScanCallback?.Dispose();
        _customBluetoothGattCallback?.Dispose();
        StopForeground(StopForegroundFlags.Remove);
        StopSelf();
        _loggingService.Log(LogLevel.Info, "BluetoothService destroyed.", "BluetoothService");
    }

    private class CustomScanCallback : ScanCallback, IDisposable
    {
        private readonly Action<BluetoothDevice?> _onScanResult;
        private readonly ILoggingService _loggingService;
        private bool _disposed = false;

        public CustomScanCallback(Action<BluetoothDevice?> onScanResult, ILoggingService loggingService)
        {
            _onScanResult = onScanResult;
            _loggingService = loggingService;
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult? result)
        {
            if (_disposed)
            {
                _loggingService.Log(LogLevel.Warning, "ScanCallback::OnScanResult called after disposal.",
                    "BluetoothService");
                return;
            }

            base.OnScanResult(callbackType, result);
            var device = result?.Device;
            if (device == null)
            {
                _loggingService.Log(LogLevel.Warning, "ScanResult::Device is null.", "BluetoothService");
                return;
            }

            _onScanResult(device);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    private sealed class CustomBluetoothGattCallback(
        Action<BluetoothGatt?, GattStatus, ProfileState> onConnectionStateChange,
        ILoggingService loggingService)
        : BluetoothGattCallback, IDisposable
    {
        private bool _disposed = false;

        public override void OnConnectionStateChange(BluetoothGatt? gatt, [GeneratedEnum] GattStatus status,
            [GeneratedEnum] ProfileState newState)
        {
            if (_disposed)
            {
                loggingService.Log(LogLevel.Warning, "GattCallback::OnConnectionStateChange called after disposal.",
                    "BluetoothService");
                return;
            }

            loggingService.Log(LogLevel.Info, $"Connection state changed. Status: {status}, New state: {newState}",
                "BluetoothService");
            onConnectionStateChange?.Invoke(gatt, status, newState);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
            }

            _disposed = true;
        }

        public new void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}