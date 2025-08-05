using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SISTEMA_DE_VENDAS_GENERICO.Models
{
    /// <summary>
    /// Modelo que representa um cliente do sistema
    /// </summary>
    public class Cliente : BaseModel
    {
        #region Campos Privados
        
        private string _nome;
        private string _telefone;
        private string _email;
        private string _endereco;
        private string _documento;
        private TipoDocumentoEnum _tipoDocumento;
        private DateTime _dataCadastro;
        private DateTime? _dataNascimento;
        private string _observacoes;
        
        #endregion

        #region Propriedades Públicas
        
        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 150 caracteres")]
        public string Nome
        {
            get => _nome;
            set => SetProperty(ref _nome, value?.Trim());
        }

        /// <summary>
        /// Número de telefone do cliente
        /// </summary>
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Telefone
        {
            get => _telefone;
            set => SetProperty(ref _telefone, LimparTelefone(value));
        }

        /// <summary>
        /// Endereço de email do cliente
        /// </summary>
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value?.Trim()?.ToLower());
        }

        /// <summary>
        /// Endereço completo do cliente
        /// </summary>
        [StringLength(255, ErrorMessage = "Endereço deve ter no máximo 255 caracteres")]
        public string Endereco
        {
            get => _endereco;
            set => SetProperty(ref _endereco, value?.Trim());
        }

        /// <summary>
        /// Número do documento (BI, Passaporte, etc.)
        /// </summary>
        [StringLength(50, ErrorMessage = "Documento deve ter no máximo 50 caracteres")]
        public string Documento
        {
            get => _documento;
            set => SetProperty(ref _documento, value?.Trim()?.ToUpper());
        }

        /// <summary>
        /// Tipo do documento
        /// </summary>
        public TipoDocumentoEnum TipoDocumento
        {
            get => _tipoDocumento;
            set => SetProperty(ref _tipoDocumento, value);
        }

        /// <summary>
        /// Data de cadastro do cliente
        /// </summary>
        public DateTime DataCadastro
        {
            get => _dataCadastro;
            set => SetProperty(ref _dataCadastro, value);
        }

        /// <summary>
        /// Data de nascimento do cliente (opcional)
        /// </summary>
        public DateTime? DataNascimento
        {
            get => _dataNascimento;
            set => SetProperty(ref _dataNascimento, value);
        }

        /// <summary>
        /// Observações sobre o cliente
        /// </summary>
        [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        public string Observacoes
        {
            get => _observacoes;
            set => SetProperty(ref _observacoes, value?.Trim());
        }

        /// <summary>
        /// Idade do cliente (calculada a partir da data de nascimento)
        /// </summary>
        public int? Idade
        {
            get
            {
                if (!DataNascimento.HasValue) return null;
                
                var hoje = DateTime.Today;
                var idade = hoje.Year - DataNascimento.Value.Year;
                
                if (DataNascimento.Value.Date > hoje.AddYears(-idade))
                    idade--;
                
                return idade;
            }
        }

        /// <summary>
        /// Descrição do tipo de documento
        /// </summary>
        public string DescricaoTipoDocumento => GetDescricaoTipoDocumento(TipoDocumento);

        /// <summary>
        /// Telefone formatado para exibição
        /// </summary>
        public string TelefoneFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Telefone)) return string.Empty;
                
                // Formato angolano: +244 XXX XXX XXX
                if (Telefone.Length >= 9)
                {
                    var numero = Telefone.Replace("+244", "").Replace(" ", "");
                    if (numero.Length == 9)
                    {
                        return $"+244 {numero.Substring(0, 3)} {numero.Substring(3, 3)} {numero.Substring(6, 3)}";
                    }
                }
                
                return Telefone;
            }
        }
        
        #endregion

        #region Construtor
        
        public Cliente()
        {
            DataCadastro = DateTime.Now;
            TipoDocumento = TipoDocumentoEnum.BI;
        }
        
        #endregion

        #region Métodos Privados
        
        /// <summary>
        /// Remove caracteres especiais do telefone
        /// </summary>
        /// <param name="telefone">Telefone a ser limpo</param>
        /// <returns>Telefone limpo</returns>
        private string LimparTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone)) return string.Empty;
            
            // Remove tudo exceto números, + e espaços
            return Regex.Replace(telefone.Trim(), @"[^\d\+\s]", "");
        }
        
        #endregion

        #region Métodos Públicos
        
        /// <summary>
        /// Verifica se o email é válido
        /// </summary>
        /// <returns>True se o email é válido</returns>
        public bool EmailValido()
        {
            if (string.IsNullOrWhiteSpace(Email)) return true; // Email é opcional
            
            try
            {
                var addr = new System.Net.Mail.MailAddress(Email);
                return addr.Address == Email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se o telefone é válido (formato angolano)
        /// </summary>
        /// <returns>True se o telefone é válido</returns>
        public bool TelefoneValido()
        {
            if (string.IsNullOrWhiteSpace(Telefone)) return true; // Telefone é opcional
            
            // Formato angolano: 9 dígitos (com ou sem +244)
            var numero = Telefone.Replace("+244", "").Replace(" ", "");
            return numero.Length == 9 && numero.All(char.IsDigit);
        }

        /// <summary>
        /// Obtém a descrição do tipo de documento
        /// </summary>
        /// <param name="tipo">Tipo de documento</param>
        /// <returns>Descrição do tipo</returns>
        public static string GetDescricaoTipoDocumento(TipoDocumentoEnum tipo)
        {
            switch (tipo)
            {
                case TipoDocumentoEnum.BI:
                    return "Bilhete de Identidade";
                case TipoDocumentoEnum.Passaporte:
                    return "Passaporte";
                case TipoDocumentoEnum.CartaoResidencia:
                    return "Cartão de Residência";
                case TipoDocumentoEnum.Outro:
                    return "Outro";
                default:
                    return "Não especificado";
            }
        }
        
        #endregion

        #region Sobrescrita de Métodos
        
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   Nome.Length >= 2 && Nome.Length <= 150 &&
                   EmailValido() &&
                   TelefoneValido();
        }

        public override string[] GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(Nome))
                errors.Add("Nome do cliente é obrigatório");
            else if (Nome.Length < 2 || Nome.Length > 150)
                errors.Add("Nome deve ter entre 2 e 150 caracteres");

            if (!EmailValido())
                errors.Add("Email deve ter um formato válido");

            if (!TelefoneValido())
                errors.Add("Telefone deve ter um formato válido (9 dígitos)");

            if (DataNascimento.HasValue && DataNascimento.Value > DateTime.Today)
                errors.Add("Data de nascimento não pode ser futura");

            return errors.ToArray();
        }

        public override string ToString()
        {
            var info = Nome;
            if (!string.IsNullOrWhiteSpace(Telefone))
                info += $" - {TelefoneFormatado}";
            return info;
        }
        
        #endregion
    }

    #region Enumerações
    
    /// <summary>
    /// Tipos de documento disponíveis
    /// </summary>
    public enum TipoDocumentoEnum
    {
        BI = 1,
        Passaporte = 2,
        CartaoResidencia = 3,
        Outro = 4
    }
    
    #endregion
}