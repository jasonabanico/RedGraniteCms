import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import pageService from '../../../services/pages';
import { ItemInput } from '../../../../__generated__/globalTypes';
import { IAddPageFormState } from './types';

const initialState: IAddPageFormState = {
    status: 'idle',
    error: null
};

export const addPage = createAsyncThunk(
    'addPageForm/addPage',
    async (itemInput: ItemInput, { rejectWithValue }) => {
        try {
            const data = await pageService.addPage(itemInput);
            return data;
        } catch (err: any) {
            return rejectWithValue(err.response.data);
        }
    }
)

const addPageFormSlice = createSlice({
    name: 'addPageForm',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(addPage.pending, (state, action) => {
                state.status = 'loading';
            })
            .addCase(addPage.fulfilled, (state, action) => {
                state.status = 'succeeded';
            })
            .addCase(addPage.rejected, (state, action) => {
                state.status = 'failed';
                state.error = 'Failed to save new page';
            })
    }
});

export default addPageFormSlice.reducer;
