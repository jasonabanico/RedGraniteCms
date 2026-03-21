import React, { Component, ErrorInfo, ReactNode } from 'react';
import { Alert, AlertTitle, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { AlertCircle } from 'lucide-react';

interface Props {
    children: ReactNode;
    fallback?: ReactNode;
}

interface State {
    hasError: boolean;
    error: Error | null;
    errorInfo: ErrorInfo | null;
}

/**
 * Error Boundary component for catching and displaying React errors gracefully.
 * Wrap this around components that may throw errors to prevent the entire app from crashing.
 */
export class ErrorBoundary extends Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = {
            hasError: false,
            error: null,
            errorInfo: null
        };
    }

    static getDerivedStateFromError(error: Error): Partial<State> {
        return { hasError: true, error };
    }

    componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
        this.setState({ errorInfo });
        console.error('ErrorBoundary caught an error:', error, errorInfo);
    }

    handleReset = (): void => {
        this.setState({
            hasError: false,
            error: null,
            errorInfo: null
        });
    };

    handleReload = (): void => {
        window.location.reload();
    };

    render(): ReactNode {
        if (this.state.hasError) {
            if (this.props.fallback) {
                return this.props.fallback;
            }

            return (
                <div className="container mx-auto mt-10 max-w-2xl px-4">
                    <Alert variant="destructive">
                        <AlertCircle className="h-4 w-4" />
                        <AlertTitle>Something went wrong</AlertTitle>
                        <AlertDescription>
                            <p>
                                An unexpected error occurred. Please try refreshing the page or contact support if the problem persists.
                            </p>
                            {import.meta.env.DEV && this.state.error && (
                                <details className="mt-3">
                                    <summary className="cursor-pointer">Error Details</summary>
                                    <pre className="mt-2 rounded-md bg-muted p-3 text-sm text-destructive overflow-auto">
                                        {this.state.error.toString()}
                                        {this.state.errorInfo?.componentStack}
                                    </pre>
                                </details>
                            )}
                            <div className="flex gap-2 mt-4">
                                <Button variant="outline" onClick={this.handleReset}>
                                    Try Again
                                </Button>
                                <Button variant="destructive" onClick={this.handleReload}>
                                    Reload Page
                                </Button>
                            </div>
                        </AlertDescription>
                    </Alert>
                </div>
            );
        }

        return this.props.children;
    }
}
