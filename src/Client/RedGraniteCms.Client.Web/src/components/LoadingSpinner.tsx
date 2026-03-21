import { Loader2 } from 'lucide-react';

interface LoadingSpinnerProps {
    message?: string;
    size?: 'sm' | undefined;
}

/**
 * Reusable loading spinner component.
 */
export function LoadingSpinner({ message = 'Loading...', size }: LoadingSpinnerProps) {
    const iconSize = size === 'sm' ? 'h-5 w-5' : 'h-8 w-8';
    return (
        <div className="flex flex-col items-center justify-center py-10">
            <Loader2 className={`${iconSize} animate-spin text-primary`} />
            <p className="mt-3 text-sm text-muted-foreground">{message}</p>
        </div>
    );
}
