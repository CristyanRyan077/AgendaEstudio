using AgendaMobile.Services;
using AgendaShared.DTOs;
using System.Net.Http.Json;

namespace AgendaMobile.Pages;

public partial class LoadingPage : ContentPage
{
    private readonly IApiService _apiService;
    public LoadingPage(IApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
        _ = CarregarDadosAsync();
    }
    private async Task CarregarDadosAsync()
    {
        try
        {
            
            var agendamentos = await _apiService.ObterAgendamentosAsync();
            var clientes = await _apiService.ObterClientesAsync();

            // Após carregar, navega para a tela principal
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Navigation.PushAsync(new MainPage(agendamentos, clientes));

                // Opcional: remove LoadingPage da pilha
                Navigation.RemovePage(this);
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao carregar dados: {ex.Message}", "OK");
        }
    }
}