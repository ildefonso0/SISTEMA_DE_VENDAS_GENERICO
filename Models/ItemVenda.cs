using System;
using System.ComponentModel.DataAnnotations;

namespace SISTEMA_DE_VENDAS_GENERICO.Models
{
    /// <summary>
    /// Modelo que representa um item de venda
    /// </summary>
    public class ItemVenda : BaseModel
    {
        #region Campos Privados
        
        private int _idVenda;
        private int _idProduto;
        private int _quantidade;
        private decimal _precoUnitario;
        private decimal _totalItem;
        private decimal _desconto;
        
        // Propriedades de navegação
        private string _nomeProduto;
        private string _codigoBarra;
        private string _unidade;
        
        #endregion

        #region Propriedades Públicas
        
        /// <summary>
        /// ID da venda à qual este item pertence
        /// </summary>
        [Required(ErrorMessage = "ID da venda é obrigatório")]
        public int IdVenda
        {
            get => _idVenda;
            set => SetProperty(ref _idVenda, value);
        }

        /// <summary>
        /// ID do produto vendido
        /// </summary>
        [Required(ErrorMessage = "Produto é obrigatório")]
        public int IdProduto
        {
            get => _idProduto;
            set => SetProperty(ref _idProduto, value);
        }

        /// <summary>
        /// Quantidade vendida do produto
        /// </summary>
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade
        {
            get => _quantidade;
            set
            {
                if (SetProperty(ref _quantidade, value))
                {
                    RecalcularTotal();
                }
            }
        }

        /// <summary>
        /// Preço unitário do produto no momento da venda
        /// </summary>
        [Required(ErrorMessage = "Preço unitário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser maior que zero")]
        public decimal PrecoUnitario
        {
            get => _precoUnitario;
            set
            {
                if (SetProperty(ref _precoUnitario, Math.Round(value, 2)))
                {
                    RecalcularTotal();
                }
            }
        }

        /// <summary>
        /// Total do item (quantidade × preço unitário - desconto)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Total do item não pode ser negativo")]
        public decimal TotalItem
        {
            get => _totalItem;
            set => SetProperty(ref _totalItem, Math.Round(value, 2));
        }

        /// <summary>
        /// Desconto aplicado especificamente a este item
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Desconto não pode ser negativo")]
        public decimal Desconto
        {
            get => _desconto;
            set
            {
                if (SetProperty(ref _desconto, Math.Round(value, 2)))
                {
                    RecalcularTotal();
                }
            }
        }

        /// <summary>
        /// Nome do produto (propriedade de navegação)
        /// </summary>
        public string NomeProduto
        {
            get => _nomeProduto;
            set => SetProperty(ref _nomeProduto, value);
        }

        /// <summary>
        /// Código de barras do produto (propriedade de navegação)
        /// </summary>
        public string CodigoBarra
        {
            get => _codigoBarra;
            set => SetProperty(ref _codigoBarra, value);
        }

        /// <summary>
        /// Unidade de medida do produto (propriedade de navegação)
        /// </summary>
        public string Unidade
        {
            get => _unidade;
            set => SetProperty(ref _unidade, value);
        }

        /// <summary>
        /// Subtotal do item (antes do desconto)
        /// </summary>
        public decimal Subtotal => Math.Round(Quantidade * PrecoUnitario, 2);

        /// <summary>
        /// Percentual de desconto aplicado ao item
        /// </summary>
        public decimal PercentualDesconto
        {
            get
            {
                if (Subtotal <= 0) return 0;
                return Math.Round((Desconto / Subtotal) * 100, 2);
            }
        }

        /// <summary>
        /// Preço unitário formatado para exibição
        /// </summary>
        public string PrecoUnitarioFormatado => $"{PrecoUnitario:N2} AOA";

        /// <summary>
        /// Total formatado para exibição
        /// </summary>
        public string TotalFormatado => $"{TotalItem:N2} AOA";

        /// <summary>
        /// Subtotal formatado para exibição
        /// </summary>
        public string SubtotalFormatado => $"{Subtotal:N2} AOA";

        /// <summary>
        /// Desconto formatado para exibição
        /// </summary>
        public string DescontoFormatado => $"{Desconto:N2} AOA";

