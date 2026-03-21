import type { PageInput } from "../../../modules/pages/types";

export interface IEditPageFormState {
    status: 'idle' | 'loading' | 'succeeded' | 'failed';
    error: string | null;
    page: PageInput | null;
}
