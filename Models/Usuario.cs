using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SISTEMA_DE_VENDAS_GENERICO.Models
{
    /// <summary>
    /// Modelo que representa um usuário do sistema
    /// </summary>
    public class Usuario : BaseModel
    {
        #region Campos Privados
        
        private string _nome;
        private string _nomeUsuario;
        private string _senha;
        private NivelAcessoEnum _nivelAcesso;
        private DateTime _dataCadastro;
        
        #endregion

        #region Propriedades Públicas
        
        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        public string Nome
        {
            get => _nome;
            set => SetProperty(ref _nome, value?.Trim());
        }

        /// <summary>
        /// Nome de usuário para login (único no sistema)
        /// </summary>
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nome de usuário deve ter entre 3 e 50 caracteres")]
        public string NomeUsuario
        {
            get => _nomeUsuario;
            set => SetProperty(ref _nomeUsuario, value?.Trim()?.ToLower());
        }

        /// <summary>
        /// Senha do usuário (deve ser criptografada antes de armazenar)
        /// </summary>
        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
        public string Senha
        {
            get => _senha;
            set => SetProperty(ref _senha, value);
        }

        /// <summary>
        /// Nível de acesso do usuário no sistema
        /// </summary>
        public NivelAcessoEnum NivelAcesso
        {
            get => _nivelAcesso;
            set => SetProperty(ref _nivelAcesso, value);
        }

        /// <summary>
        /// Data de cadastro do usuário
        /// </summary>
        public DateTime DataCadastro
        {
            get => _dataCadastro;
            set => SetProperty(ref _dataCadastro, value);
        }

        /// <summary>
        /// Descrição textual do nível de acesso
        /// </summary>
        public string DescricaoNivelAcesso => GetDescricaoNivelAcesso(NivelAcesso);
        
        #endregion

        #region Construtor
        
        public Usuario()
        {
            DataCadastro = DateTime.Now;
            NivelAcesso = NivelAcessoEnum.Vendedor;
        }
        
        #endregion

        #region Métodos Públicos
        
        /// <summary>
        /// Verifica se o usuário tem permissão para acessar determinada funcionalidade
        /// </summary>
        /// <param name="funcionalidade">Funcionalidade a ser verificada</param>
        /// <returns>True se tem permissão</returns>
        public bool TemPermissao(FuncionalidadeEnum funcionalidade)
        {
            switch (NivelAcesso)
            {
                case NivelAcessoEnum.Administrador:
                    return true; // Administrador tem acesso total

                case NivelAcessoEnum.Gerente:
                    return funcionalidade != FuncionalidadeEnum.GestaoUsuarios &&
                           funcionalidade != FuncionalidadeEnum.ConfiguracoesSistema;

                case NivelAcessoEnum.Vendedor:
                    return funcionalidade == FuncionalidadeEnum.Vendas ||
                           funcionalidade == FuncionalidadeEnum.ConsultaProdutos ||
                           funcionalidade == FuncionalidadeEnum.ConsultaClientes;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Obtém a descrição textual do nível de acesso
        /// </summary>
        /// <param name="nivel">Nível de acesso</param>
        /// <returns>Descrição do nível</returns>
        public static string GetDescricaoNivelAcesso(NivelAcessoEnum nivel)
        {
            switch (nivel)
            {
                case NivelAcessoEnum.Administrador:
                    return "Administrador";
                case NivelAcessoEnum.Gerente:
                    return "Gerente";
                case NivelAcessoEnum.Vendedor:
                    return "Vendedor";
                default:
                    return "Indefinido";
            }
        }
        
        #endregion

        #region Sobrescrita de Métodos
        
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   !string.IsNullOrWhiteSpace(NomeUsuario) &&
                   !string.IsNullOrWhiteSpace(Senha) &&
                   Nome.Length >= 2 && Nome.Length <= 100 &&
                   NomeUsuario.Length >= 3 && NomeUsuario.Length <= 50 &&
                   Senha.Length >= 6;
        }

        public override string[] GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(Nome))
                errors.Add("Nome é obrigatório");
            else if (Nome.Length < 2 || Nome.Length > 100)
                errors.Add("Nome deve ter entre 2 e 100 caracteres");

            if (string.IsNullOrWhiteSpace(NomeUsuario))
                errors.Add("Nome de usuário é obrigatório");
            else if (NomeUsuario.Length < 3 || NomeUsuario.Length > 50)
                errors.Add("Nome de usuário deve ter entre 3 e 50 caracteres");

            if (string.IsNullOrWhiteSpace(Senha))
                errors.Add("Senha é obrigatória");
            else if (Senha.Length < 6)
                errors.Add("Senha deve ter pelo menos 6 caracteres");

            return errors.ToArray();
        }

        public override string ToString()
        {
            return $"{Nome} ({NomeUsuario}) - {DescricaoNivelAcesso}";
        }
        
        #endregion
    }

    #region Enumerações
    
    /// <summary>
    /// Níveis de acesso disponíveis no sistema
    /// </summary>
    public enum NivelAcessoEnum
    {
        Vendedor = 1,
        Gerente = 2,
        Administrador = 3
    }

    /// <summary>
    /// Funcionalidades do sistema para controle de permissões
    /// </summary>
    public enum FuncionalidadeEnum
    {
        Dashboard,
        Vendas,
        ConsultaProdutos,
        GestaProdutos,
        ConsultaClientes,
        GestaoClientes,
        GestaoFornecedores,
        ControleEstoque,
        Relatorios,
        GestaoUsuarios,
        ConfiguracoesSistema
    }
    
    #endregion
}