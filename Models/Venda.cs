using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SISTEMA_DE_VENDAS_GENERICO.Models
{
    /// <summary>
    /// Modelo que representa uma venda no sistema
    /// </summary>
    public class Venda : BaseModel
    {
        #region Campos Privados
        
        private int? _idCliente;
        private int _idUsuario;
        private DateTime _dataVenda;
        private decimal _subtotal;
        private decimal _desconto;
        private decimal _total;
        private string _tipoPagamento;
        private StatusVendaEnum _status;
        private string _observacoes;
        private string _numeroFatura;
        
        // Propriedades de navegação
        private string _nomeCliente;
        private string _nomeUsuario;
        private List<ItemVenda> _itens;
        
        #endregion

        #region Propriedades Públicas
        
        /// <summary>
        /// ID do cliente (opcional - venda pode ser sem cliente identificado)
        /// </summary>
        public int? IdCliente
        {
            get => _idCliente;
            set => SetProperty(ref _idCliente, value);
        }

        /// <summary>
        /// ID do usuário que realizou a venda
        /// </summary>
        [Required(ErrorMessage = "Usuário é obrigatório")]
        public int IdUsuario
        {
            get => _idUsuario;
            set => SetProperty(ref _idUsuario, value);
        }

        /// <summary>
        /// Data e hora da venda
        /// </summary>
        public DateTime DataVenda
        {
            get => _dataVenda;
            set => SetProperty(ref _dataVenda, value);
        }

        /// <summary>
        /// Subtotal da venda (antes do desconto)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Subtotal não pode ser negativo")]
        public decimal Subtotal
        {
            get => _subtotal;
            set => SetProperty(ref _subtotal, Math.Round(value, 2));
        }

        /// <summary>
        /// Valor do desconto aplicado
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Desconto não pode ser negativo")]
        public decimal Desconto
        {
            get => _desconto;
            set => SetProperty(ref _desconto, Math.Round(value, 2));
        }

        /// <summary>
        /// Valor total da venda (subtotal - desconto)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Total não pode ser negativo")]
        public decimal Total
        {
            get => _total;
            set => SetProperty(ref _total, Math.Round(value, 2));
        }

        /// <summary>
        /// Tipo/forma de pagamento utilizada
        /// </summary>
        [Required(ErrorMessage = "Tipo de pagamento é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo de pagamento deve ter no máximo 50 caracteres")]
        public string TipoPagamento
        {
            get => _tipoPagamento;
            set => SetProperty(ref _tipoPagamento, value?.Trim());
        }

        /// <summary>
        /// Status atual da venda
        /// </summary>
        public StatusVendaEnum Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Observações sobre a venda
        /// </summary>
        [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        public string Observacoes
        {
            get => _observacoes;
            set => SetProperty(ref _observacoes, value?.Trim());
        }

        /// <summary>
        /// Número da fatura gerada
        /// </summary>
        [StringLength(50, ErrorMessage = "Número da fatura deve ter no máximo 50 caracteres")]
        public string NumeroFatura
        {
            get => _numeroFatura;
            set => SetProperty(ref _numeroFatura, value?.Trim()?.ToUpper());
        }

        /// <summary>
        /// Nome do cliente (propriedade de navegação)
        /// </summary>
        public string NomeCliente
        {
            get => _nomeCliente ?? "Cliente não identificado";
            set => SetProperty(ref _nomeCliente, value);
        }

        /// <summary>
        /// Nome do usuário que realizou a venda (propriedade de navegação)
        /// </summary>
        public string NomeUsuario
        {
            get => _nomeUsuario;
            set => SetProperty(ref _nomeUsuario, value);
        }

        /// <summary>
        /// Lista de itens da venda
        /// </summary>
        public List<ItemVenda> Itens
        {
            get => _itens ?? (_itens = new List<ItemVenda>());
            set => SetProperty(ref _itens, value);
        }

        /// <summary>
        /// Quantidade total de itens na venda
        /// </summary>
        public int QuantidadeItens => Itens?.Sum(i => i.Quantidade) ?? 0;

        /// <summary>
        /// Percentual de desconto aplicado
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
        /// Descrição do status da venda
        /// </summary>
        public string DescricaoStatus => GetDescricaoStatus(Status);

        /// <summary>
        /// Total formatado para exibição
        /// </summary>
        public string TotalFormatado => $"{Total:N2} AOA";

        /// <summary>
        /// Subtotal formatado para exibição
        /// </summary>
        public string SubtotalFormatado => $"{Subtotal:N2} AOA";

        /// <summary>
        /// Desconto formatado para exibição
        /// </summary>
        public string DescontoFormatado => $"{Desconto:N2} AOA";
        
        #endregion

        #region Construtor
        
        public Venda()
        {
            DataVenda = DateTime.Now;
            Status = StatusVendaEnum.Pendente;
            Subtotal = 0;
            Desconto = 0;
            Total = 0;
            Itens = new List<ItemVenda>();
        }
        
        #endregion

        #region Métodos Públicos
        
        /// <summary>
        /// Adiciona um item à venda
        /// </summary>
        /// <param name="item">Item a ser adicionado</param>
        public void AdicionarItem(ItemVenda item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            var itemExistente = Itens.FirstOrDefault(i => i.IdProduto == item.IdProduto);
            if (itemExistente != null)
            {
                itemExistente.Quantidade += item.Quantidade;
                itemExistente.RecalcularTotal();
            }
            else
            {
                item.IdVenda = Id;
                Itens.Add(item);
            }
            
            RecalcularTotais();
        }

        /// <summary>
        /// Remove um item da venda
        /// </summary>
        /// <param name="item">Item a ser removido</param>
        public void RemoverItem(ItemVenda item)
        {
            if (item != null && Itens.Remove(item))
            {
                RecalcularTotais();
            }
        }

        /// <summary>
        /// Recalcula os totais da venda
        /// </summary>
        public void RecalcularTotais()
        {
            Subtotal = Itens?.Sum(i => i.TotalItem) ?? 0;
            
            // Garante que o desconto não seja maior que o subtotal
            if (Desconto > Subtotal)
                Desconto = Subtotal;
            
            Total = Subtotal - Desconto;
        }

        /// <summary>
        /// Aplica desconto à venda
        /// </summary>
        /// <param name="valorDesconto">Valor do desconto</param>
        /// <param name="isPercentual">Se true, o valor é tratado como percentual</param>
        public void AplicarDesconto(decimal valorDesconto, bool isPercentual = false)
        {
            if (valorDesconto < 0) return;
            
            if (isPercentual)
            {
                if (valorDesconto > 100) valorDesconto = 100;
                Desconto = Math.Round((Subtotal * valorDesconto) / 100, 2);
            }
            else
            {
                Desconto = Math.Min(valorDesconto, Subtotal);
            }
            
            RecalcularTotais();
        }

        /// <summary>
        /// Finaliza a venda
        /// </summary>
        public void Finalizar()
        {
            if (Itens == null || !Itens.Any())
                throw new InvalidOperationException("Não é possível finalizar uma venda sem itens");
            
            if (string.IsNullOrWhiteSpace(TipoPagamento))
                throw new InvalidOperationException("Tipo de pagamento é obrigatório");
            
            Status = StatusVendaEnum.Finalizada;
            
            // Gera número da fatura se não foi definido
            if (string.IsNullOrWhiteSpace(NumeroFatura))
            {
                NumeroFatura = GerarNumeroFatura();
            }
        }

        /// <summary>
        /// Cancela a venda
        /// </summary>
        /// <param name="motivo">Motivo do cancelamento</param>
        public void Cancelar(string motivo = null)
        {
            Status = StatusVendaEnum.Cancelada;
            
            if (!string.IsNullOrWhiteSpace(motivo))
            {
                Observacoes = string.IsNullOrWhiteSpace(Observacoes) 
                    ? $"Cancelada: {motivo}" 
                    : $"{Observacoes}\nCancelada: {motivo}";
            }
        }

        /// <summary>
        /// Gera um número único para a fatura
        /// </summary>
        /// <returns>Número da fatura</returns>
        private string GerarNumeroFatura()
        {
            return $"FT{DateTime.Now:yyyyMMdd}{Id:D6}";
        }

        /// <summary>
        /// Obtém a descrição do status da venda
        /// </summary>
        /// <param name="status">Status da venda</param>
        /// <returns>Descrição do status</returns>
        public static string GetDescricaoStatus(StatusVendaEnum status)
        {
            switch (status)
            {
                case StatusVendaEnum.Pendente:
                    return "Pendente";
                case StatusVendaEnum.Finalizada:
                    return "Finalizada";
                case StatusVendaEnum.Cancelada:
                    return "Cancelada";
                default:
                    return "Indefinido";
            }
        }
        
        #endregion

        #region Sobrescrita de Métodos
        
        public override bool IsValid()
        {
            return IdUsuario > 0 &&
                   !string.IsNullOrWhiteSpace(TipoPagamento) &&
                   Subtotal >= 0 &&
                   Desconto >= 0 &&
                   Total >= 0 &&
                   Desconto <= Subtotal &&
                   Itens != null && Itens.Any() &&
                   Itens.All(i => i.IsValid());
        }

        public override string[] GetValidationErrors()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (IdUsuario <= 0)
                errors.Add("Usuário é obrigatório");

            if (string.IsNullOrWhiteSpace(TipoPagamento))
                errors.Add("Tipo de pagamento é obrigatório");

            if (Subtotal < 0)
                errors.Add("Subtotal não pode ser negativo");

            if (Desconto < 0)
                errors.Add("Desconto não pode ser negativo");

            if (Total < 0)
                errors.Add("Total não pode ser negativo");

            if (Desconto > Subtotal)
                errors.Add("Desconto não pode ser maior que o subtotal");

            if (Itens == null || !Itens.Any())
                errors.Add("Venda deve ter pelo menos um item");
            else
            {
                foreach (var item in Itens)
                {
                    var itemErrors = item.GetValidationErrors();
                    errors.AddRange(itemErrors.Select(e => $"Item {item.NomeProduto}: {e}"));
                }
            }

            return errors.ToArray();
        }

        public override string ToString()
        {
            return $"Venda #{Id} - {DataVenda:dd/MM/yyyy HH:mm} - {TotalFormatado}";
        }
        
        #endregion
    }

    #region Enumerações
    
    /// <summary>
    /// Status possíveis para uma venda
    /// </summary>
    public enum StatusVendaEnum
    {
        Pendente = 1,
        Finalizada = 2,
        Cancelada = 3
    }
    
    #endregion
}