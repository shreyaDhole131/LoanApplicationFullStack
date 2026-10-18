import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  // Preserve any Authorization header already supplied by the request.
  // This is essential for temporary 2FA challenge tokens.
  if (req.headers.has('Authorization')) {
    return next(req);
  }

  // Add the saved access token only when no Authorization header exists.
  const token =
    typeof localStorage !== 'undefined'
      ? localStorage.getItem('token')
      : null;

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};