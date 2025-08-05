using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SISTEMA_DE_VENDAS_GENERICO.Utils
{
    /// <summary>
    /// Classe auxiliar para validações diversas
    /// </summary>
    public static class ValidationHelper
    {
        #region Validações de Email
        
        /// <summary>
        /// Valida se um email tem formato válido
        /// </summary>
        /// <param name="email">Email a ser validado</param>
        /// <returns>True se o email é válido</returns>
        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        #endregion

        #region Validações de Telefone
        
        /// <summary>
        /// Valida se um telefone angolano tem formato válido
        /// </summary>
        /// <param name="telefone">Telefone a ser validado</param>
        /// <returns>True se o telefone é válido</returns>
        public static bool ValidarTelefoneAngolano(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return false;
            
            // Remove caracteres especiais
            string numeroLimpo = Regex.Replace(telefone, @"[^\d]", "");
            
            // Verifica formatos válidos para Angola
            // 9 dígitos: 9XXXXXXXX
            // 12 dígitos com código do país: 244XXXXXXXXX
            return numeroLimpo.Length == 9 && numeroLimpo.StartsWith("9") ||
                   numeroLimpo.Length == 12 && numeroLimpo.StartsWith("244");
        }

        /// <summary>
        /// Formata um telefone angolano para exibição
        /// </summary>
        /// <param name="telefone">Telefone a ser formatado</param>
        /// <returns>Telefone formatado</returns>
        public static string FormatarTelefoneAngolano(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return string.Empty;
            
            string numeroLimpo = Regex.Replace(telefone, @"[^\d]", "");
            
            if (numeroLimpo.Length == 9 && numeroLimpo.StartsWith("9"))
            {
                // Formato: +244 9XX XXX XXX
                return $"+244 {numeroLimpo.Substring(0, 3)} {numeroLimpo.Substring(3, 3)} {numeroLimpo.Substring(6, 3)}";
            }
            else if (numeroLimpo.Length == 12 && numeroLimpo.StartsWith("244"))
            {
                // Remove o código do país e formata
                string numero = numeroLimpo.Substring(3);
                return $"+244 {numero.Substring(0, 3)} {numero.Substring(3, 3)} {numero.Substring(6, 3)}";
            }
            
            return telefone; // Retorna original se não conseguir formatar
        }
        
        #endregion

        #region Validações de Documento
        
        /// <summary>
        /// Valida um Bilhete de Identidade angolano
        /// </summary>
        /// <param name="bi">Número do BI</param>
        /// <returns>True se o BI é válido</returns>
        public static bool ValidarBIAngolano(string bi)
        {
            if (string.IsNullOrWhiteSpace(bi))
                return false;
            
            // Remove caracteres especiais
            string biLimpo = Regex.Replace(bi.ToUpper(), @"[^\dA-Z]", "");
            
            // Formato típico: 9 dígitos + 2 letras (ex: 123456789BA)
            return Regex.IsMatch(biLimpo, @"^\d{9}[A-Z]{2}$");
        }

        /// <summary>
        /// Valida um número de passaporte
        /// </summary>
        /// <param name="passaporte">Número do passaporte</param>
        /// <returns>True se o passaporte é válido</returns>
        public static bool ValidarPassaporte(string passaporte)
        {
            if (string.IsNullOrWhiteSpace(passaporte))
                return false;
            
            string passaporteLimpo = Regex.Replace(passaporte.ToUpper(), @"[^\dA-Z]", "");
            
            // Formato típico: 2 letras + 6-8 dígitos
            return Regex.IsMatch(passaporteLimpo, @"^[A-Z]{2}\d{6,8}$");
        }
        
        #endregion

        #region Validações de Texto
        
        /// <summary>
        /// Valida se um texto contém apenas letras e espaços
        /// </summary>
        /// <param name="texto">Texto a ser validado</param>
        /// <returns>True se contém apenas letras e espaços</returns>
        public static bool ValidarApenasLetras(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;
            
            return Regex.IsMatch(texto, @"^[a-zA-ZÀ-ÿ\s]+$");
        }

        /// <summary>
        /// Valida se um texto contém apenas números
        /// </summary>
        /// <param name="texto">Texto a ser validado</param>
        /// <returns>True se contém apenas números</returns>
        public static bool ValidarApenasNumeros(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;
            
            return Regex.IsMatch(texto, @"^\d+$");
        }

        /// <summary>
        /// Valida se um texto é alfanumérico
        /// </summary>
        /// <param name="texto">Texto a ser validado</param>
        /// <returns>True se é alfanumérico</returns>
        public static bool ValidarAlfanumerico(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;
            
            return Regex.IsMatch(texto, @"^[a-zA-Z0-9]+$");
        }
        
        #endregion

        #region Validações de Código de Barras
        
        /// <summary>
        /// Valida um código de barras EAN-13
        /// </summary>
        /// <param name="codigo">Código de barras</param>
        /// <returns>True se o código é válido</returns>
        public static bool ValidarEAN13(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;
            
            string codigoLimpo = Regex.Replace(codigo, @"[^\d]", "");
            
            if (codigoLimpo.Length != 13)
                return false;
            
            // Calcula o dígito verificador
            int soma = 0;
            for (int i = 0; i < 12; i++)
            {
                int digito = int.Parse(codigoLimpo[i].ToString());
                soma += (i % 2 == 0) ? digito : digito * 3;
            }
            
            int digitoVerificador = (10 - (soma % 10)) % 10;
            int ultimoDigito = int.Parse(codigoLimpo[12].ToString());
            
            return digitoVerificador == ultimoDigito;
        }

        /// <summary>
        /// Valida um código de barras EAN-8
        /// </summary>
        /// <param name="codigo">Código de barras</param>
        /// <returns>True se o código é válido</returns>
        public static bool ValidarEAN8(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;
            
            string codigoLimpo = Regex.Replace(codigo, @"[^\d]", "");
            
            if (codigoLimpo.Length != 8)
                return false;
            
            // Calcula o dígito verificador
            int soma = 0;
            for (int i = 0; i < 7; i++)
            {
                int digito = int.Parse(codigoLimpo[i].ToString());
                soma += (i % 2 == 0) ? digito * 3 : digito;
            }
            
            int digitoVerificador = (10 - (soma % 10)) % 10;
            int ultimoDigito = int.Parse(codigoLimpo[7].ToString());
            
            return digitoVerificador == ultimoDigito;
        }
        
        #endregion

        #region Validações de Data
        
        /// <summary>
        /// Valida se uma data está dentro de um intervalo válido
        /// </summary>
        /// <param name="data">Data a ser validada</param>
        /// <param name="dataMinima">Data mínima permitida</param>
        /// <param name="dataMaxima">Data máxima permitida</param>
        /// <returns>True se a data está no intervalo</returns>
        public static bool ValidarIntervaloData(DateTime data, DateTime? dataMinima = null, DateTime? dataMaxima = null)
        {
            if (dataMinima.HasValue && data < dataMinima.Value)
                return false;
            
            if (dataMaxima.HasValue && data > dataMaxima.Value)
                return false;
            
            return true;
        }

        /// <summary>
        /// Valida se uma data de nascimento é válida
        /// </summary>
        /// <param name="dataNascimento">Data de nascimento</param>
        /// <returns>True se a data é válida</returns>
        public static bool ValidarDataNascimento(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            var idadeMinima = hoje.AddYears(-120); // 120 anos atrás
            
            return ValidarIntervaloData(dataNascimento, idadeMinima, hoje);
        }
        
        #endregion

        #region Validações Numéricas
        
        /// <summary>
        /// Valida se um valor está dentro de um intervalo
        /// </summary>
        /// <param name="valor">Valor a ser validado</param>
        /// <param name="minimo">Valor mínimo</param>
        /// <param name="maximo">Valor máximo</param>
        /// <returns>True se está no intervalo</returns>
        public static bool ValidarIntervalo(decimal valor, decimal minimo, decimal maximo)
        {
            return valor >= minimo && valor <= maximo;
        }

        /// <summary>
        /// Valida se um valor é positivo
        /// </summary>
        /// <param name="valor">Valor a ser validado</param>
        /// <returns>True se é positivo</returns>
        public static bool ValidarPositivo(decimal valor)
        {
            return valor > 0;
        }

        /// <summary>
        /// Valida se um valor não é negativo
        /// </summary>
        /// <param name="valor">Valor a ser validado</param>
        /// <returns>True se não é negativo</returns>
        public static bool ValidarNaoNegativo(decimal valor)
        {
            return valor >= 0;
        }
        
        #endregion

        #region Validações de Senha
        
        /// <summary>
        /// Valida a força de uma senha
        /// </summary>
        /// <param name="senha">Senha a ser validada</param>
        /// <returns>Nível de força da senha</returns>
        public static PasswordStrength ValidarForcaSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return PasswordStrength.VeryWeak;
            
            int pontos = 0;
            
            // Comprimento
            if (senha.Length >= 8) pontos++;
            if (senha.Length >= 12) pontos++;
            
            // Caracteres
            if (Regex.IsMatch(senha, @"[a-z]")) pontos++; // Minúsculas
            if (Regex.IsMatch(senha, @"[A-Z]")) pontos++; // Maiúsculas
            if (Regex.IsMatch(senha, @"\d")) pontos++;    // Números
            if (Regex.IsMatch(senha, @"[!@#$%^&*(),.?""':;{}|<>]")) pontos++; // Especiais
            
            // Variedade
            if (senha.Distinct().Count() >= senha.Length * 0.7) pontos++;
            
            switch (pontos)
            {
                case 0:
                case 1:
                    return PasswordStrength.VeryWeak;
                case 2:
                case 3:
                    return PasswordStrength.Weak;
                case 4:
                case 5:
                    return PasswordStrength.Medium;
                case 6:
                    return PasswordStrength.Strong;
                default:
                    return PasswordStrength.VeryStrong;
            }
        }

        /// <summary>
        /// Obtém a descrição da força da senha
        /// </summary>
        /// <param name="strength">Força da senha</param>
        /// <returns>Descrição textual</returns>
        public static string GetDescricaoForcaSenha(PasswordStrength strength)
        {
            switch (strength)
            {
                case PasswordStrength.VeryWeak:
                    return "Muito Fraca";
                case PasswordStrength.Weak:
                    return "Fraca";
                case PasswordStrength.Medium:
                    return "Média";
                case PasswordStrength.Strong:
                    return "Forte";
                case PasswordStrength.VeryStrong:
                    return "Muito Forte";
                default:
                    return "Indefinida";
            }
        }
        
        #endregion

        #region Validações Compostas
        
        /// <summary>
        /// Valida múltiplas condições e retorna todas as mensagens de erro
        /// </summary>
        /// <param name="validacoes">Lista de validações</param>
        /// <returns>Lista de mensagens de erro</returns>
        public static List<string> ValidarMultiplas(params (bool isValid, string errorMessage)[] validacoes)
        {
            var erros = new List<string>();
            
            foreach (var (isValid, errorMessage) in validacoes)
            {
                if (!isValid)
                    erros.Add(errorMessage);
            }
            
            return erros;
        }

        /// <summary>
        /// Valida se todas as condições são verdadeiras
        /// </summary>
        /// <param name="condicoes">Lista de condições</param>
        /// <returns>True se todas são verdadeiras</returns>
        public static bool ValidarTodas(params bool[] condicoes)
        {
            return condicoes.All(c => c);
        }

        /// <summary>
        /// Valida se pelo menos uma condição é verdadeira
        /// </summary>
        /// <param name="condicoes">Lista de condições</param>
        /// <returns>True se pelo menos uma é verdadeira</returns>
        public static bool ValidarAlgumaCondicao(params bool[] condicoes)
        {
            return condicoes.Any(c => c);
        }
        
        #endregion
    }

    #region Enumerações
    
    /// <summary>
    /// Níveis de força de senha
    /// </summary>
    public enum PasswordStrength
    {
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }
    
    #endregion
}