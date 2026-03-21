import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { login, register } from '../services/auth';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { ErrorAlert } from '../components';

export function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isRegistering, setIsRegistering] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setIsSubmitting(true);

        try {
            if (isRegistering) {
                await register(email, password);
            } else {
                await login(email, password);
            }
            navigate('/');
        } catch (err: any) {
            setError(err.message || 'Authentication failed');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="container mx-auto max-w-sm px-4 mt-8">
            <h2 className="text-2xl font-bold mb-4">
                {isRegistering ? 'Register' : 'Login'}
            </h2>

            {error && <ErrorAlert message={error} onDismiss={() => setError(null)} />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                    <Label htmlFor="email">Email</Label>
                    <Input
                        id="email"
                        type="email"
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                        placeholder="admin@example.com"
                        required
                        disabled={isSubmitting}
                    />
                </div>

                <div className="space-y-2">
                    <Label htmlFor="password">Password</Label>
                    <Input
                        id="password"
                        type="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        placeholder="••••••••"
                        required
                        disabled={isSubmitting}
                    />
                </div>

                <Button type="submit" className="w-full" disabled={isSubmitting}>
                    {isSubmitting ? 'Please wait...' : isRegistering ? 'Register' : 'Login'}
                </Button>
            </form>

            <p className="text-sm text-muted-foreground mt-4 text-center">
                {isRegistering ? 'Already have an account?' : "Don't have an account?"}{' '}
                <button
                    type="button"
                    className="underline"
                    onClick={() => { setIsRegistering(!isRegistering); setError(null); }}
                >
                    {isRegistering ? 'Login' : 'Register'}
                </button>
            </p>
        </div>
    );
}
