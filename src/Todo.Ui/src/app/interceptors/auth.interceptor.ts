import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
    const authService = inject(AuthService);

    // Obter o userId do AuthService
    const userId = authService.getUserId();

    if (userId) {
        // Adicionar o header X-User-Id se o usuário estiver logado
        const modifiedRequest = request.clone({
            setHeaders: {
                'X-User-Id': userId.toString()
            }
        });

        return next(modifiedRequest);
    }

    // Se não há usuário logado, continuar com a requisição original
    return next(request);
}; 