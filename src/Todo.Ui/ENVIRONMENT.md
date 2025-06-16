# Configuração de Ambientes

## Variáveis de Ambiente

A aplicação utiliza variáveis de ambiente para gerenciar diferentes configurações entre desenvolvimento e produção.

### Arquivos de Ambiente

- `src/environments/environment.ts` - Configurações para desenvolvimento
- `src/environments/environment.prod.ts` - Configurações para produção

### Variáveis Disponíveis

| Variável | Descrição | Desenvolvimento | Produção |
|----------|-----------|-----------------|----------|
| `apiUrl` | URL base da API | `http://localhost:5000` | `https://sua-api-producao.com` |
| `production` | Flag de produção | `false` | `true` |

### Como Usar

1. **Desenvolvimento**: Execute `npm start` (usa environment.ts)
2. **Produção**: Execute `npm run build` (usa environment.prod.ts)

### Personalização

Para adicionar novas variáveis:

1. Adicione a variável em `environment.ts`
2. Adicione a variável em `environment.prod.ts`
3. Use `environment.nomeDaVariavel` nos serviços

### Exemplo de Uso

```typescript
import { environment } from '../environments/environment';

export class MeuService {
  private apiUrl = environment.apiUrl;
}
``` 