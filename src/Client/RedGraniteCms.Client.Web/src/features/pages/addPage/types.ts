export interface IAddPageFormState {
    status: 'idle' | 'loading' | 'succeeded' | 'failed';
    error: string | null;
}
