using System;
using System.Globalization;

namespace SISTEMA_DE_VENDAS_GENERICO.Utils
{
    /// <summary>
    /// Classe auxiliar para formatação de moeda angolana (Kwanza - AOA)
    /// </summary>
    public static class CurrencyHelper
    {
        #region Constantes
        
        /// <summary>
        /// Código da moeda angolana
        /// </summary>
        public const string CURRENCY_CODE = "AOA";
        
        /// <summary>
        /// Símbolo da moeda angolana
        /// </summary>
        public const string CURRENCY_SYMBOL = "Kz";
        
        /// <summary>
        /// Nome completo da moeda
        /// </summary>
        public const string CURRENCY_NAME = "Kwanza Angolano";
        
        #endregion

        #region Propriedades Estáticas
        
        /// <summary>
        /// Cultura personalizada para Angola
        /// </summary>
        private static readonly CultureInfo _angolaCulture;
        
        #endregion

        #region Construtor Estático
        
        static CurrencyHelper()
        {
            // Cria uma cultura personalizada baseada no português
            _angolaCulture = new CultureInfo("pt-PT");
            
            // Personaliza as configurações de moeda para Angola
            _angolaCulture.NumberFormat.CurrencySymbol = CURRENCY_SYMBOL;
            _angolaCulture.NumberFormat.CurrencyDecimalDigits = 2;
            _angolaCulture.NumberFormat.CurrencyDecimalSeparator = ",";
            _angolaCulture.NumberFormat.CurrencyGroupSeparator = ".";
            _angolaCulture.NumberFormat.CurrencyPositivePattern = 3; // n $
            _angolaCulture.NumberFormat.CurrencyNegativePattern = 8; // -n $
        }
        
        #endregion

        #region Métodos Públicos
        
        /// <summary>
        /// Formata um valor decimal como moeda angolana
        /// </summary>
        /// <param name="valor">Valor a ser formatado</param>
        /// <param name="incluirSimbolo">Se deve incluir o símbolo da moeda</param>
        /// <returns>Valor formatado</returns>
        public static string FormatarMoeda(decimal valor, bool incluirSimbolo = true)
        {
            if (incluirSimbolo)
            {
                return valor.ToString("C", _angolaCulture);
            }
            else
            {
                return valor.ToString("N2", _angolaCulture);
            }
        }

        /// <summary>
        /// Formata um valor decimal como moeda angolana com código
        /// </summary>
        /// <param name="valor">Valor a ser formatado</param>
        /// <returns>Valor formatado com código AOA</returns>
        public static string FormatarMoedaComCodigo(decimal valor)
        {
            return $"{valor.ToString("N2", _angolaCulture)} {CURRENCY_CODE}";
        }

        /// <summary>
        /// Converte uma string para decimal, considerando o formato angolano
        /// </summary>
        /// <param name="valorTexto">Texto a ser convertido</param>
        /// <param name="valor">Valor convertido (saída)</param>
        /// <returns>True se a conversão foi bem-sucedida</returns>
        public static bool TentarConverterMoeda(string valorTexto, out decimal valor)
        {
            valor = 0;
            
            if (string.IsNullOrWhiteSpace(valorTexto))
                return false;
            
            // Remove símbolos de moeda e espaços
            string textoLimpo = valorTexto
                .Replace(CURRENCY_SYMBOL, "")
                .Replace(CURRENCY_CODE, "")
                .Trim();
            
            // Tenta converter usando a cultura angolana
            if (decimal.TryParse(textoLimpo, NumberStyles.Currency, _angolaCulture, out valor))
                return true;
            
            // Tenta converter usando a cultura invariante como fallback
            return decimal.TryParse(textoLimpo, NumberStyles.Currency, CultureInfo.InvariantCulture, out valor);
        }

        /// <summary>
        /// Converte uma string para decimal (lança exceção se falhar)
        /// </summary>
        /// <param name="valorTexto">Texto a ser convertido</param>
        /// <returns>Valor convertido</returns>
        /// <exception cref="FormatException">Se o formato for inválido</exception>
        public static decimal ConverterMoeda(string valorTexto)
        {
            if (TentarConverterMoeda(valorTexto, out decimal valor))
                return valor;
            
            throw new FormatException($"Não foi possível converter '{valorTexto}' para um valor monetário válido.");
        }

        /// <summary>
        /// Valida se uma string representa um valor monetário válido
        /// </summary>
        /// <param name="valorTexto">Texto a ser validado</param>
        /// <returns>True se é um valor monetário válido</returns>
        public static bool ValidarMoeda(string valorTexto)
        {
            return TentarConverterMoeda(valorTexto, out _);
        }

        /// <summary>
        /// Arredonda um valor para duas casas decimais (padrão monetário)
        /// </summary>
        /// <param name="valor">Valor a ser arredondado</param>
        /// <returns>Valor arredondado</returns>
        public static decimal ArredondarMoeda(decimal valor)
        {
            return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Calcula o percentual entre dois valores monetários
        /// </summary>
        /// <param name="valor">Valor base</param>
        /// <param name="total">Valor total</param>
        /// <returns>Percentual calculado</returns>
        public static decimal CalcularPercentual(decimal valor, decimal total)
        {
            if (total == 0) return 0;
            return ArredondarMoeda((valor / total) * 100);
        }

        /// <summary>
        /// Calcula o valor de um percentual sobre um montante
        /// </summary>
        /// <param name="montante">Montante base</param>
        /// <param name="percentual">Percentual a ser aplicado</param>
        /// <returns>Valor calculado</returns>
        public static decimal CalcularValorPercentual(decimal montante, decimal percentual)
        {
            return ArredondarMoeda((montante * percentual) / 100);
        }

        /// <summary>
        /// Formata um valor para exibição em relatórios
        /// </summary>
        /// <param name="valor">Valor a ser formatado</param>
        /// <param name="incluirCodigo">Se deve incluir o código da moeda</param>
        /// <returns>Valor formatado para relatório</returns>
        public static string FormatarParaRelatorio(decimal valor, bool incluirCodigo = false)
        {
            if (incluirCodigo)
                return FormatarMoedaComCodigo(valor);
            else
                return FormatarMoeda(valor);
        }

        /// <summary>
        /// Obtém informações sobre a moeda angolana
        /// </summary>
        /// <returns>Objeto com informações da moeda</returns>
        public static CurrencyInfo ObterInformacoesMoeda()
        {
            return new CurrencyInfo
            {
                Codigo = CURRENCY_CODE,
                Simbolo = CURRENCY_SYMBOL,
                Nome = CURRENCY_NAME,
                CasasDecimais = 2,
                SeparadorDecimal = ",",
                SeparadorMilhar = "."
            };
        }
        
        #endregion
    }

    #region Classes Auxiliares
    
    /// <summary>
    /// Informações sobre uma moeda
    /// </summary>
    public class CurrencyInfo
    {
        public string Codigo { get; set; }
        public string Simbolo { get; set; }
        public string Nome { get; set; }
        public int CasasDecimais { get; set; }
        public string SeparadorDecimal { get; set; }
        public string SeparadorMilhar { get; set; }
    }
    
    #endregion
}