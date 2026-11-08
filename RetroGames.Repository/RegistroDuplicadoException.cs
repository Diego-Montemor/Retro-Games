namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(Exception excecaoInterna)
        : base("Já existe um registro com o valor informado para um campo único.", excecaoInterna)
    {
    }
}
