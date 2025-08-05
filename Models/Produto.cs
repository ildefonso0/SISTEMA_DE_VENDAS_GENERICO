using System;
using System.ComponentModel.DataAnnotations;

namespace SISTEMA_DE_VENDAS_GENERICO.Models
{
    /// <summary>
    /// Modelo que representa um produto no sistema
    /// </summary>
    public class Produto : BaseModel
    {
        #region Campos Privados
        
        private string _nome;
        private string _codigoBarra;
        private int _idCategoria;
        private decimal _precoCompra;
        private decimal _precoVenda;
        private int _estoqueAtual;
        private int _estoqueMinimo;
        private string _unidade;
        private byte[] _imagem;
        private string _observacoes;
        
        // Propriedades de navegação
        private string _nomeCategoria;
        
        #endregion

        #region Propriedades Públicas
        
        /// <summary>
        /// Nome do produto
        /// </summary>
        [Required(ErrorMessage = "Nome do produto é obrigatório")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 150 caracteres")]
        public string Nome
        {
            get => _nome;
            set => SetProperty(ref _nome, value?.Trim());
        }

        /// <summary>
        /// Código de barras do produto (opcional)
        /// </summary>
        [StringLength(50, ErrorMessage = "Código de barras deve ter no máximo 50 caracteres")]
        public string CodigoBarra
        {
            get => _codigoBarra;
            set => SetProperty(ref _codigoBarra, value?.Trim());
        }

        /// <summary>
        /// ID da categoria do produto
        /// </summary>
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int IdCategoria
        {
            get => _idCategoria;
            set => SetProperty(ref _idCategoria, value);
        }

        /// <summary>
        /// Preço de compra do produto em Kwanza (AOA)
        /// </summary>
        [Required(ErrorMessage = "Preço de compra é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço de compra deve ser maior que zero")]
        public decimal PrecoCompra
        {
            get => _precoCompra;
            set => SetProperty(ref _precoCompra, Math.Round(value, 2));
        }

        /// <summary>
        /// Preço de venda do produto em Kwanza (AOA)
        /// </summary>
        [Required(ErrorMessage = "Preço de venda é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço de venda deve ser maior que zero")]
        public decimal PrecoVenda
        {
            get => _precoVenda;
            set => SetProperty(ref _precoVenda, Math.Round(value, 2));
        }

        /// <summary>
        /// Quantidade atual em estoque
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Estoque não pode ser negativo")]
        public int EstoqueAtual
        {
            get => _estoqueAtual;
            set => SetProperty(ref _estoqueAtual, value);
        }

        /// <summary>
        /// Estoque mínimo para alerta
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Estoque mínimo não pode ser negativo")]
        public int EstoqueMinimo
        {
            get => _estoqueMinimo;
            set => SetProperty(ref _estoqueMinimo, value);
        }

        /// <summary>
        /// Unidade de medida do produto
        /// </summary>
        [Required(ErrorMessage = "Unidade é obrigatória")]
        [StringLength(20, ErrorMessage = "Unidade deve ter no máximo 20 caracteres")]
        public string Unidade
        {
            get => _unidade;
            set => SetProperty(ref _unidade, value?.Trim());
        }

        /// <summary>
        /// Imagem do produto (opcional)
        /// </summary>
        public byte[] Imagem
        {
            get => _imagem;
            set => SetProperty(ref _imagem, value);
        }

        /// <summary>
        /// Observações sobre o produto
        /// </summary>
        [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        public string Observacoes
        {
            get => _observacoes;
            set => SetProperty(ref _observacoes, value?.Trim());
        }

        /// <summary>
        /// Nome da categoria (propriedade de navegação)
        /// </summary>
        public string NomeCategoria
        {
            get => _nomeCategoria;
            set => SetProperty(ref _nomeCategoria, value);
        }

        /// <summary>
        /// Indica se o produto está com estoque baixo
        /// </summary>
        public bool EstoqueBaixo => EstoqueAtual <= EstoqueMinimo;

        /// <summary>
        /// Margem de lucro do produto (percentual)
        /// </summary>
        public decimal MargemLucro
        {
            get
            {
                if (PrecoCompra <= 0) return 0;
                return Math.Round(((PrecoVenda - PrecoCompra) / PrecoCompra) * 100, 2);
            }
        }

        /// <summary>
        /// Valor do lucro unitário
        /// </summary>
        public decimal LucroUnitario => Math.Round(PrecoVenda - PrecoCompra, 2);

        /// <summary>
        /// Preço de venda formatado para exibição
        /// </summary>
        public string PrecoVendaFormatado => $"{PrecoVenda:N2} AOA";

        /// <summary>
        /// Preço de compra formatado para exibição
        /// </summary>
        public string PrecoCompraFormatado => $"{PrecoCompra:N2} AOA";
        
        #endregion

        #region Construtor
        
        public Produto()
        {
            Unidade = "Unidade";
            EstoqueMinimo = 5; // Estoque mínimo padrão
            PrecoCompra = 0;
            PrecoVenda = 0;
            EstoqueAtual = 0;
        }
        
        #endregion

        #region Métodos Públicos
        
        /// <summary>
        /// Verifica se o produto pode ser vendido
        /// </summary>
        /// <param name="quantidade">Quantidade desejada</param>
        /// <returns>True se há estoque suficiente</returns>
        public bool PodeVender(int quantidade)
        {
            return Ativo && EstoqueAtual >= quantidade && quantidade > 0;
        }

        /// <summary>
        /// Calcula o valor total para uma quantidade específica
        /// </summary>
        /// <param name="quantidade">Quantidade</param>
        /// <returns>Valor total</returns>
        public decimal CalcularValorTotal(int quantidade)
        {
            return Math.Round(PrecoVenda * quantidade, 2);
        }

        /// <summary>
        /// Atualiza o estoque do produto
        /// </summary>
        /// <param name="quantidade">Quantidade (positiva para entrada, negativa para saída)</param>
        /// <returns>True se a operação foi bem-sucedida</returns>
        public bool AtualizarEstoque(int quantidade)
        {
            int novoEstoque = EstoqueAtual + quantidade;
            
            if (novoEstoque < 0)
                return false; // Não permite estoque negativo
            
            EstoqueAtual = novoEstoque;
            return true;
        }
        
        #endregion

        #region Sobrescrita de Métodos
        
        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Nome) &&
                   !string.IsNullOrWhiteSpace(Unidade) &&
                   IdCategoria > 0 &&
                   PrecoCompra > 0 &&
                   PrecoVenda > 0 &&
                   EstoqueAtual >= 0 &&
                   EstoqueMinimo >= 0;
        }

        public override string[] GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(Nome))
                errors.Add("Nome do produto é obrigatório");
            else if (Nome.Length < 2 || Nome.Length > 150)
                errors.Add("Nome deve ter entre 2 e 150 caracteres");

            if (string.IsNullOrWhiteSpace(Unidade))
                errors.Add("Unidade é obrigatória");

            if (IdCategoria <= 0)
                errors.Add("Categoria é obrigatória");

            if (PrecoCompra <= 0)
                errors.Add("Preço de compra deve ser maior que zero");

            if (PrecoVenda <= 0)
                errors.Add("Preço de venda deve ser maior que zero");

            if (PrecoVenda < PrecoCompra)
                errors.Add("Preço de venda não pode ser menor que o preço de compra");

            if (EstoqueAtual < 0)
                errors.Add("Estoque atual não pode ser negativo");

            if (EstoqueMinimo < 0)
                errors.Add("Estoque mínimo não pode ser negativo");

            return errors.ToArray();
        }

        public override string ToString()
        {
            return $"{Nome} - {PrecoVendaFormatado}";
        }
        
        #endregion
    }
}