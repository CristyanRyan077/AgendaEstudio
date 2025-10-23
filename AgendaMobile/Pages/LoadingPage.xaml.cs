using AgendaMobile.Helpers;
using AgendaMobile.Services;
using AgendaMobile.ViewModels;
using AgendaShared.DTOs;
using System.Net.Http.Json;

namespace AgendaMobile.Pages;

public partial class LoadingPage : ContentPage
{
    private readonly IApiService _apiService;
    private readonly CalendarioViewModel _calendarioViewModel;  
    public LoadingPage(IApiService apiService, CalendarioViewModel calendariovm)
	{
		InitializeComponent();
        _apiService = apiService;
        _calendarioViewModel = calendariovm;
        _ = InicializarAsync();
    }
    private async Task InicializarAsync()
    {
        await _calendarioViewModel.CarregarDadosAsync();

        if (!string.IsNullOrEmpty(_calendarioViewModel.ErroMensagem))
        {
            await DisplayAlert("Erro", $"Falha ao carregar dados: {_calendarioViewModel.ErroMensagem}", "OK");
            return;
        }

        // se carregou com sucesso, navega
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var mainPage = ServiceHelper.GetService<MainPage>();
            await Navigation.PushAsync(mainPage);
            Navigation.RemovePage(this);
        });
    }
}