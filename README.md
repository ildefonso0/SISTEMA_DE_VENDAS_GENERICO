# Sistema de Vendas Genérico

Sistema de vendas desenvolvido em C# WPF com SQL Server, focado no mercado angolano.

## 📋 Características

- **Plataforma**: C# (.NET Framework 4.8)
- **Interface**: WPF (Windows Presentation Foundation)
- **Banco de Dados**: SQL Server (LocalDB)
- **Arquitetura**: Clean Architecture simples
- **Foco**: Simplicidade e funcionalidade

## 🎨 Identidade Visual

- **Fundo Principal**: #F4F4F4 (Cinza claro)
- **Menu Lateral**: #2C3E50 (Azul petróleo)
- **Botões Principais**: #3498DB (Azul vibrante)
- **Botões de Alerta**: #E74C3C (Vermelho)
- **Fonte**: Segoe UI

## 🚀 Como Executar

### Pré-requisitos
- Visual Studio 2019 ou superior
- .NET Framework 4.8
- SQL Server LocalDB

### Passos para Execução

1. **Clone ou baixe o projeto**
2. **Abra o projeto no Visual Studio**
3. **Execute o script SQL** (`Scripts/CriarBancoCompleto.sql`) no SQL Server Management Studio
4. **Compile e execute o projeto**

### Dados de Acesso

Após executar o script SQL, você terá os seguintes usuários:

| Usuário | Senha | Nível de Acesso |
|---------|-------|-----------------|
| admin | admin123 | Administrador |
| gerente | gerente123 | Gerente |
| vendedor | venda123 | Vendedor |

## 📊 Módulos do Sistema

### 1. Dashboard
- Total de vendas do dia
- Produtos mais vendidos
- Estoque baixo
- Últimas vendas

### 2. Vendas
- Busca de produtos por nome ou código
- Carrinho de compras
- Aplicação de desconto
- Múltiplas formas de pagamento
- Cliente opcional
- Impressão de recibo

### 3. Produtos
- Cadastro completo de produtos
- Categorias
- Controle de estoque
- Imagens (opcional)

### 4. Estoque
- Movimentação de entrada e saída
- Histórico de movimentações
- Alertas de estoque baixo
- Controle por motivo

### 5. Clientes
- Cadastro de clientes
- Histórico de compras
- Informações de contato

### 6. Fornecedores
- Cadastro completo de fornecedores
- Informações de contato

### 7. Relatórios
- Vendas por período
- Produtos mais vendidos
- Clientes mais ativos
- Movimentação de estoque
- Resumo financeiro
- Exportação para CSV

### 8. Usuários
- Gestão de usuários
- Níveis de acesso
- Controle de permissões
- Ativação/desativação

### 9. Configurações
- Dados da empresa
- Formas de pagamento
- Backup do banco

## 🔐 Níveis de Acesso

### Administrador
- Acesso total a todos os módulos
- Gestão de usuários
- Configurações do sistema

### Gerente
- Acesso a vendas, produtos, estoque, clientes, fornecedores e relatórios
- Não pode gerir usuários

### Vendedor
- Acesso limitado a vendas e consulta de produtos
- Não pode cadastrar produtos ou ver relatórios

## 🗃️ Estrutura do Banco de Dados

O sistema utiliza as seguintes tabelas principais:

- **Usuarios**: Controle de acesso
- **Produtos**: Cadastro de produtos
- **Categorias**: Categorização de produtos
- **Clientes**: Cadastro de clientes
- **Vendas**: Registro de vendas
- **ItensVenda**: Itens de cada venda
- **Fornecedores**: Cadastro de fornecedores
- **MovimentacaoEstoque**: Controle de estoque
- **FormasPagamento**: Formas de pagamento
- **Configuracoes**: Configurações do sistema

## 🛠️ Desenvolvimento

### Estrutura de Pastas
```
SISTEMA_DE_VENDAS_GENERICO/
├── Models/           # Classes de modelo
├── Views/            # Telas XAML
├── Data/             # Conexão com banco
├── Scripts/          # Scripts SQL
└── Properties/       # Configurações do projeto
```

### Tecnologias Utilizadas
- **WPF**: Interface gráfica
- **ADO.NET**: Acesso a dados
- **SQL Server**: Banco de dados
- **C#**: Linguagem de programação

## ✨ Funcionalidades Implementadas

### ✅ Completas
- **Login**: Sistema de autenticação com níveis de acesso
- **Dashboard**: Estatísticas em tempo real
- **Vendas**: Sistema completo de PDV com impressão de recibo
- **Produtos**: CRUD completo com categorias
- **Clientes**: Gestão completa de clientes
- **Fornecedores**: Cadastro e gestão de fornecedores
- **Usuários**: Gestão completa com permissões
- **Estoque**: Controle de movimentação com histórico
- **Relatórios**: Múltiplos relatórios com exportação
- **Configurações**: Configurações da empresa e sistema

## 📝 Observações Importantes

1. **Senhas**: Sistema utiliza senhas simples sem criptografia (conforme especificação)
2. **Banco Local**: Configurado para SQL Server LocalDB
3. **Interface**: Design responsivo e intuitivo
4. **Adaptação**: Focado no mercado angolano
5. **Moeda**: Valores em Kwanza Angolano (AOA)

## 🔧 Configuração do Banco

1. Certifique-se de que o SQL Server LocalDB está instalado
2. Execute o script `Scripts/CriarBancoCompleto.sql`
4. Verifique a string de conexão no `App.config`

## 📞 Suporte

Para dúvidas ou problemas:
- Verifique se todas as dependências estão instaladas
- Confirme se o banco de dados está configurado corretamente
- Verifique se o .NET Framework 4.8 está instalado

---

**Desenvolvido para o mercado angolano com foco em simplicidade e funcionalidade.** 