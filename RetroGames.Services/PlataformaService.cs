using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public Task<List<Plataforma>> ListarAsync()
    {
        return repositorio.Plataforma.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        return await repositorio.Plataforma.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        if (await repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            plataforma.Id = await repositorio.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        return plataforma;
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        await BuscarPorIdAsync(plataforma.Id);
        plataforma.Nome = plataforma.Nome.Trim();

        if (await repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome, plataforma.Id))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            await repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);

        if (await repositorio.Plataforma.PossuiJogosAsync(id))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaPossuiJogos);
        }

        await repositorio.Plataforma.ExcluirAsync(id);
    }
}
