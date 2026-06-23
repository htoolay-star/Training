using Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using WebApp.Components.Shared;
using WebApp.Services.Auth;
using WebApp.Services.Dialogs;

namespace WebApp.Services;

public sealed class InjectService
{
    private readonly SessionManager _session;
    private readonly NavigationManager _navigation;
    private readonly IDialogService _dialog;
    private readonly ISnackbar _snackbar;

    public InjectService(
        SessionManager session,
        NavigationManager navigation,
        IDialogService dialog,
        ISnackbar snackbar)
    {
        _session = session;
        _navigation = navigation;
        _dialog = dialog;
        _snackbar = snackbar;
    }

    public Task<Result<T>> GetAsync<T>(string url, CancellationToken ct = default) =>
        _session.SendAsync<T>(HttpMethod.Get, url, body: null, ct);

    public Task<Result<T>> CreateAsync<T>(string url, object body, CancellationToken ct = default) =>
        _session.SendAsync<T>(HttpMethod.Post, url, body, ct);

    public Task<Result<T>> UpdateAsync<T>(string url, object body, CancellationToken ct = default) =>
        _session.SendAsync<T>(HttpMethod.Put, url, body, ct);

    public Task<Result<T>> DeleteAsync<T>(string url, CancellationToken ct = default) =>
        _session.SendAsync<T>(HttpMethod.Delete, url, body: null, ct);

    public async Task<bool> SubmitAsync<T>(Task<Result<T>> apiCall, string? successMessage = null)
    {
        var result = await apiCall;
        if (result.IsSuccess)
        {
            ShowSnackbar(successMessage ?? result.Message, DialogType.Success);
            return true;
        }

        await ShowMessageAsync(result.Message, DialogType.Error);
        return false;
    }

    #region Dialogs

    public Task ShowInfoAsync(string message, string title = "Information") =>
        ShowMessageAsync(message, DialogType.Info, title);

    public Task ShowSuccessAsync(string message, string title = "Success") =>
        ShowMessageAsync(message, DialogType.Success, title);

    public Task ShowWarningAsync(string message, string title = "Warning") =>
        ShowMessageAsync(message, DialogType.Warning, title);

    public Task ShowErrorAsync(string message, string title = "Error") =>
        ShowMessageAsync(message, DialogType.Error, title);

    public async Task<bool> ConfirmAsync(
        string message,
        string title = "Are you sure?",
        string confirmText = "Confirm",
        Color color = Color.Primary)
    {
        var parameters = new DialogParameters<ConfirmDialog>
        {
            { x => x.Title, title },
            { x => x.Message, message },
            { x => x.ConfirmText, confirmText },
            { x => x.Color, color },
        };

        var options = new DialogOptions { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, CloseButton = true };
        var dialog = await _dialog.ShowAsync<ConfirmDialog>(title, parameters, options);
        var result = await dialog.Result;

        return result is { Canceled: false, Data: true };
    }

    public Task<bool> ConfirmDeleteAsync(string? itemName = null) =>
        ConfirmAsync(
            message: itemName is null
                ? "This record will be permanently deleted."
                : $"\"{itemName}\" will be permanently deleted.",
            title: "Delete",
            confirmText: "Delete",
            color: Color.Error);

    private async Task ShowMessageAsync(string message, DialogType type, string title = "")
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        var parameters = new DialogParameters<MessageDialog>
        {
            { x => x.Title, title },
            { x => x.Message, message },
            { x => x.DialogType, type },
        };

        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true,
            NoHeader = true,
            Position = DialogPosition.Center,
        };

        var dialog = await _dialog.ShowAsync<MessageDialog>(title, parameters, options);
        await dialog.Result;
    }

    #endregion

    #region Snackbars

    public void ShowSuccessSnackbar(string? message) => ShowSnackbar(message, DialogType.Success);
    public void ShowInfoSnackbar(string? message) => ShowSnackbar(message, DialogType.Info);
    public void ShowWarningSnackbar(string? message) => ShowSnackbar(message, DialogType.Warning);
    public void ShowErrorSnackbar(string? message) => ShowSnackbar(message, DialogType.Error);

    private void ShowSnackbar(string? message, DialogType type)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        var severity = type switch
        {
            DialogType.Success => Severity.Success,
            DialogType.Warning => Severity.Warning,
            DialogType.Error => Severity.Error,
            _ => Severity.Info
        };

        _snackbar.Add(message, severity, config =>
        {
            config.SnackbarVariant = Variant.Filled;
            config.ShowCloseIcon = true;
        });
    }

    #endregion

    #region Navigation

    public void GoTo(string url, bool forceReload = false) => _navigation.NavigateTo(url, forceReload);

    public void Refresh(bool forceReload = false) => _navigation.Refresh(forceReload);

    public string CurrentRelativePath => _navigation.ToBaseRelativePath(_navigation.Uri);

    #endregion
}
