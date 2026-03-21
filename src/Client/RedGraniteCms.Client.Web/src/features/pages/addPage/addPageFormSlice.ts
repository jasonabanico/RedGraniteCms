import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import pageService from '../../../modules/pages';
import type { PageInput } from '../../../modules/pages/types';
import { IAddPageFormState } from './types';

const initialState: IAddPageFormState = {
    status: 'idle',
    error: null
};

export const addPage = createAsyncThunk(
    'addPageForm/addPage',
    async (input: PageInput, { rejectWithValue }) => {
        try {
            const data = await pageService.addPage(input);
            return data;
        } catch (err: any) {
            return rejectWithValue(err.message ?? 'Unknown error');
        }
    }
)

const addPageFormSlice = createSlice({
    name: 'addPageForm',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(addPage.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(addPage.fulfilled, (state) => {
                state.status = 'succeeded';
            })
            .addCase(addPage.rejected, (state) => {
                state.status = 'failed';
                state.error = 'Failed to save new page';
            })
    }
});

export default addPageFormSlice.reducer;
