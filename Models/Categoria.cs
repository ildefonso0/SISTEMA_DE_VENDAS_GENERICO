using System.ComponentModel.DataAnnotations;

namespace SISTEMA_DE_VENDAS_GENERICO.Models
{
    /// <summary>
    /// Modelo que representa uma categoria de produtos
    /// </summary>
    public class Categoria : BaseModel
    {
        #region Campos Privados
        
        private string _nome;
        private string _descricao;
        private string _cor;
        private string _icone;
        
        #endregion

        #region Propriedades Públicas
        
        /// <summary>
        /// Nome da categoria
        /// </summary>
        [Required(ErrorMessage = "Nome da categoria é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        public string Nome
        {
            get => _nome;
            set => SetProperty(ref _nome, value?.Trim());
        }

        /// <summary>
        /// Descrição detalhada da categoria
        /// </summary>
        [StringLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
        public string Descricao
        {
            get => _descricao;
            set => SetProperty(ref _descricao, value?.Trim());
        }

        /// <summary>
        /// Cor associada à categoria (formato hexadecimal)
        /// </summary>
        [StringLength(7, ErrorMessage = "Cor deve estar no formato #RRGGBB")]
        public string Cor
        {
            get => _cor;
            set => SetProperty(ref _cor, ValidarCor(value));
        }

        /// <summary>
        /// Ícone da categoria (emoji ou código)
        /// </summary>
        [StringLength(10, ErrorMessage = "Ícone deve ter no máximo 10 caracteres")]
        public string Icone
        {
            get => _icone;
            set => SetProperty(ref _icone, value?.Trim());
        }

        /// <summary>
        /// Nome completo da categoria com ícone
        /// </summary>
        public string NomeCompleto
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Icone))
                    return $"{Icone} {Nome}";
                return Nome;
            }
        }

        /// <summary>
        /// Cor padrão se não foi definida
        /// </summary>
        public string CorExibicao => string.IsNullOrWhiteSpace(Cor) ? "#3498DB" : Cor;
        
        #endregion

        #region Construtor
        
        public Categoria()
        {
            Cor = "#3498DB"; // Azul padrão
            Icone = "📦"; // Ícone padrão
        }
        
        #endregion

        #region Métodos Privados
        
        /// <summary>
        /// Valida e formata a cor no formato hexadecimal
        /// </summary>
        /// <param name="cor">Cor a ser validada</param>
        /// <returns>Cor formatada ou valor padrão</returns>
        private string ValidarCor(string cor)
        {
            if (string.IsNullOrWhiteSpace(cor))
                return "#3498DB";
            
            cor = cor.Trim().ToUpper();
            
            // Adiciona # se não tiver
            if (!cor.StartsWith("#"))
                cor = "#" + cor;
            
            // Verifica se tem o formato correto
            if (cor.Length == 7 && System.Text.RegularExpressions.Regex.IsMatch(cor, @"^#[0-9A-F]{6}$"))
                return cor;
            
            return "#3498DB"; // Retorna cor padrão se inválida
        }
        
        #endregion

        #region Métodos Públicos
        
        /// <summary>
        /// Verifica se a categoria pode ser excluída
        /// (não deve ter produtos associados)
        /// </summary>
        /// <returns>True se pode ser excluída</returns>
        public bool PodeSerExcluida()
        {
            // Esta verificação deve ser feita no serviço/repositório
            // que tem acesso ao banco de dados
            return true;
        }

        /// <summary>
        /// Obtém uma lista de ícones sugeridos para categorias
        /// </summary>
        /// <returns>Array de ícones</returns>
        public static string[] GetIconesSugeridos()
        {
            return new[]
            {
                "📦", "🍔", "🥤", "🍞", "🥛", "🧴", "🧽", "💊", "👕", "📱",
                "💻", "🏠", "🚗", "📚", "🎮", "🎵", "🏃", "🌱", "🔧", "🎨"
            };
        }

        /// <summary>
        /// Obtém uma lista de cores sugeridas para categorias
        /// </summary>
        /// <returns>Array de cores em formato hexadecimal</returns>
        public static string[] GetCoresSugeridas()
        {
            return new[]
            {
                "#3498DB", "#E74C3C", "#2ECC71", "#F39C12", "#9B59B6",
                "#1ABC9C", "#34495E", "#E67E22", "#95A5A6", "#16A085",
                "#27AE60", "#2980B9", "#8E44AD", "#F1C40F", "#E8F5E8"
            };
        }
        
        #endregion

        #region Sobrescrita de Métodos
        
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   Nome.Length >= 2 && Nome.Length <= 100;
        }

        public override string[] GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(Nome))
                errors.Add("Nome da categoria é obrigatório");
            else if (Nome.Length < 2 || Nome.Length > 100)
                errors.Add("Nome deve ter entre 2 e 100 caracteres");

            if (!string.IsNullOrWhiteSpace(Descricao) && Descricao.Length > 255)
                errors.Add("Descrição deve ter no máximo 255 caracteres");

            return errors.ToArray();
        }

        public override string ToString()
        {
            return NomeCompleto;
        }
        
        #endregion
    }
}