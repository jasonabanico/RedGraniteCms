const API_BASE_URL = import.meta.env.VITE_GRAPHQL_URL?.replace('/graphql', '') || 'https://localhost:7236';

export interface AuthResponse {
    token: string;
    email: string;
}

export async function login(email: string, password: string): Promise<AuthResponse> {
    const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
        const error = await response.json().catch(() => ({}));
        throw new Error(error.error || 'Login failed');
    }

    const data: AuthResponse = await response.json();
    localStorage.setItem('auth_token', data.token);
    localStorage.setItem('auth_email', data.email);
    return data;
}

export async function register(email: string, password: string): Promise<AuthResponse> {
    const response = await fetch(`${API_BASE_URL}/api/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
        const error = await response.json().catch(() => ({}));
        const messages = Object.values(error).flat().join(' ');
        throw new Error(messages || 'Registration failed');
    }

    const data: AuthResponse = await response.json();
    localStorage.setItem('auth_token', data.token);
    localStorage.setItem('auth_email', data.email);
    return data;
}

export function logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_email');
}

export function getToken(): string | null {
    return localStorage.getItem('auth_token');
}

export function getEmail(): string | null {
    return localStorage.getItem('auth_email');
}

export function isAuthenticated(): boolean {
    const token = getToken();
    if (!token) return false;

    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.exp * 1000 > Date.now();
    } catch {
        return false;
    }
}
