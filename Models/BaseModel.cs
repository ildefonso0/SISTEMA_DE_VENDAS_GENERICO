using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SISTEMA_DE_VENDAS_GENERICO.Models
{
    /// <summary>
    /// Classe base para todos os modelos do sistema
    /// Implementa INotifyPropertyChanged para binding automático com a UI
    /// </summary>
    public abstract class BaseModel : INotifyPropertyChanged
    {
        #region Eventos
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion

        #region Propriedades Comuns
        
        private int _id;
        private DateTime _dataCriacao;
        private DateTime _dataAtualizacao;
        private bool _ativo = true;

        /// <summary>
        /// Identificador único do registro
        /// </summary>
        public virtual int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Data de criação do registro
        /// </summary>
        public virtual DateTime DataCriacao
        {
            get => _dataCriacao;
            set => SetProperty(ref _dataCriacao, value);
        }

        /// <summary>
        /// Data da última atualização do registro
        /// </summary>
        public virtual DateTime DataAtualizacao
        {
            get => _dataAtualizacao;
            set => SetProperty(ref _dataAtualizacao, value);
        }

        /// <summary>
        /// Indica se o registro está ativo no sistema
        /// </summary>
        public virtual bool Ativo
        {
            get => _ativo;
            set => SetProperty(ref _ativo, value);
        }
        
        #endregion

        #region Construtor
        
        protected BaseModel()
        {
            DataCriacao = DateTime.Now;
            DataAtualizacao = DateTime.Now;
            Ativo = true;
        }
        
        #endregion

        #region Métodos Protegidos
        
        /// <summary>
        /// Define o valor de uma propriedade e notifica sobre a mudança
        /// </summary>
        /// <typeparam name="T">Tipo da propriedade</typeparam>
        /// <param name="field">Campo de apoio da propriedade</param>
        /// <param name="value">Novo valor</param>
        /// <param name="propertyName">Nome da propriedade (preenchido automaticamente)</param>
        /// <returns>True se o valor foi alterado</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            
            // Atualiza a data de modificação automaticamente
            if (propertyName != nameof(DataAtualizacao))
            {
                DataAtualizacao = DateTime.Now;
            }
            
            return true;
        }

        /// <summary>
        /// Notifica sobre mudança de propriedade
        /// </summary>
        /// <param name="propertyName">Nome da propriedade</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion

        #region Métodos Virtuais
        
        /// <summary>
        /// Valida os dados do modelo
        /// </summary>
        /// <returns>True se os dados são válidos</returns>
        public virtual bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Obtém mensagens de validação
        /// </summary>
        /// <returns>Array com mensagens de erro</returns>
        public virtual string[] GetValidationErrors()
        {
            return new string[0];
        }
        
        #endregion

        #region Sobrescrita de Métodos
        
        public override bool Equals(object obj)
        {
            if (obj is BaseModel other)
            {
                return Id == other.Id && Id != 0;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id: {Id}]";
        }
        
        #endregion
    }
}