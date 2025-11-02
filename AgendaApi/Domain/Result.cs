namespace AgendaApi.Domain
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; } // A mensagem de erro, se houver

        // Construtor privado para forçar o uso dos métodos "Ok" e "Fail"
        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        // Método estático para criar um resultado de SUCESSO
        public static Result Ok()
        {
            return new Result(true, null);
        }

        // Método estático para criar um resultado de FALHA
        public static Result Fail(string message)
        {
            return new Result(false, message);
        }
    }
}
