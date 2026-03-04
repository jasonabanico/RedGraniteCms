import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import pageService from '../../../services/pages';
import { ItemInput } from '../../../../__generated__/globalTypes';
import { IEditPageFormState } from './types';

const initialState: IEditPageFormState = {
    status: 'idle',
    error: null,
    page: null
};

export const updatePage = createAsyncThunk(
    'editPageForm/updatePage',
    async (itemInput: ItemInput, { rejectWithValue }) => {
        try {
            const data = await pageService.updatePage(itemInput);
            return data;
        } catch (err: any) {
            return rejectWithValue(err.response?.data || 'Unknown error');
        }
    }
);

const editPageFormSlice = createSlice({
    name: 'editPageForm',
    initialState,
    reducers: {
        resetEditForm: (state) => {
            state.status = 'idle';
            state.error = null;
            state.page = null;
        }
    },
    extraReducers: (builder) => {
        builder
            .addCase(updatePage.pending, (state) => {
                state.status = 'loading';
            })
            .addCase(updatePage.fulfilled, (state) => {
                state.status = 'succeeded';
            })
            .addCase(updatePage.rejected, (state, action) => {
                state.status = 'failed';
                state.error = typeof action.payload === 'string' ? action.payload : 'Failed to save page';
            });
    }
});

export const { resetEditForm } = editPageFormSlice.actions;
export default editPageFormSlice.reducer;
