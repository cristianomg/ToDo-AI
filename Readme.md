## 🗺️ Roadmap - Todo List Application

Este projeto tem como objetivo criar um sistema de **gerenciamento de tarefas** (Todo List) com foco em produtividade, organização e usabilidade. As tarefas podem ser diárias, semanais ou mensais.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
- **Docker** instalado e em execução

### Executando a Aplicação

1. Clone o repositório
2. Navegue até a pasta do projeto
3. Execute o comando:

```bash
docker-compose up -d --build
```

### URLs da Aplicação

- **Frontend**: http://localhost:5000/login
- **Backend (Swagger)**: http://localhost:8080/swagger/index.html

---

## 🧪 Como Testar o Sistema

Para facilitar os testes, foram criados **10 usuários de teste** no sistema. Você pode usar qualquer um deles para fazer login sem precisar criar uma nova conta.

### Usuários de Teste Disponíveis:
- **John Doe**
- **Jane Smith**
- **Michael Johnson**
- **Emily Davis**
- **Robert Wilson**
- **Sarah Brown**
- **David Miller**
- **Lisa Taylor**
- **James Anderson**
- **Jennifer Thomas**

### Como Fazer Login:
1. Acesse: http://localhost:5000/login
2. No dropdown de seleção de usuário, escolha qualquer um dos 10 usuários disponíveis
3. Clique em "Entrar" para acessar o sistema

---

## 🛠️ Tecnologias Utilizadas

- ASP.NET Core (.NET 8)
- Angular
- PostgreSQL
- Entity Framework Core
- MediatR (arquitetura CQRS)

---

## ✅ Features Implementadas

### 🎯 Funcionalidades Básicas (MVP)
- [x] Login de usuários
- [x] CRUD de tarefas (criar, editar, concluir, excluir)
- [x] Campos da tarefa: título, descrição, tipo (diária/semanal/mensal), data de vencimento, prioridade
- [x] Visualização das tarefas por período (diário, semanal, mensal)
- [x] Filtros por tipo e data
- [x] Alteração de status da tarefas (arrastar e soltar)

### 🔄 Organização e Automação
- [x] Tarefas recorrentes (diária, semanal, mensal)
- [x] Subtarefas (checklists)

### 🎨 Personalização
- [x] Temas personalizados (modo claro/escuro, cores)

### 🧪 Testes e Qualidade
- [x] Documentação da API com Swagger
- [x] Tratamento global de exceções
- [x] Testes unitários com xUnit e Moq

---

## 🚧 Features Futuras

### 🔔 Organização e Automação (Pendentes)
- [ ] Lembretes e notificações
- [ ] Barra de progresso por dia, semana e mês
- [ ] Histórico de tarefas concluídas
- [ ] Cancelamento de recorrência

### 👥 Colaboração
- [ ] Compartilhamento de tarefas com outros usuários
- [ ] Comentários nas tarefas
- [ ] Listas compartilhadas entre usuários (família, time, casal)

### 🤖 Inteligência e Personalização
- [ ] Sugestões automáticas com base no uso
- [ ] Estatísticas de produtividade
- [ ] Integração com Google Calendar / Outlook
