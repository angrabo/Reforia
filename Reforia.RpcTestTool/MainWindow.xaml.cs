using System.Text.Encodings.Web;
using Reforia.Core.Modules.Communication.Contracts;

namespace Reforia.RpcTestTool;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

public partial class MainWindow : Window
{
    private HubConnection? _connection;

    private static readonly JsonSerializerOptions PrettyJson = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        
    };

    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        => NewRequestId();

    private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (_connection is not null && _connection.State != HubConnectionState.Disconnected)
            {
                await DisconnectAsync();
                return;
            }

            await ConnectAsync();
        }
        catch (Exception ex)
        {
            AppendLog($"[ERROR] {ex.GetType().Name}: {ex.Message}");
        }
    }

    private async void SendButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_connection is null || _connection.State != HubConnectionState.Connected)
        {
            AppendLog("[WARN] Connection is required.");
            return;
        }

        var functionName = FunctionNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(functionName))
        {
            AppendLog("[WARN] FunctionName is required.");
            return;
        }

        var requestId = RequestIdTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(requestId))
        {
            NewRequestId();
            requestId = RequestIdTextBox.Text;
        }

        var bodyText = BodyTextBox.Text;

        if (!IsValidJson(bodyText))
            AppendLog("[ERROR] JSON parse error");

        var request = new WebBody
        {
            FunctionName = functionName,
            RequestId = requestId,
            Body = bodyText
        };

        SendButton.IsEnabled = false;
        
        try
        {
            AppendLog($"→ Call(FunctionName='{request.FunctionName}', RequestId='{request.RequestId}')");

            var response = await _connection.InvokeAsync<WebResponse>("Call", request);

            AppendLog($"← StatusCode={response.StatusCode}, RequestId='{response.RequestId}'");

            var rendered = JsonSerializer.Serialize(response, PrettyJson);
            AppendResponse(rendered);

            NewRequestId();
        }
        catch (Exception ex)
        {
            AppendLog($"[ERROR] {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            SendButton.IsEnabled = _connection.State == HubConnectionState.Connected;
        }
    }

    private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        => ResponseTextBox.Clear();

    private void NewRequestIdButton_OnClick(object sender, RoutedEventArgs e) 
        => NewRequestId();

    private void NewRequestId()
        => RequestIdTextBox.Text = Guid.NewGuid().ToString();

    private async Task ConnectAsync()
    {
        var scheme = ((SchemeComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "http").Trim();
        var host = HostTextBox.Text.Trim();
        var port = PortTextBox.Text.Trim();
        var hubPath = HubPathTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(host))
            throw new InvalidOperationException("Invalid Host");
        if (string.IsNullOrWhiteSpace(port))
            throw new InvalidOperationException("Invalid Port");
        if (string.IsNullOrWhiteSpace(hubPath))
            hubPath = "/hub";
        if (!hubPath.StartsWith('/'))
            hubPath = "/" + hubPath;

        var hubUrl = $"{scheme}://{host}:{port}{hubPath}";
        AppendLog($"Connecting with: {hubUrl}");

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.Reconnecting += error =>
        {
            Dispatcher.Invoke(() =>
            {
                StatusTextBlock.Text = "Reconnecting…";
                AppendLog($"[INFO] Reconnecting… {(error is null ? "" : error.Message)}");
            });
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            Dispatcher.Invoke(() =>
            {
                StatusTextBlock.Text = "Connected";
                AppendLog($"[INFO] Reconnected. ConnectionId={(connectionId ?? "<null>")}");
            });
            return Task.CompletedTask;
        };

        _connection.Closed += error =>
        {
            Dispatcher.Invoke(() =>
            {
                StatusTextBlock.Text = "Diconnected";
                ConnectButton.Content = "Connected";
                SendButton.IsEnabled = false;
                AppendLog($"[INFO] Closed. {(error is null ? "" : error.Message)}");
            });
            return Task.CompletedTask;
        };

        await _connection.StartAsync();

        StatusTextBlock.Text = "Connect";
        ConnectButton.Content = "Disconnect";
        SendButton.IsEnabled = true;

        AppendLog("[INFO] Connected.");
    }

    private async Task DisconnectAsync()
    {
        if (_connection is null)
            return;

        AppendLog("[INFO] Disconnecting…");
        await _connection.StopAsync();
        await _connection.DisposeAsync();
        _connection = null;

        StatusTextBlock.Text = "Disconnected";
        ConnectButton.Content = "Connect";
        SendButton.IsEnabled = false;

        AppendLog("[INFO] Disconnected.");
    }

    private void AppendResponse(string text)
    {
        ResponseTextBox.AppendText(text);
        ResponseTextBox.AppendText(Environment.NewLine);
        ResponseTextBox.AppendText(new string('-', 60));
        ResponseTextBox.AppendText(Environment.NewLine);
        ResponseTextBox.ScrollToEnd();
    }

    private void AppendLog(string text)
    {
        var line = $"[{DateTime.Now:HH:mm:ss}] {text}";
        ResponseTextBox.AppendText(line);
        ResponseTextBox.AppendText(Environment.NewLine);
        ResponseTextBox.ScrollToEnd();
    }

    private static bool IsValidJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        try
        {
            using var _ = JsonDocument.Parse(text);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void BodyGeneratorButton_OnClick(object sender, RoutedEventArgs e)
    {
        var window = new JsonConverter();
        window.Show();
    }
}