        /// <summary>
        /// Descrição completa do item para exibição
        /// </summary>
        public string DescricaoCompleta
        {
            get
            {
                var descricao = $"{NomeProduto} - {Quantidade} {Unidade}";
                if (!string.IsNullOrWhiteSpace(CodigoBarra))
                    descricao += $" ({CodigoBarra})";
                return descricao;
            }
        }
        
        #endregion

        #region Construtor
        
        public ItemVenda()
        {
            Quantidade = 1;
            PrecoUnitario = 0;
            Desconto = 0;
            TotalItem = 0;
        }

        /// <summary>
        /// Construtor com parâmetros básicos
        /// </summary>
        /// <param name="idProduto">ID do produto</param>
        /// <param name="quantidade">Quantidade</param>
        /// <param name="precoUnitario">Preço unitário</param>
        public ItemVenda(int idProduto, int quantidade, decimal precoUnitario) : this()
        {
            IdProduto = idProduto;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
            RecalcularTotal();
        }
        
        #endregion

        #region Métodos Públicos
        
        /// <summary>
        /// Recalcula o total do item baseado na quantidade, preço unitário e desconto
        /// </summary>
        public void RecalcularTotal()
        {
            var subtotal = Quantidade * PrecoUnitario;
            
            // Garante que o desconto não seja maior que o subtotal
            if (Desconto > subtotal)
                Desconto = subtotal;
            
            TotalItem = subtotal - Desconto;
        }

        /// <summary>
        /// Aplica desconto ao item
        /// </summary>
        /// <param name="valorDesconto">Valor do desconto</param>
        /// <param name="isPercentual">Se true, o valor é tratado como percentual</param>
        public void AplicarDesconto(decimal valorDesconto, bool isPercentual = false)
        {
            if (valorDesconto < 0) return;
            
            var subtotal = Subtotal;
            
            if (isPercentual)
            {
                if (valorDesconto > 100) valorDesconto = 100;
                Desconto = Math.Round((subtotal * valorDesconto) / 100, 2);
            }
            else
            {
                Desconto = Math.Min(valorDesconto, subtotal);
            }
            
            RecalcularTotal();
        }

        /// <summary>
        /// Aumenta a quantidade do item
        /// </summary>
        /// <param name="quantidade">Quantidade a ser adicionada</param>
        public void AdicionarQuantidade(int quantidade)
        {
            if (quantidade > 0)
            {
                Quantidade += quantidade;
            }
        }

        /// <summary>
        /// Diminui a quantidade do item
        /// </summary>
        /// <param name="quantidade">Quantidade a ser removida</param>
        /// <returns>True se a operação foi bem-sucedida</returns>
        public bool RemoverQuantidade(int quantidade)
        {
            if (quantidade > 0 && Quantidade > quantidade)
            {
                Quantidade -= quantidade;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cria uma cópia do item
        /// </summary>
        /// <returns>Nova instância do ItemVenda</returns>
        public ItemVenda Clonar()
        {
            return new ItemVenda
            {
                IdVenda = IdVenda,
                IdProduto = IdProduto,
                Quantidade = Quantidade,
                PrecoUnitario = PrecoUnitario,
                Desconto = Desconto,
                NomeProduto = NomeProduto,
                CodigoBarra = CodigoBarra,
                Unidade = Unidade
            };
        }
        
        #endregion

        #region Sobrescrita de Métodos
        
        public override bool IsValid()
        {
            return IdProduto > 0 &&
                   Quantidade > 0 &&
                   PrecoUnitario > 0 &&
                   Desconto >= 0 &&
                   TotalItem >= 0 &&
                   Desconto <= Subtotal;
        }

        public override string[] GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (IdProduto <= 0)
                errors.Add("Produto é obrigatório");

            if (Quantidade <= 0)
                errors.Add("Quantidade deve ser maior que zero");

            if (PrecoUnitario <= 0)
                errors.Add("Preço unitário deve ser maior que zero");

            if (Desconto < 0)
                errors.Add("Desconto não pode ser negativo");

            if (TotalItem < 0)
                errors.Add("Total do item não pode ser negativo");

            if (Desconto > Subtotal)
                errors.Add("Desconto não pode ser maior que o subtotal do item");

            return errors.ToArray();
        }

        public override string ToString()
        {
            return $"{NomeProduto} - Qtd: {Quantidade} - Total: {TotalFormatado}";
        }
        
        #endregion
    }
}