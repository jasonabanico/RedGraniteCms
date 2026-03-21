import { Alert, AlertTitle, AlertDescription } from '@/components/ui/alert';
import { AlertCircle } from 'lucide-react';

interface ErrorAlertProps {
    title?: string;
    message: string;
    onDismiss?: () => void;
}

/**
 * Reusable error alert component.
 */
export function ErrorAlert({ title = 'Error', message, onDismiss }: ErrorAlertProps) {
    return (
        <Alert variant="destructive" onDismiss={onDismiss}>
            <AlertCircle className="h-4 w-4" />
            <AlertTitle>{title}</AlertTitle>
            <AlertDescription>{message}</AlertDescription>
        </Alert>
    );
}
