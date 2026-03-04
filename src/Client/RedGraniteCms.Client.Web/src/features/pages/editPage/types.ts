import { ItemInput } from "../../../../__generated__/globalTypes";

export interface IEditPageFormState {
    status: 'idle' | 'loading' | 'succeeded' | 'failed';
    error: string | null;
    page: ItemInput | null;
}
